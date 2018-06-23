using TriggersTools.Asciify.ColorMine.Converters;
using TriggersTools.Asciify.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Numerics;

namespace TriggersTools.Asciify.Asciifying.Asciifiers {
	internal struct SectionedDoubleCounts {
		
		public SectionedDouble Double;
		public SectionedDouble Counts;
		
		public SectionedDoubleCounts(SectionedDouble area, SectionedDouble counts) {
			Double = (area / (counts * 100)).ZeroNaNs;
			Counts = counts;
		}
	}
	internal class SectionedIntensityAsciifier : SectionedBaseAsciifier<SectionedDouble, SectionedDoubleCounts>, ISectionedIntensityAsciifier {
		
		protected override SectionedDouble CalcFontData(Color color) {
			return new SectionedDouble(LabConverter.ToLab(color).L / 100);
		}
		
		protected override SectionedDouble CalcFontData(IEnumerable<PixelPoint> pixels) {
			SectionedDouble area = new SectionedDouble();
			foreach (PixelPoint p in pixels) {
				double color = LabConverter.ToLab(p.Color).L;
				if (p.X < left) {
					area.Left += color;
				}
				else if (p.X >= right) {
					area.Right += color;
				}
				if (p.Y < top) {
					area.Top += color;
				}
				else if (p.Y >= bottom) {
					area.Bottom += color;
				}
				if (p.X >= left && p.X < right &&
					p.Y >= top && p.Y < bottom) {
					area.Center += color;
				}
				area.All += color;
			}
			return (area / (fontCounts * 100)).ZeroNaNs;
		}

		protected override SectionedDoubleCounts CalcImageData(IEnumerable<PixelPoint> pixels, Point start) {
			SectionedDouble area = new SectionedDouble();
			//SectionedInt area = new SectionedInt();
			SectionedDouble counts = new SectionedDouble();
			//double inc = 1;
			foreach (PixelPoint p in pixels) {
				double color = LabConverter.ToLab(p.Color).L;
				if (p.X < left) {
					area.Left += color;
					counts.Left++;
				}
				else if (p.X >= right) {
					area.Right += color;
					counts.Right++;
				}
				if (p.Y < top) {
					area.Top += color;
					counts.Top++;
				}
				else if (p.Y >= bottom) {
					area.Bottom += color;
					counts.Bottom++;
				}
				if (p.X >= left && p.X < right &&
					p.Y >= top && p.Y < bottom) {
					area.Center += color;
					counts.Center++;
				}
				area.All += color;
				counts.All++;
			}
			return new SectionedDoubleCounts(area, counts);
		}

		protected override double CalcScore(SectionedDoubleCounts a, SectionedDouble b) {
			SectionedDouble x = SectionedDouble.Abs(a.Double, b) * a.Counts;
			x.All *= AllFactor;
			return x.Total;
			/*double left = Math.Abs(a.Double.Left - b.Double.Left) * a.Counts.Left;
			double right = Math.Abs(a.Double.Right - b.Double.Right) * a.Counts.Right;
			double top = Math.Abs(a.Double.Top - b.Double.Top) * a.Counts.Top;
			double bottom = Math.Abs(a.Double.Bottom - b.Double.Bottom) * a.Counts.Bottom;
			double center = Math.Abs(a.Double.Center - b.Double.Center) * a.Counts.Center;
			double all = Math.Abs(a.Double.All - b.Double.All) * a.Counts.All;
			return left + right + top + bottom + center + all * AllFactor;*/
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
