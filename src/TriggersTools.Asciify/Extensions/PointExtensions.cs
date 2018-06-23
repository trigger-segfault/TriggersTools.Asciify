using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TriggersTools.Asciify.Utility {
	public static class PointExtensions {

		public static IEnumerable<Point> Enumerate(this Point range) {
			for (int y = 0; y < range.Y; y++) {
				for (int x = 0; x < range.X; x++) {
					yield return new Point(x, y);
				}
			}
		}

		public static IEnumerable<Point> Enumerate(this Size size) {
			for (int y = 0; y < size.Height; y++) {
				for (int x = 0; x < size.Width; x++) {
					yield return new Point(x, y);
				}
			}
		}

		public static IEnumerable<Point> Enumerate(this Rectangle area) {
			for (int y = area.Y; y < area.Bottom; y++) {
				for (int x = area.X; x < area.Right; x++) {
					yield return new Point(x, y);
				}
			}
		}

	}
}
