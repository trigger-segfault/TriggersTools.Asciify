using TriggersTools.Asciify.ColorMine.Converters;
using TriggersTools.Asciify.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using TriggersTools.Asciify.ColorMine.Comparisons;

namespace TriggersTools.Asciify.Asciifying.Asciifiers {
	/*internal struct SectionedColorI {
		public ColorI Left;
		public ColorI Right;
		public ColorI Top;
		public ColorI Bottom;
		public ColorI Center;
		public ColorI All;
	}*/
	internal struct SectionedLab {
		public ColorLab Left;
		public ColorLab Right;
		public ColorLab Top;
		public ColorLab Bottom;
		public ColorLab Center;
		public ColorLab All;

		public SectionedLab(ColorLab uniform) {
			Left = uniform;
			Right = uniform;
			Top = uniform;
			Bottom = uniform;
			Center = uniform;
			All = uniform;
		}

		public SectionedLab(ColorLab l, ColorLab r, ColorLab t, ColorLab b, ColorLab c, ColorLab a) {
			Left = l;
			Right = r;
			Top = t;
			Bottom = b;
			Center = c;
			All = a;
		}

		public static SectionedLab operator +(SectionedLab a, SectionedLab b) =>
			new SectionedLab(
				a.Left + b.Left, a.Right + b.Right,
				a.Top + b.Top, a.Bottom + b.Bottom,
				a.Center + b.Center, a.All + b.All);
		public static SectionedLab operator +(SectionedLab a, SectionedDouble b) =>
			new SectionedLab(
				a.Left + b.Left, a.Right + b.Right,
				a.Top + b.Top, a.Bottom + b.Bottom,
				a.Center + b.Center, a.All + b.All);
		public static SectionedLab operator +(SectionedLab a, ColorLab b) =>
			new SectionedLab(
				a.Left + b, a.Right + b,
				a.Top + b, a.Bottom + b,
				a.Center + b, a.All + b);
		public static SectionedLab operator +(SectionedLab a, double b) =>
			new SectionedLab(
				a.Left + b, a.Right + b,
				a.Top + b, a.Bottom + b,
				a.Center + b, a.All + b);
		public static SectionedLab operator +(SectionedDouble a, SectionedLab b) =>
			new SectionedLab(
				a.Left + b.Left, a.Right + b.Right,
				a.Top + b.Top, a.Bottom + b.Bottom,
				a.Center + b.Center, a.All + b.All);
		public static SectionedLab operator +(ColorLab a, SectionedLab b) =>
			new SectionedLab(
				a + b.Left, a + b.Right,
				a + b.Top, a + b.Bottom,
				a + b.Center, a + b.All);
		public static SectionedLab operator +(double a, SectionedLab b) =>
			new SectionedLab(
				a + b.Left, a + b.Right,
				a + b.Top, a + b.Bottom,
				a + b.Center, a + b.All);

		public static SectionedLab operator -(SectionedLab a, SectionedLab b) =>
			new SectionedLab(
				a.Left - b.Left, a.Right - b.Right,
				a.Top - b.Top, a.Bottom - b.Bottom,
				a.Center - b.Center, a.All - b.All);
		public static SectionedLab operator -(SectionedLab a, SectionedDouble b) =>
			new SectionedLab(
				a.Left - b.Left, a.Right - b.Right,
				a.Top - b.Top, a.Bottom - b.Bottom,
				a.Center - b.Center, a.All - b.All);
		public static SectionedLab operator -(SectionedLab a, ColorLab b) =>
			new SectionedLab(
				a.Left - b, a.Right - b,
				a.Top - b, a.Bottom - b,
				a.Center - b, a.All - b);
		public static SectionedLab operator -(SectionedLab a, double b) =>
			new SectionedLab(
				a.Left - b, a.Right - b,
				a.Top - b, a.Bottom - b,
				a.Center - b, a.All - b);
		public static SectionedLab operator -(SectionedDouble a, SectionedLab b) =>
			new SectionedLab(
				a.Left - b.Left, a.Right - b.Right,
				a.Top - b.Top, a.Bottom - b.Bottom,
				a.Center - b.Center, a.All - b.All);
		public static SectionedLab operator -(ColorLab a, SectionedLab b) =>
			new SectionedLab(
				a - b.Left, a - b.Right,
				a - b.Top, a - b.Bottom,
				a - b.Center, a - b.All);
		public static SectionedLab operator -(double a, SectionedLab b) =>
			new SectionedLab(
				a - b.Left, a - b.Right,
				a - b.Top, a - b.Bottom,
				a - b.Center, a - b.All);

