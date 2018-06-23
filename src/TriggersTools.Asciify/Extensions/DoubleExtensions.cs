using System;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.Asciify.Extensions {
	public static class DoubleExtensions {

		public static double ZeroNaN(this double value) {
			if (double.IsNaN(value))
				return 0d;
			return value;
		}
	}
}
