﻿using System;
using System.Collections.Generic;
using System.Linq;
using Hex;

namespace Extensions
{
	public static class GameLog
	{
        public static string GetFirst(this string[][] data, string key) => data.First(x => x[0] == key)[1];
        public static string[] SplitData(this string toSplit) => toSplit.Split(new[] {"; "}, StringSplitOptions.RemoveEmptyEntries);
        public static List<HexCell> ConvertToHexCellList(this string[] coordinatesArray) => coordinatesArray.Select(coordinates => HexMapDrawer.Instance.Cells.First(c => c.Coordinates.ToString() == coordinates)).ToList();
        public static string ConvertToNameWithoutID(this string nameWithID)
		{
			string[] n = nameWithID.Split(' ');
            return string.Join(" ", n.Take(n.Length - 1)); //return everything except last element, that is ID
		}
	}
}
