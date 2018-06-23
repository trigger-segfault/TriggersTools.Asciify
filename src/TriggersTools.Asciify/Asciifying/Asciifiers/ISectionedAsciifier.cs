using System;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.Asciify.Asciifying.Asciifiers {
	public interface ISectionedAsciifier : IAsciifier {
		double AllFactor { get; set; }
	}
	public interface ISectionedColorAsciifier : ISectionedAsciifier, IColorAsciifier {
	}
	public interface ISectionedIntensityAsciifier : ISectionedAsciifier, IIntensityAsciifier {
	}
}
