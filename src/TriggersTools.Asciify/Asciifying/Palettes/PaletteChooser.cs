using TriggersTools.Asciify.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using TriggersTools.Asciify.ColorMine.Converters;
using TriggersTools.Asciify.ColorMine.Comparisons;
using ScoredColors = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.Drawing.Color, long>>;
using System.Diagnostics;

namespace TriggersTools.Asciify.Asciifying.Palettes {

	public struct ColorCount {
		public Color Color { get; }
		public long Count { get; }

		public ColorCount(Color color, long count) {
			Color = color;
			Count = count;
		}
		public ColorCount(KeyValuePair<Color, long> pair) {
			Color = pair.Key;
			Count = pair.Value;
		}

		public static implicit operator ColorCount(KeyValuePair<Color, long> pair) {
			return new ColorCount(pair);
		}
	}

	public static class PaletteChooser {
		//https://stackoverflow.com/questions/19808743/for-an-jpg-image-file-get-3-4-average-main-colors
		public static List<Color> FindMainColorsByHsb(Bitmap bitmap, int maxCount,
			float minHueDiff = 3f, float minSatDiff = 0.03f, float minBriDiff = 0.03f)
		{
			/*Dictionary<Color, long> colors = new Dictionary<Color, long>(); // color, pixelcount i.e ('#FFFFFF',100);
			
			using (DisposableBitmapData bmpData = bitmap.LockRead(PixelFormat.Format24bppRgb)) {
				BitmapData data = bmpData;

				foreach (PixelPoint p in data.ForEachPixel()) {
					Color color = p.Color;

					if (color.R == 255 && color.G == 255 && color.B == 255)
						continue;
					
					if (colors.ContainsKey(color))
						colors[color]++;
					else
						colors.Add(color, 1);
				}
			}*/
			
			// sort dictionary of colors so that most used is at top
			//IEnumerable<Color> allColors = colors.OrderByDescending(x => x.Value).Select(p => p.Key);
			var colors = ListAllColorsByMostCommon(bitmap);

			return GetMainColorsByHsb(colors, maxCount, minHueDiff, minSatDiff, minBriDiff);
		}

		public static List<Color> FindMainColorsByLab(Bitmap bitmap, int maxCount,
			double minDiff, double lWeight = 1d)
		{
			var colors = ListAllColorsByMostCommon(bitmap);

			return GetMainColorsByLab(colors, maxCount, minDiff, lWeight);
		}

		public static IEnumerable<ColorCount> ListAllColorsByMostCommon(Bitmap bitmap) {
			Dictionary<Color, long> colors = new Dictionary<Color, long>();

			using (DisposableBitmapData bmpData = bitmap.LockRead(PixelFormat.Format24bppRgb)) {
				BitmapData data = bmpData;

				foreach (PixelPoint p in data.ForEachPixel()) {
					Color color = p.Color;

					if (colors.TryGetValue(color, out long count))
						colors[color] = count + 1;
					else
						colors.Add(color, 1);
				}
			}
			return colors.OrderByDescending(x => x.Value).Select(p => (ColorCount) p);//.Select(p => p.Key);
		}
		

		private static List<Color> GetMainColorsByLab(IEnumerable<ColorCount> colors, int maxCount, double minDiff, double lWeight = 1d) {
			//List<Tuple<Color, ColorLab>> mainColors = new List<Tuple<Color, ColorLab>>();
			List<Color> results = new List<Color>();
			List<ColorLab> labs = new List<ColorLab>();

			long lastCount = -1;
			foreach (ColorCount colorCount in colors) {
				long count = colorCount.Count;
				Color color = colorCount.Color;
				ColorLab lab = LabConverter.ToLab(color);
				lab.L *= lWeight;

				bool uniqueColorFound = true;
				foreach (ColorLab labOther in labs) {
					double score = Cie76Comparison.CompareS(lab, labOther);

					if (score < minDiff) {
						uniqueColorFound = false;
						break;
					}
				}
				if (uniqueColorFound) {       // color differs by min ammount of HSL so add to response
					results.Add(color);
					labs.Add(lab);
					if (results.Count == maxCount)
						break;
				}
				lastCount = count;
			}

			Trace.WriteLine($"Colors Found: {results.Count}/{maxCount}");
			return results;
		}

		private static List<Color> GetMainColorsByHsb(IEnumerable<ColorCount> colors, int maxCount, float minHueDiff, float minSatDiff, float minBriDiff) {
			List<Color> results = new List<Color>();

			long lastCount = -1;
			foreach (ColorCount colorCount in colors) {
				long count = colorCount.Count;
				Color color = colorCount.Color;
				float bri = color.GetBrightness();
				float sat = color.GetSaturation();
				float hue = color.GetHue();

				bool uniqueColorFound = true; // want main colors ie dark brown, gold, silver, not 3 shades of brown
				// Make sure color isn't too similar to already-added colors
				foreach (Color colorOther in results) {
					float briOther = colorOther.GetBrightness();
					float satOther = colorOther.GetSaturation();
					float hueOther = colorOther.GetHue();

					// hue is 360 degrees of color, to calculate hue difference                        
					// need to subtract 360 when either are out by 180 (i.e red is at 0 and 359, diff should be 1 etc)
					if (hue - hueOther > 180)
						hue -= 360;
					if (hueOther - hue > 180)
						hueOther -= 360;

					float briDiff = Math.Abs(bri - briOther);
					float satDiff = Math.Abs(sat - satOther);
					float hueDiff = Math.Abs(hue - hueOther);
					int matchHSB = 0;

					if (briDiff <= minBriDiff)
						matchHSB++;
					if (satDiff <= minSatDiff)
						matchHSB++;
					if (hueDiff <= minHueDiff)
						matchHSB++;

					if (satDiff != 1 && (briDiff <= minBriDiff || satDiff <= minSatDiff || hueDiff <= minHueDiff)) {
						uniqueColorFound = false;
						break;
					}
				}
				if (uniqueColorFound) {       // color differs by min ammount of HSL so add to response
					results.Add(color);
					if (results.Count == maxCount)
						break;
				}
				lastCount = count;
			}

			Trace.WriteLine($"Colors Found: {results.Count}/{maxCount}");
			return results;
		}
	}
}
