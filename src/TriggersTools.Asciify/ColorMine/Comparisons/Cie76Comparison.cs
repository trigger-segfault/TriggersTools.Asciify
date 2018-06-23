using System;

namespace TriggersTools.Asciify.ColorMine.Comparisons {
	/// <summary>
	/// Implements the CIE76 method of delta-e: http://en.wikipedia.org/wiki/Color_difference#CIE76
	/// </summary>
	public class Cie76Comparison : IColorSpaceComparison {

		public const double MaxScore = 375.59552713;


		/// <summary>
		/// Calculates the CIE76 delta-e value: http://en.wikipedia.org/wiki/Color_difference#CIE76
		/// </summary>
		public double Compare(ColorLab a, ColorLab b, bool quick = false) {
			double differences = Distance(a.L, b.L) + Distance(a.A, b.A) + Distance(a.B, b.B);
			return (quick ? differences : Math.Sqrt(differences));
		}

		private static double Distance(double a, double b) {
			return (a - b) * (a - b);
		}

		public static double CompareS(ColorLab a, ColorLab b, bool quick = false) {
			double differences = Distance(a.L, b.L) + Distance(a.A, b.A) + Distance(a.B, b.B);
			return (quick ? differences : Math.Sqrt(differences));
		}
	}
}