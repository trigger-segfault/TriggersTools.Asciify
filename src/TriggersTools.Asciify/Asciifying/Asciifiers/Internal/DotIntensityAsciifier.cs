using TriggersTools.Asciify.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using TriggersTools.Asciify.ColorMine.Converters;

namespace TriggersTools.Asciify.Asciifying.Asciifiers {
	internal class DotIntensityAsciifier : AsciifierBase<double, double>, IDotIntensityAsciifier {
		public bool ReverseIntensity { get; set; }

		protected override void PostInitialize() {
			base.PostInitialize();
		}

		protected override double CalcFontData(Color color) {
			return LabConverter.ToLab(color).L / 100;
		}

		protected override double CalcFontData(IEnumerable<PixelPoint> pixels) {
			double intensityTotal = 0;
			double count = 0;
			foreach (PixelPoint p in pixels) {
				intensityTotal += LabConverter.ToLab(p.Color).L;
				count++;
			}
			return intensityTotal / (count * 100);
		}

		protected override double CalcImageData(IEnumerable<PixelPoint> pixels, Point start) {
			double intensityTotal = 0;
			double count = 0;
			foreach (PixelPoint p in pixels) {
				intensityTotal += LabConverter.ToLab(p.Color).L;
				count++;
			}
			return intensityTotal / (count * 100);
		}

		protected override double CalcScore(double a, double b) {
			return Math.Abs(a - b);
		}

		protected override void PrepareImage(Graphics g, Image image, Size scaled, Color transparent, ImageAttributes attributes) {
			// https://stackoverflow.com/a/2265990/7517185
			//create the grayscale ColorMatrix
			ColorMatrix colorMatrix = new ColorMatrix(
				new float[][] {
					new float[] {.3f, .3f, .3f, 0, 0},
					new float[] {.59f, .59f, .59f, 0, 0},
					new float[] {.11f, .11f, .11f, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {0, 0, 0, 0, 1}
				});

			//set the color matrix attribute
			attributes.SetColorMatrix(MultiplyMatrix(ColorMatrix, colorMatrix));
			
			DrawImage(g, image, scaled, attributes);
		}
	}
}
