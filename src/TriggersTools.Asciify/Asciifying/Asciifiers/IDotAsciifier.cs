using System;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.Asciify.Asciifying.Asciifiers {
	public interface IDotAsciifier : IAsciifier {
	}
	public interface IDotColorAsciifier : IDotAsciifier, IColorAsciifier {
	}
	public interface IDotIntensityAsciifier : IDotAsciifier, IIntensityAsciifier {
	}
}