		public static SectionedLab operator *(SectionedLab a, SectionedLab b) =>
			new SectionedLab(
				a.Left * b.Left, a.Right * b.Right,
				a.Top * b.Top, a.Bottom * b.Bottom,
				a.Center * b.Center, a.All * b.All);
		public static SectionedLab operator *(SectionedLab a, SectionedDouble b) =>
			new SectionedLab(
				a.Left * b.Left, a.Right * b.Right,
				a.Top * b.Top, a.Bottom * b.Bottom,
				a.Center * b.Center, a.All * b.All);
		public static SectionedLab operator *(SectionedLab a, ColorLab b) =>
			new SectionedLab(
				a.Left * b, a.Right * b,
				a.Top * b, a.Bottom * b,
				a.Center * b, a.All * b);
		public static SectionedLab operator *(SectionedLab a, double b) =>
			new SectionedLab(
				a.Left * b, a.Right * b,
				a.Top * b, a.Bottom * b,
				a.Center * b, a.All * b);
		public static SectionedLab operator *(SectionedDouble a, SectionedLab b) =>
			new SectionedLab(
				a.Left * b.Left, a.Right * b.Right,
				a.Top * b.Top, a.Bottom * b.Bottom,
				a.Center * b.Center, a.All * b.All);
		public static SectionedLab operator *(ColorLab a, SectionedLab b) =>
			new SectionedLab(
				a * b.Left, a * b.Right,
				a * b.Top, a * b.Bottom,
				a * b.Center, a * b.All);
		public static SectionedLab operator *(double a, SectionedLab b) =>
			new SectionedLab(
				a * b.Left, a * b.Right,
				a * b.Top, a * b.Bottom,
				a * b.Center, a * b.All);

		public static SectionedLab operator /(SectionedLab a, SectionedLab b) =>
			new SectionedLab(
				a.Left / b.Left, a.Right / b.Right,
				a.Top / b.Top, a.Bottom / b.Bottom,
				a.Center / b.Center, a.All / b.All);
		public static SectionedLab operator /(SectionedLab a, SectionedDouble b) =>
			new SectionedLab(
				a.Left / b.Left, a.Right / b.Right,
				a.Top / b.Top, a.Bottom / b.Bottom,
				a.Center / b.Center, a.All / b.All);
		public static SectionedLab operator /(SectionedLab a, ColorLab b) =>
			new SectionedLab(
				a.Left / b, a.Right / b,
				a.Top / b, a.Bottom / b,
				a.Center / b, a.All / b);
		public static SectionedLab operator /(SectionedLab a, double b) =>
			new SectionedLab(
				a.Left / b, a.Right / b,
				a.Top / b, a.Bottom / b,
				a.Center / b, a.All / b);
		public static SectionedLab operator /(SectionedDouble a, SectionedLab b) =>
			new SectionedLab(
				a.Left / b.Left, a.Right / b.Right,
				a.Top / b.Top, a.Bottom / b.Bottom,
				a.Center / b.Center, a.All / b.All);
		public static SectionedLab operator /(ColorLab a, SectionedLab b) =>
			new SectionedLab(
				a / b.Left, a / b.Right,
				a / b.Top, a / b.Bottom,
				a / b.Center, a / b.All);
		public static SectionedLab operator /(double a, SectionedLab b) =>
			new SectionedLab(
				a / b.Left, a / b.Right,
				a / b.Top, a / b.Bottom,
				a / b.Center, a / b.All);


		public static SectionedLab Max(SectionedLab a, SectionedLab b) =>
			new SectionedLab(
				ColorLab.Max(a.Left, b.Left),
				ColorLab.Max(a.Right, b.Right),
				ColorLab.Max(a.Top, b.Top),
				ColorLab.Max(a.Bottom, b.Bottom),
				ColorLab.Max(a.Center, b.Center),
				ColorLab.Max(a.All, b.All));

		public static SectionedLab Min(SectionedLab a, SectionedLab b) =>
			new SectionedLab(
				ColorLab.Min(a.Left, b.Left),
				ColorLab.Min(a.Right, b.Right),
				ColorLab.Min(a.Top, b.Top),
				ColorLab.Min(a.Bottom, b.Bottom),
				ColorLab.Min(a.Center, b.Center),
				ColorLab.Min(a.All, b.All));

		public SectionedLab ZeroNaNs =>
			new SectionedLab(
				Left.ZeroNaNs,
				Right.ZeroNaNs,
				Top.ZeroNaNs,
				Bottom.ZeroNaNs,
				Center.ZeroNaNs,
				All.ZeroNaNs);
	}
	internal struct SectionedLabCounts {

		/*public ColorLab Left;
		public ColorLab Right;
		public ColorLab Top;
		public ColorLab Bottom;
		public ColorLab Center;
		public ColorLab All;
		public int LeftCount;
		public int RightCount;
		public int TopCount;
		public int BottomCount;
		public int CenterCount;
		public int AllCount;*/

		public SectionedLab Lab;
		public SectionedDouble Counts;

		public SectionedLabCounts(SectionedLab area, SectionedDouble counts) {
			Lab = (area / counts).ZeroNaNs;
			Counts = counts;
		}
	}
	internal class SectionedColorAsciifier : SectionedBaseAsciifier<SectionedLab, SectionedLabCounts>, ISectionedColorAsciifier {

