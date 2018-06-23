using TriggersTools.Asciify.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using TriggersTools.Asciify.ColorMine.Converters;

namespace TriggersTools.Asciify.Asciifying.Asciifiers {
	/*public struct SectionedDataCount<T> where T : struct {
		public T Left;
		public T Right;
		public T Top;
		public T Bottom;
		public T Center;
		public T All;
		public int LeftCount;
		public int RightCount;
		public int TopCount;
		public int BottomCount;
		public int CenterCount;
		public int AllCount;
	}
	public struct SectionedData<T> where T : struct {
		public T Left;
		public T Right;
		public T Top;
		public T Bottom;
		public T Center;
		public T All;
	}*/

	/*internal struct SectionedInt {
		public int Left;
		public int Right;
		public int Top;
		public int Bottom;
		public int Center;
		public int All;
	}*/
	internal struct SectionedDouble {
		public double Left;
		public double Right;
		public double Top;
		public double Bottom;
		public double Center;
		public double All;
		
		public SectionedDouble(double uniform) {
			Left = uniform;
			Right = uniform;
			Top = uniform;
			Bottom = uniform;
			Center = uniform;
			All = uniform;
		}

		public SectionedDouble(double l, double r, double t, double b, double c, double a) {
			Left = l;
			Right = r;
			Top = t;
			Bottom = b;
			Center = c;
			All = a;
		}

		/*public static implicit operator SectionedDouble(SectionedInt i) {
			return new SectionedDouble(i.Left, i.Right, i.Top, i.Bottom, i.Center, i.All);
		}*/

		public static SectionedDouble operator +(SectionedDouble a, SectionedDouble b) =>
			new SectionedDouble(
				a.Left + b.Left, a.Right + b.Right,
				a.Top + b.Top, a.Bottom + b.Bottom,
				a.Center + b.Center, a.All + b.All);
		public static SectionedDouble operator +(SectionedDouble a, double b) =>
			new SectionedDouble(
				a.Left + b, a.Right + b,
				a.Top + b, a.Bottom + b,
				a.Center + b, a.All + b);
		public static SectionedDouble operator +(double a, SectionedDouble b) =>
			new SectionedDouble(
				a + b.Left, a + b.Right,
				a + b.Top, a + b.Bottom,
				a + b.Center, a + b.All);

		public static SectionedDouble operator -(SectionedDouble a, SectionedDouble b) =>
			new SectionedDouble(
				a.Left - b.Left, a.Right - b.Right,
				a.Top - b.Top, a.Bottom - b.Bottom,
				a.Center - b.Center, a.All - b.All);
		public static SectionedDouble operator -(SectionedDouble a, double b) =>
			new SectionedDouble(
				a.Left - b, a.Right - b,
				a.Top - b, a.Bottom - b,
				a.Center - b, a.All - b);
		public static SectionedDouble operator -(double a, SectionedDouble b) =>
			new SectionedDouble(
				a - b.Left, a - b.Right,
				a - b.Top, a - b.Bottom,
				a - b.Center, a - b.All);

		public static SectionedDouble operator *(SectionedDouble a, SectionedDouble b) =>
			new SectionedDouble(
				a.Left * b.Left, a.Right * b.Right,
				a.Top * b.Top, a.Bottom * b.Bottom,
				a.Center * b.Center, a.All * b.All);
		public static SectionedDouble operator *(SectionedDouble a, double b) =>
			new SectionedDouble(
				a.Left * b, a.Right * b,
				a.Top * b, a.Bottom * b,
				a.Center * b, a.All * b);
		public static SectionedDouble operator *(double a, SectionedDouble b) =>
			new SectionedDouble(
				a * b.Left, a * b.Right,
				a * b.Top, a * b.Bottom,
				a * b.Center, a * b.All);

		public static SectionedDouble operator /(SectionedDouble a, SectionedDouble b) =>
			new SectionedDouble(
				a.Left / b.Left, a.Right / b.Right,
				a.Top / b.Top, a.Bottom / b.Bottom,
				a.Center / b.Center, a.All / b.All);
		public static SectionedDouble operator /(SectionedDouble a, double b) =>
			new SectionedDouble(
				a.Left / b, a.Right / b,
				a.Top / b, a.Bottom / b,
				a.Center / b, a.All / b);
		public static SectionedDouble operator /(double a, SectionedDouble b) =>
			new SectionedDouble(
				a / b.Left, a / b.Right,
				a / b.Top, a / b.Bottom,
				a / b.Center, a / b.All);

		public static SectionedDouble Max(SectionedDouble a, SectionedDouble b) =>
			new SectionedDouble(
				Math.Max(a.Left, b.Left), Math.Max(a.Right, b.Right),
				Math.Max(a.Top, b.Top), Math.Max(a.Bottom, b.Bottom),
				Math.Max(a.Center, b.Center), Math.Max(a.All, b.All));

		public static SectionedDouble Min(SectionedDouble a, SectionedDouble b) =>
			new SectionedDouble(
				Math.Min(a.Left, b.Left), Math.Min(a.Right, b.Right),
				Math.Min(a.Top, b.Top), Math.Min(a.Bottom, b.Bottom),
				Math.Min(a.Center, b.Center), Math.Min(a.All, b.All));

		public static SectionedDouble Abs(SectionedDouble a, SectionedDouble b) =>
			new SectionedDouble(
				Math.Abs(a.Left - b.Left), Math.Abs(a.Right - b.Right),
				Math.Abs(a.Top - b.Top), Math.Abs(a.Bottom - b.Bottom),
				Math.Abs(a.Center - b.Center), Math.Abs(a.All - b.All));

		public SectionedDouble ZeroNaNs =>
			new SectionedDouble(
				double.IsNaN(Left) ? 0 : Left, double.IsNaN(Right) ? 0 : Right,
				double.IsNaN(Top) ? 0 : Top, double.IsNaN(Bottom) ? 0 : Bottom,
				double.IsNaN(Center) ? 0 : Center, double.IsNaN(All) ? 0 : All);

		public double Total =>
			Left + Right + Top + Bottom + Center + All;
	}
	internal abstract class SectionedBaseAsciifier<TFntData, TImgData> : AsciifierBase<TFntData, TImgData>, ISectionedAsciifier {
		protected int left;
		protected int right;
		protected int top;
		protected int bottom;

		protected SectionedDouble fontCounts;

		private double allFactor = 0.0;
		public double AllFactor {
			get => allFactor;
			set {
				if (value < 0)
					throw new ArgumentException("AllFactor must be greater than or equal to zero!");
				allFactor = value;
			}
		}

		protected override void PreInitialize() {
			left = Font.Width / 4;
			top = Font.Height / 4;
			right = Font.Width - left;
			bottom = Font.Height - top;
			int hsideCount = Font.Height * left;
			int vsideCount = Font.Width * top;
			int centerCount = (right - left) * (bottom - top);
			int allCount = Font.Width * Font.Height;
			fontCounts = new SectionedDouble(
				hsideCount, hsideCount,
				vsideCount, vsideCount,
				centerCount, allCount);
		}
	}
}
