using TriggersTools.Asciify.ColorMine.Converters;
using TriggersTools.Asciify.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using TriggersTools.Asciify.ColorMine.Comparisons;

namespace TriggersTools.Asciify.Asciifying.Asciifiers {
	internal class DotColorAsciifier : AsciifierBase<ColorLab, ColorLab>, IDotColorAsciifier {

		protected override ColorLab CalcFontData(Color color) {
			return LabConverter.ToLab(color);
		}

		protected override ColorLab CalcFontData(IEnumerable<PixelPoint> pixels) {
			double count = 0;
			ColorLab charValue = new ColorLab();
			double inc = 1;
			foreach (PixelPoint p in pixels) {
				inc = p.Color.A / 255d;
				charValue += LabConverter.ToLab(p.Color) * inc;
				count += inc;
			}
			return (charValue / count).ZeroNaNs;
		}

		protected override ColorLab CalcImageData(IEnumerable<PixelPoint> pixels, Point start) {
			int count = 0;
			ColorLab charValue = new ColorLab();
			foreach (PixelPoint p in pixels) {
				charValue += LabConverter.ToLab(p.Color);
				count++;
			}
			return charValue / count;
		}

		protected override double CalcScore(ColorLab a, ColorLab b) {
			//return Comparer.Compare(a, b, true);
			return Cie76Comparison.CompareS(a, b, true);
		}
	}
}
