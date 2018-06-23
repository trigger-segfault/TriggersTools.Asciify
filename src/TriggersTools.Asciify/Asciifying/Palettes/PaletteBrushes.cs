using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TriggersTools.Asciify.Asciifying.Palettes {
	public class PaletteBrushes : IEnumerable<SolidBrush>, IDisposable {

		public SolidBrush[] Brushes { get; }
		public SolidBrush Foreground { get; }
		public SolidBrush Background { get; }
		
		public PaletteBrushes(IReadOnlyList<Color> colors, Color? foreground, Color? background) {
			Brushes = new SolidBrush[colors.Count];
			for (int i = 0; i < colors.Count; i++)
				Brushes[i] = new SolidBrush(colors[i]);
			if (foreground.HasValue)
				Foreground = new SolidBrush(foreground.Value);
			if (background.HasValue)
				Background = new SolidBrush(background.Value);
		}

		public IEnumerator<SolidBrush> GetEnumerator() {
			for (int i = 0; i < Brushes.Length; i++)
				yield return Brushes[i];
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public SolidBrush this[int index, bool fore] {
			get {
				if (index != -1)
					return Brushes[index];
				else if (fore)
					return Foreground;
				else
					return Background;
			}
		}

		public int Count => Brushes.Length;

		public void Dispose() {
			foreach (SolidBrush brush in Brushes)
				brush.Dispose();
		}
	}
}
