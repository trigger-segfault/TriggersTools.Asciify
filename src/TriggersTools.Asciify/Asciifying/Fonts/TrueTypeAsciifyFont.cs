using TriggersTools.Asciify.Asciifying;
using TriggersTools.Asciify.Asciifying.Palettes;
using TriggersTools.Asciify.ColorMine.Comparisons;
using TriggersTools.Asciify.ColorMine.Converters;
using TriggersTools.Asciify.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;

namespace TriggersTools.Asciify.Asciifying.Fonts {
	public class TrueTypeAsciifyFont : IAsciifyFont {

		private static readonly StringFormat StringFormat = new StringFormat {
			FormatFlags = StringFormatFlags.FitBlackBox,
		};

		private Dictionary<char, float> charWidths;

		public ICharacterSet CharacterSet { get; private set; }
		public IEnumerable<char> OutCharRange => charWidths.Keys;
		public Size Size { get; private set; }
		public int Width => Size.Width;
		public int Height => Size.Height;
		public int Magnitude => Size.Width * Size.Height;
		public Size Offset { get; private set; }
		//public double MaxIntensity { get; private set; }

		public string Family { get; private set; }
		public float FontSize { get; private set; }
		public FontStyle Style { get; private set; }
		public char WidestChar { get; private set; }

		public ReadOnlyDictionary<char, float> CharWidths =>
			new ReadOnlyDictionary<char, float>(charWidths);

		public Font Font { get; private set; }

		public TrueTypeAsciifyFont(string name, float size, FontStyle style, ICharacterSet range = null, bool removeNonMonospace = false) {
			if (range == null)
				range = CharacterSets.Default;
			Family = name;
			Style = style;
			FontSize = size;
			CharacterSet = range;

			CalculateWidestSize(removeNonMonospace);
			CalculateOffset();
			//CalculateMaxIntensity();
		}

		public TrueTypeAsciifyFont(string name, Size size, FontStyle style, ICharacterSet range = null, bool removeNonMonospace = false) {
			if (range == null)
				range = CharacterSets.Default;
			Family = name;
			Style = style;
			Size = size;
			CharacterSet = range;

			CalculateFontSize(removeNonMonospace);
			CalculateOffset();
			//CalculateMaxIntensity();
		}

		/*private Cie76Comparison comparer = new Cie76Comparison();

		public double CalculateIntensity(BitmapData data, Color background) {
			int count = 0;
			ColorLab charValue = ColorLab.Black;
			foreach (PixelPoint p in data.ForEachPixel()) {
				charValue += LabConverter.ToLab(p.Color);
				count += p.Color.RgbTotal();
			}
			//double intensity = (double) count / total;
			return comparer.Compare(charValue / Magnitude, LabConverter.ToLab(background)) / 100.0;
		}*/

		/*public double[] CalculateMaxIntensity(AsciifyPalette palette) {
			double[] maxIntensities = new double[palette.CountF];
			if (palette.CountB == 1) {
				// Max Intensity can only be used with a single background color
				for (int i = 0; i < palette.CountF; i++)
					maxIntensities[i] = 1d;
				return maxIntensities;
			}
			int total = Width * Height * 3 * 255;
			//double maxScore = comparer.Compare(LabConverter.BlackReference, LabConverter.WhiteReference);
			//Trace.WriteLine($"MaxScore: {maxScore}");
			using (Font = CreateFont())
			using (PaletteBrushes brushes = palette.CreateBrushes())
			using (Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb)) {
				foreach (PaletteColor color in palette) {
					char maxChar = '\0';
					foreach (char c in OutCharRange) {
						using (Graphics g = Graphics.FromImage(bitmap)) {
							g.Clear(Color.Black);
							DrawChar(g, c, (SolidBrush) Brushes.White, (SolidBrush) Brushes.Black, Point.Empty);
						}
						using (DisposableBitmapData bmpData = bitmap.LockRead()) {
							BitmapData data = bmpData;
							int count = 0;
							ColorLab charValue = new ColorLab();
							foreach (PixelPoint p in data.ForEachPixel()) {
								charValue += LabConverter.ToLab(p.Color);
								count += p.Color.RgbTotal();
							}
							//double intensity = (double) count / total;
							double intensity = comparer.Compare(charValue / Magnitude, LabConverter.BlackReference) / 100.0;
							if (intensity > MaxIntensity) {
								MaxIntensity = intensity;
								maxChar = c;
							}
						}
					}
				}
			}
			Font = null;
		}*/

		public void Setup(AsciifyPalette palette) {
			Font = CreateFont();
		}

		public void Cleanup() {
			Font?.Dispose();
			Font = null;
		}
		
		public void DrawChar(Graphics g, char c, SolidBrush fore, SolidBrush back, Point point) {
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			g.FillRectangle(back, new Rectangle(point, Size));
			/*switch (c) {
			case '░':
				break;
			case '▒':
				break;
			case '▓':
				break;
			case '█':
				break;
			case '▄':
				break;
			case '▀':
				break;
			case '▌':
				break;
			case '▐':
				break;
			default:
				break;
			}*/
			
			g.DrawString(c.ToString(), Font, fore, (PointF) point - Offset, StringFormat);
		}

		private Font CreateFont() =>
			new Font(Family, FontSize, Style, GraphicsUnit.Point);

