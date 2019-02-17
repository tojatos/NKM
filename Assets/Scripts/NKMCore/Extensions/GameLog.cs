using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Hex;

namespace NKMCore.Extensions
{
	public static class GameLog
	{
        public static string GetFirst(this string[][] data, string key) => data.First(x => x[0] == key)[1];
		public static string[] SplitData(this string toSplit) => toSplit.SplitData("; ");
        public static string[] SplitData(this string toSplit, string delimiter) => toSplit.Split(new[] {delimiter}, StringSplitOptions.RemoveEmptyEntries);
        public static List<HexCell> ConvertToHexCellList(this string[] coordinatesArray, HexMap map) => coordinatesArray.Select(coordinates => map.Cells.First(c => c.Coordinates.ToString() == coordinates)).ToList();
        public static string ConvertToNameWithoutID(this string nameWithID)
		{
			string[] n = nameWithID.Split(' ');
            return string.Join(" ", n.Take(n.Length - 1)); //return everything except last element, that is ID
		}
	}
}
