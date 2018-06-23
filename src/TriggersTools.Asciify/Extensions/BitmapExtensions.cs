using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace TriggersTools.Asciify.Extensions {
	public class DisposableBitmapData : IDisposable {

		public Bitmap Bitmap { get; }
		public BitmapData Data { get; }

		public DisposableBitmapData(Bitmap bitmap, ImageLockMode mode, PixelFormat? format = null) {
			Bitmap = bitmap;
			Data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), mode, format ?? bitmap.PixelFormat);
		}

		public DisposableBitmapData(Bitmap bitmap, BitmapData data) {
			Bitmap = bitmap;
			Data = data;
		}

		public static implicit operator BitmapData(DisposableBitmapData data)
			=> data.Data;

		public void Dispose() {
			Bitmap.UnlockBits(Data);
		}
	}

	public struct PixelPoint {

		public int X { get; }
		public int Y { get; }
		public Point Point => new Point(X, Y);
		public Color Color { get; }

		public override string ToString() {
			return $"{{X={X} Y={Y}}} {{R={Color.R} B={Color.B} B={Color.B}}}";
		}

		public PixelPoint(int x, int y, Color color) {
			X = x;
			Y = y;
			Color = color;
		}

		public PixelPoint(Point point, Color color) {
			X = point.X;
			Y = point.Y;
			Color = color;
		}

		public static implicit operator Color(PixelPoint pp) {
			return pp.Color;
		}

		public static implicit operator Point(PixelPoint pp) {
			return pp.Point;
		}
	}

	public static class BitmapExtensions {
		
		public static void DrawImage(this Graphics g, Image image, Point point, Rectangle srcRect, ImageAttributes imageAttr) {
			Rectangle dstRect = new Rectangle(point.X, point.Y, srcRect.Width, srcRect.Height);
			g.DrawImage(
				image,
				dstRect,
				srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
				GraphicsUnit.Pixel,
				imageAttr);
		}

		public static IEnumerable<PixelPoint> ForEachPixel(this BitmapData data) {
			for (int y = 0; y < data.Height; y++) {
				for (int x = 0; x < data.Width; x++) {
					yield return new PixelPoint(x, y, data.GetColor(x, y));
				}
			}
		}

		public static IEnumerable<PixelPoint> ForEachPixel(this BitmapData data, Rectangle area) {
			for (int y = area.Y; y < area.Bottom && y < data.Height; y++) {
				for (int x = area.X; x < area.Right && x < data.Width; x++) {
					yield return new PixelPoint(x, y, data.GetColor(x, y));
				}
			}
		}

		public static IEnumerable<PixelPoint> ForEachPixel(this BitmapData data, Point point, Size size) {
			for (int y = point.Y; y < point.Y + size.Height && y < data.Height; y++) {
				for (int x = point.X; x < point.X + size.Width && x < data.Width; x++) {
					yield return new PixelPoint(x, y, data.GetColor(x, y));
				}
			}
		}

		public static DisposableBitmapData LockRead(this Bitmap bitmap, PixelFormat? format = null) {
			return new DisposableBitmapData(bitmap, ImageLockMode.ReadOnly, format);
		}

		public static DisposableBitmapData LockWrite(this Bitmap bitmap, PixelFormat? format = null) {
			return new DisposableBitmapData(bitmap, ImageLockMode.WriteOnly, format);
		}

		public static DisposableBitmapData LockReadWrite(this Bitmap bitmap, PixelFormat? format = null) {
			return new DisposableBitmapData(bitmap, ImageLockMode.ReadWrite, format);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void SetColor(this BitmapData data, Point point, Color color)
			=> data.SetColor(point.X, point.Y, color);

		public unsafe static void SetColor(this BitmapData data, int x, int y, Color color) {
			bool alpha = data.HasAlpha();
			byte* ptr = data.PtrAt(x, y);
			if (alpha) {
				ptr[2] = color.R;
				ptr[1] = color.G;
				ptr[0] = color.B;
				ptr[3] = color.A;
			}
			else {
				ptr[2] = color.R;
				ptr[1] = color.G;
				ptr[0] = color.B;
			}
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static bool MatchesColor(this BitmapData data, Point point, Color color)
			=> data.MatchesColor(point.X, point.Y, color);

		public unsafe static bool MatchesColor(this BitmapData data, int x, int y, Color color) {
			bool alpha = data.HasAlpha();
			byte* ptr = data.PtrAt(x, y);
			if (alpha)
				return	color.R == ptr[2] && color.G == ptr[1] &&
						color.B == ptr[0] && color.A == ptr[3];
			else
				return	color.R == ptr[2] && color.G == ptr[1] &&
						color.B == ptr[0] && color.A == 255;
		}

		public unsafe static Color GetColor(this BitmapData data, int x, int y) {
			bool alpha = data.HasAlpha();
			byte* ptr = data.PtrAt(x, y);
			if (alpha)
				return Color.FromArgb(ptr[3], ptr[2], ptr[1], ptr[0]);
			else
				return Color.FromArgb(ptr[2], ptr[1], ptr[0]);
		}

		public unsafe static bool HasAlpha(this BitmapData data) {
			return Image.IsAlphaPixelFormat(data.PixelFormat);
		}

		public static int GetBytesPerPixel(this BitmapData data)
			=> Image.GetPixelFormatSize(data.PixelFormat) / 8;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static byte* PtrAt(this BitmapData data, Point point)
			=> data.PtrAt(point.X, point.Y);

		private unsafe static byte* PtrAt(this BitmapData data, int x, int y) {
			int byteDepth = Image.GetPixelFormatSize(data.PixelFormat) / 8;
			return ((byte*) data.Scan0) + (y * data.Stride) + x * byteDepth;
		}

		public static void OpenInMSPaint(this Bitmap bitmap, [CallerMemberName] string fileName = null) {
			Directory.CreateDirectory("Temp");
			string file = Path.ChangeExtension(Path.Combine("Temp", fileName), ".png");
			bitmap.Save(file);
			ProcessStartInfo start = new ProcessStartInfo {
				UseShellExecute = false,
				FileName = "mspaint",
				Arguments = $"\"{file}\"",
			};
			Process.Start(start);
		}
	}
}
