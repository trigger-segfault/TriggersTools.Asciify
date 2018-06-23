using TriggersTools.Asciify.ColorMine.Comparisons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TriggersTools.Asciify.ColorMine {
	public enum ColorSpaceComparisonType {
		None = -1,
		[Description("CIE76")]
		Cie76,
		[Description("CIE94")]
		Cie94,
		[Description("CIEDE2000")]
		CieDe2000,
		[Description("CMC l:c")]
		Cmc,
	}
	public static partial class ColorSpaceComparer {

		public static IColorSpaceComparison GetComparer(ColorSpaceComparisonType type) {
			switch (type) {
			case ColorSpaceComparisonType.Cie76: return new Cie76Comparison();
			case ColorSpaceComparisonType.Cie94: return new Cie94Comparison();
			case ColorSpaceComparisonType.CieDe2000: return new CieDe2000Comparison();
			case ColorSpaceComparisonType.Cmc: return new CmcComparison();
			default: throw new ArgumentException(nameof(type));
			}
		}
	}
}