		protected override SectionedLab CalcFontData(Color color) {
			return new SectionedLab(LabConverter.ToLab(color));
		}

		protected override SectionedLab CalcFontData(IEnumerable<PixelPoint> pixels) {
			SectionedDouble counts = new SectionedDouble();
			SectionedLab area = new SectionedLab();
			double inc = 1;
			foreach (PixelPoint p in pixels) {
				inc = p.Color.A / 255d;
				ColorLab color = LabConverter.ToLab(p.Color) * inc;
				if (p.X < left) {
					area.Left += color;
					counts.Left += inc;
					//leftCount++;
				}
				else if (p.X >= right) {
					area.Right += color;
					counts.Right += inc;
					//rightCount++;
				}
				if (p.Y < top) {
					area.Top += color;
					counts.Top += inc;
					//topCount++;
				}
				else if (p.Y >= bottom) {
					area.Bottom += color;
					counts.Bottom += inc;
					//bottomCount++;
				}
				if (p.X >= left && p.X < right &&
					p.Y >= top && p.Y < bottom) {
					area.Center += color;
					counts.Center += inc;
					//centerCount++;
				}
				area.All += color;
				counts.All += inc;
				//allCount++;
			}
			return (area / counts).ZeroNaNs;
		}

		protected override SectionedLabCounts CalcImageData(IEnumerable<PixelPoint> pixels, Point start) {
			/*int leftCount = 0;
			int rightCount = 0;
			int topCount = 0;
			int bottomCount = 0;
			int centerCount = 0;
			int allCount = 0;*/
			SectionedDouble counts = new SectionedDouble();
			SectionedLab area = new SectionedLab();
			foreach (PixelPoint p in pixels) {
				ColorLab color = LabConverter.ToLab(p.Color);
				if (p.X < left) {
					area.Left += color;
					counts.Left++;
					//leftCount++;
				}
				else if (p.X >= right) {
					area.Right += color;
					counts.Right++;
					//rightCount++;
				}
				if (p.Y < top) {
					area.Top += color;
					counts.Top++;
					//topCount++;
				}
				else if (p.Y >= bottom) {
					area.Bottom += color;
					counts.Bottom++;
					//bottomCount++;
				}
				if (p.X >= left && p.X < right &&
					p.Y >= top && p.Y < bottom) {
					area.Center += color;
					counts.Center++;
					//centerCount++;
				}
				area.All += color;
				counts.All++;
				//allCount++;
			}
			return new SectionedLabCounts(area, counts);
		}

		protected override double CalcScore(SectionedLabCounts a, SectionedLab b) {
			/*double left = Comparer.Compare(a.Lab.Left, TransformLab(b.Lab.Left)) * a.Counts.Left;
			double right = Comparer.Compare(a.Lab.Right, TransformLab(b.Lab.Right)) * a.Counts.Right;
			double top = Comparer.Compare(a.Lab.Top, TransformLab(b.Lab.Top)) * a.Counts.Top;
			double bottom = Comparer.Compare(a.Lab.Bottom, TransformLab(b.Lab.Bottom)) * a.Counts.Bottom;
			double center = Comparer.Compare(a.Lab.Center, TransformLab(b.Lab.Center)) * a.Counts.Center;
			double all = Comparer.Compare(a.Lab.All, TransformLab(b.Lab.All)) * a.Counts.All;
			return left + right + top + bottom + center + all * AllFactor;*/
			/*double left = Comparer.Compare(a.Lab.Left, b.Lab.Left) * a.Counts.Left;
			double right = Comparer.Compare(a.Lab.Right, b.Lab.Right) * a.Counts.Right;
			double top = Comparer.Compare(a.Lab.Top, b.Lab.Top) * a.Counts.Top;
			double bottom = Comparer.Compare(a.Lab.Bottom, b.Lab.Bottom) * a.Counts.Bottom;
			double center = Comparer.Compare(a.Lab.Center, b.Lab.Center) * a.Counts.Center;
			double all = Comparer.Compare(a.Lab.All, b.Lab.All) * a.Counts.All;
			return left + right + top + bottom + center + all * AllFactor;*/
			double left = Cie76Comparison.CompareS(a.Lab.Left, b.Left) * a.Counts.Left;
			double right = Cie76Comparison.CompareS(a.Lab.Right, b.Right) * a.Counts.Right;
			double top = Cie76Comparison.CompareS(a.Lab.Top, b.Top) * a.Counts.Top;
			double bottom = Cie76Comparison.CompareS(a.Lab.Bottom, b.Bottom) * a.Counts.Bottom;
			double center = Cie76Comparison.CompareS(a.Lab.Center, b.Center) * a.Counts.Center;
			double all = Cie76Comparison.CompareS(a.Lab.All, b.All) * a.Counts.All;
			return left + right + top + bottom + center + all * AllFactor;
		}
	}
}
