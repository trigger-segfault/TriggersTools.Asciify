using System;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.Asciify.ColorMine.Converters {
	internal static class XyzConverter {
		
		public static readonly ColorXyz WhiteReference = new ColorXyz(95.047, 100.000, 108.883);

		public const double Epsilon = 0.008856; // Intent is 216/24389
		public const double Kappa = 903.3; // Intent is 24389/27

		public static ColorXyz ToXyz(ColorRgb rgb) {
			double r = PivotRgb(rgb.R / 255.0);
			double g = PivotRgb(rgb.G / 255.0);
			double b = PivotRgb(rgb.B / 255.0);

			// Observer. = 2°, Illuminant = D65
			return new ColorXyz(
				r * 0.4124 + g * 0.3576 + b * 0.1805,
				r * 0.2126 + g * 0.7152 + b * 0.0722,
				r * 0.0193 + g * 0.1192 + b * 0.9505);
		}

		public static ColorRgb ToColor(ColorXyz xyz) {
			// (Observer = 2°, Illuminant = D65)
			double x = xyz.X / 100.0;
			double y = xyz.Y / 100.0;
			double z = xyz.Z / 100.0;

			double r = x * 3.2406 + y * -1.5372 + z * -0.4986;
			double g = x * -0.9689 + y * 1.8758 + z * 0.0415;
			double b = x * 0.0557 + y * -0.2040 + z * 1.0570;

			r = r > 0.0031308 ? 1.055 * Math.Pow(r, 1 / 2.4) - 0.055 : 12.92 * r;
			g = g > 0.0031308 ? 1.055 * Math.Pow(g, 1 / 2.4) - 0.055 : 12.92 * g;
			b = b > 0.0031308 ? 1.055 * Math.Pow(b, 1 / 2.4) - 0.055 : 12.92 * b;

			return new ColorRgb(ToRgb(r), ToRgb(g), ToRgb(b));
		}

		private static double ToRgb(double n) {
			var result = 255.0 * n;
			if (result < 0)
				return 0;
			if (result > 255)
				return 255;
			return result;
		}

		private static double PivotRgb(double n) {
			return (n > 0.04045 ? Math.Pow((n + 0.055) / 1.055, 2.4) : n / 12.92) * 100.0;
		}

		private static double CubicRoot(double n) {
			return Math.Pow(n, 1.0 / 3.0);
		}

	}
}