		private void CalculateWidestSize(bool removeNonMonospace) {
			using (Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format24bppRgb))
			using (Graphics g = Graphics.FromImage(bitmap))
			using (Font font = CreateFont()) {
				charWidths = new Dictionary<char, float>();
				WidestChar = 'A';
				float widestWidth = MeasureString("A", font).Width;
				foreach (char w in CharacterSet.Characters) {
					float width = g.MeasureString($"{w}", font).Width;
					if (width > widestWidth) {
						if (!removeNonMonospace) {
							charWidths[w] = width;
							WidestChar = w;
							widestWidth = width;
						}
					}
					else  {
						charWidths[w] = width;
					}
				}
				string c = new string(WidestChar, 1);
				// Find out what the current size of the string in this font is
				SizeF measured = g.MeasureString($"{c}", font);
				SizeF measured2 = new SizeF {
					Width = g.MeasureString($"{c}{c}", font).Width,
					Height = g.MeasureString($"{c}\n{c}", font).Height,
				};
				Size = new Size {
					Width = ((int) Math.Ceiling(measured2.Width - measured.Width)),
					Height = (int) Math.Ceiling(measured.Height),//((int) MathF.Floor(measured2.Height - measured.Height)),
				};
				/*Size = new Size {
					Width = (int) MathF.Round(measured.Width),
					Height = (int) MathF.Round(measured.Height),
				};*/
			}
		}
		private void CalculateFontSize(bool removeNonMonospace) {
			FontSize = 100;
			CalculateWidestSize(removeNonMonospace);
			using (Bitmap bitmap = new Bitmap(Size.Width, Size.Height, PixelFormat.Format24bppRgb))
			using (Graphics g = Graphics.FromImage(bitmap))
			using (Font font = CreateFont()) {
				/*WidestChar = '\0';
				float widestWidth = 0f;
				foreach (char w in CharRanges) {
					float width = g.MeasureString($"{w}", font).Width;
					if (width > widestWidth) {
						WidestChar = w;
						widestWidth = width;
					}
				}*/
				string c = new string(WidestChar, 1);
				// Find out what the current size of the string in this font is
				SizeF measured = g.MeasureString($"{c}", font);
				SizeF measured2 = new SizeF {
					Width = g.MeasureString($"{c}{c}", font).Width,
					Height = g.MeasureString($"{c}\n{c}", font).Height,
				};
				Size = new Size {
					Width = ((int) Math.Round(measured2.Width - measured.Width)),
					Height = ((int) Math.Round(measured2.Height - measured.Height)),
				};
				Size = new Size {
					Width = (int) Math.Round(measured.Width),
					Height = (int) Math.Round(measured.Height),
				};

				// Either width or height is too big...
				// Usually either the height ratio or the width ratio
				// will be less than 1. Work them out...
				float heightScaleRatio = Size.Height / measured.Height;
				float widthScaleRatio = Size.Width / measured.Width;

				// We'll scale the font by the one which is furthest out of range...
				float scaleRatio = Math.Min(heightScaleRatio, widthScaleRatio);
				FontSize = font.Size * scaleRatio;
			}
		}

		private void CalculateOffset() {
			using (Font font = CreateFont())
			using (Bitmap bitmap = CreateOffsetTestBitmap(font))
			using (DisposableBitmapData bmpData = bitmap.LockRead()) {
				BitmapData data = bmpData;

				bool offsetFound = false;
				for (int x = 0; x <= bitmap.Width; x++) {
					bool match = x != bitmap.Width && !data.MatchesColor(x, bitmap.Height / 2, Color.White);
					if (match) {
						Offset = new Size(x, Offset.Height);
						break;
					}
					/*if (!offsetFound && match) {
						Offset = new Size(x, Offset.Height);
						offsetFound = true;
						break;
					}
					else if (offsetFound && !match) {
						Size = new Size(x - Offset.Width, Size.Height);
					}*/
				}

				offsetFound = false;
				for (int y = 0; y <= bitmap.Height; y++) {
					bool match = y != bitmap.Height && !data.MatchesColor(bitmap.Width / 2, y, Color.White);
					if (!offsetFound && match) {
						Offset = new Size(Offset.Width, y);
						offsetFound = true;
					}
					else if (offsetFound && !match) {
						Size = new Size(Size.Width, y - Offset.Height);
						break;
					}
				}

				Dictionary<char, float> newCharWidths = new Dictionary<char, float>();
				foreach (var pair in charWidths) {
					newCharWidths.Add(pair.Key, pair.Value - Offset.Width);
				}
				charWidths = newCharWidths;
			}
		}

		private Bitmap CreateOffsetTestBitmap(Font font) {
			SizeF sizef = MeasureString("█", font);
			int width = (int) Math.Ceiling(sizef.Width);
			int height = (int) Math.Ceiling(sizef.Height);
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
			try {
				using (Graphics g = Graphics.FromImage(bitmap)) {
					//g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
					g.Clear(Color.White);
					g.DrawString("█", font, Brushes.Black, Point.Empty);
					return bitmap;
				}
			}
			catch {
				bitmap.Dispose();
				throw;
			}
		}

		private SizeF MeasureString(string text, Font font) {
			using (Bitmap bitmap = new Bitmap(1, 1))
			using (Graphics g = Graphics.FromImage(bitmap)) {
				g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				return g.MeasureString(text, font);
			}
		}

		public override string ToString() {
			return $"{Family} {FontSize}pt {Style}";
		}
	}
}
