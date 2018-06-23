using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TriggersTools.Asciify.ColorMine.Converters {
	public static class LabConverter {
		public static readonly ColorLab WhiteReference = ToLab(new ColorRgb(255d, 255d, 255d));
		public static readonly ColorLab BlackReference = ToLab(new ColorRgb(0d, 0d, 0d));

		public static ColorLab ToLab(ColorRgb rgb) {
			ColorXyz xyz = XyzConverter.ToXyz(rgb);

			ColorXyz white = XyzConverter.WhiteReference;
			double x = PivotXyz(xyz.X / white.X);
			double y = PivotXyz(xyz.Y / white.Y);
			double z = PivotXyz(xyz.Z / white.Z);

			return new ColorLab(
				Math.Max(0, 116 * y - 16),
				500 * (x - y),
				200 * (y - z));
		}

		public static ColorRgb ToColor(ColorLab lab) {
			double y = (lab.L + 16.0) / 116.0;
			double x = lab.A / 500.0 + y;
			double z = y - lab.B / 200.0;

			ColorXyz white = XyzConverter.WhiteReference;
			double x3 = x * x * x;
			double z3 = z * z * z;
			ColorXyz xyz = new ColorXyz(
				white.X * (x3 > XyzConverter.Epsilon ? x3 : (x - 16.0 / 116.0) / 7.787),
				white.Y * (lab.L > (XyzConverter.Kappa * XyzConverter.Epsilon) ? Math.Pow(((lab.L + 16.0) / 116.0), 3) : lab.L / XyzConverter.Kappa),
				white.Z * (z3 > XyzConverter.Epsilon ? z3 : (z - 16.0 / 116.0) / 7.787));

			return XyzConverter.ToColor(xyz);
		}

		private static double PivotXyz(double n) {
			return n > XyzConverter.Epsilon ? CubicRoot(n) : (XyzConverter.Kappa * n + 16) / 116;
		}

		private static double CubicRoot(double n) {
			return Math.Pow(n, 1.0 / 3.0);
		}
	}
}
