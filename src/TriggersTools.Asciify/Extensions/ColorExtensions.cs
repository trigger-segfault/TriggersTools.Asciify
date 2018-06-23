using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TriggersTools.Asciify.Extensions {
	public static class ColorExtensions {

		public static bool Matches(this Color a, Color b) {
			return a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
		}

		public static bool Matches(this Color a, Color b, bool compareAlpha) {
			return	a.R == b.R && a.G == b.G && a.B == b.B &&
					(!compareAlpha || a.A == b.A);
		}

		public static int RgbTotal(this Color color) {
			return color.R + color.G + color.B;
		}
	}
}
