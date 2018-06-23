using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace TriggersTools.Asciify.Extensions {
	public static class ArrayExtensions {

		public static T[][] ToJaggedArray<T>(this T[,] array, bool reverse) {
			T[][] output;
			int lengthx = 0, lengthy = 0;
			if (reverse) {
				lengthy = array.GetLength(0);
				lengthx = array.GetLength(1);
				output = new T[lengthx][];
				for (int x = 0; x < lengthx; x++) {
					output[x] = new T[lengthy];
					for (int y = 0; y < lengthy; y++)
						output[x][y] = array[y, x];
				}
			}
			else {
				lengthx = array.GetLength(0);
				lengthy = array.GetLength(1);
				output = new T[lengthx][];
				for (int x = 0; x < lengthx; x++) {
					output[x] = new T[lengthy];
					for (int y = 0; y < lengthy; y++)
						output[x][y] = array[x, y];
				}
			}
			return output;
		}

		public static T[,] ToMultiArray<T>(this T[][] array, bool reverse) {
			T[,] output;
			int lengthx = 0, lengthy = 0;
			if (reverse) {
				lengthy = array.Length;
				if (lengthy != 0)
					lengthx = array[0].Length;
				output = new T[lengthx, lengthy];
				for (int x = 0; x < lengthx; x++) {
					LengthCheck(array, x, lengthy, 1);
					for (int y = 0; y < lengthy; y++)
						output[x, y] = array[y][x];
				}
			}
			else {
				lengthy = array.Length;
				if (lengthy != 0)
					lengthx = array[0].Length;
				output = new T[lengthx, lengthy];
				for (int x = 0; x < lengthx; x++) {
					LengthCheck(array, x, lengthy, 1);
					for (int y = 0; y < lengthy; y++)
						output[x, y] = array[x][y];
				}
			}
			return output;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void LengthCheck<T>(T[][] array, int index, int length, int level) {
			if (array[index].Length != length) {
				throw new ArgumentException($"Jarred array level: {level}, index: " +
					$"{index}, length: {array[index].Length} does not match " +
					$"initial length of {length}!");
			}
		}
	}
}
