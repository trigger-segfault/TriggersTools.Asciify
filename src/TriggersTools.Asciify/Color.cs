using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TriggersTools.Asciify {
	public struct ColorRgb {
		public double R;
		public double G;
		public double B;
		
		public static explicit operator Color(ColorRgb color) =>
			Color.FromArgb(
				Math.Max(0, Math.Min(255, (int) Math.Round(color.R))),
				Math.Max(0, Math.Min(255, (int) Math.Round(color.G))),
				Math.Max(0, Math.Min(255, (int) Math.Round(color.B))));
		
		public static implicit operator ColorRgb(Color color) =>
			new ColorRgb(color.R, color.G, color.B);

		public static ColorRgb operator +(ColorRgb a, ColorRgb b) =>
			new ColorRgb(a.R + b.R, a.G + b.G, a.B + b.B);
		public static ColorRgb operator +(ColorRgb a, double b) =>
			new ColorRgb(a.R + b, a.G + b, a.B + b);
		public static ColorRgb operator +(double a, ColorRgb b) =>
			new ColorRgb(a + b.R, a + b.G, a + b.B);

		public static ColorRgb operator -(ColorRgb a, ColorRgb b) =>
			new ColorRgb(a.R - b.R, a.G - b.G, a.B - b.B);
		public static ColorRgb operator -(ColorRgb a, double b) =>
			new ColorRgb(a.R - b, a.G - b, a.B - b);
		public static ColorRgb operator -(double a, ColorRgb b) =>
			new ColorRgb(a - b.R, a - b.G, a - b.B);

		public static ColorRgb operator *(ColorRgb a, ColorRgb b) =>
			new ColorRgb(a.R * b.R, a.G * b.G, a.B * b.B);
		public static ColorRgb operator *(ColorRgb a, double b) =>
			new ColorRgb(a.R * b, a.G * b, a.B * b);
		public static ColorRgb operator *(double a, ColorRgb b) =>
			new ColorRgb(a * b.R, a * b.G, a * b.B);

		public static ColorRgb operator /(ColorRgb a, ColorRgb b) =>
			new ColorRgb(a.R / b.R, a.G / b.G, a.B / b.B);
		public static ColorRgb operator /(ColorRgb a, double b) =>
			new ColorRgb(a.R / b, a.G / b, a.B / b);
		public static ColorRgb operator /(double a, ColorRgb b) =>
			new ColorRgb(a / b.R, a / b.G, a / b.B);

		public static bool operator ==(ColorRgb a, ColorRgb b) =>
			a.R == b.R && a.G == b.G && a.B == b.B;

		public static bool operator !=(ColorRgb a, ColorRgb b) =>
			a.R != b.R || a.G != b.G || a.B != b.B;

		public ColorRgb(double r, double g, double b) {
			R = r;
			G = g;
			B = b;
		}

		public override bool Equals(object obj) {
			if (obj is ColorRgb colord)
				return this == colord;
			return false;
		}

		public override int GetHashCode() =>
			R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();

		public override string ToString() => $"R={R} G={G} B={B}";
	}

	public struct ColorLab {
		public static readonly ColorLab White = new ColorLab(100d, 0, 0);
		public static readonly ColorLab Black = new ColorLab();

		public double L;
		public double A;
		public double B;
		
		public ColorLab(double l, double a, double b) {
			L = l;
			A = a;
			B = b;
		}

		public override string ToString() => $"L={L} A={A} B={B}";

		public static ColorLab operator +(ColorLab a, ColorLab b) =>
			new ColorLab(a.L + b.L, a.A + b.A, a.B + b.B);
		public static ColorLab operator +(ColorLab a, double b) =>
			new ColorLab(a.L + b, a.A + b, a.B + b);
		public static ColorLab operator +(double a, ColorLab b) =>
			new ColorLab(a + b.L, a + b.A, a + b.B);

		public static ColorLab operator -(ColorLab a, ColorLab b) =>
			new ColorLab(a.L - b.L, a.A - b.A, a.B - b.B);
		public static ColorLab operator -(ColorLab a, double b) =>
			new ColorLab(a.L - b, a.A - b, a.B - b);
		public static ColorLab operator -(double a, ColorLab b) =>
			new ColorLab(a - b.L, a - b.A, a - b.B);

		public static ColorLab operator *(ColorLab a, ColorLab b) =>
			new ColorLab(a.L * b.L, a.A * b.A, a.B * b.B);
		public static ColorLab operator *(ColorLab a, double b) =>
			new ColorLab(a.L * b, a.A * b, a.B * b);
		public static ColorLab operator *(double a, ColorLab b) =>
			new ColorLab(a * b.L, a * b.A, a * b.B);

		public static ColorLab operator /(ColorLab a, ColorLab b) =>
			new ColorLab(a.L / b.L, a.A / b.A, a.B / b.B);
		public static ColorLab operator /(ColorLab a, double b) =>
			new ColorLab(a.L / b, a.A / b, a.B / b);
		public static ColorLab operator /(double a, ColorLab b) =>
			new ColorLab(a / b.L, a / b.A, a / b.B);

		public static ColorLab Max(ColorLab a, ColorLab b) =>
			new ColorLab(
				Math.Max(a.L, b.L),
				Math.Max(a.A, b.A),
				Math.Max(a.B, b.B));

		public static ColorLab Min(ColorLab a, ColorLab b) =>
			new ColorLab(
				Math.Min(a.L, b.L),
				Math.Min(a.A, b.A),
				Math.Min(a.B, b.B));

		public static ColorLab Clamp(ColorLab a) =>
			new ColorLab(
				Math.Max(0, Math.Min(100, a.L)),
				Math.Max(-128, Math.Min(128, a.A)),
				Math.Max(-128, Math.Min(128, a.B)));

		public bool IsAnyZero => L == 0 || A == 0 || B == 0;

		public ColorLab ZeroNaNs =>
			new ColorLab(
				double.IsNaN(L) ? 0 : L,
				double.IsNaN(A) ? 0 : A,
				double.IsNaN(B) ? 0 : B);
	}

	internal struct ColorXyz {
		public double X;
		public double Y;
		public double Z;

		public ColorXyz(double x, double y, double z) {
			X = x;
			Y = y;
			Z = z;
		}

		public override string ToString() => $"X={X} Y={Y} Z={Z}";
	}
}
