using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Hex
{
    public static class HexMapFactory
    {
        public static HexMap FromScriptable(HexMapScriptable hexMapScriptable)
        {
			int width = hexMapScriptable.Map.width;
			int height = hexMapScriptable.Map.height;
	        HexMap map = new HexMap(new List<HexCell>(), hexMapScriptable.SpawnPoints);
			for (int z = 0, i = 0; z < height; z++)
			{
				for (int x = 0; x < width; x++)
				{
                    Color pixelColor = hexMapScriptable.Map.GetPixel(x, z);
					if (Math.Abs(pixelColor.a) < 0.001) continue; //transparent pixel
					
					HexTileType type = hexMapScriptable.ColorMappings.ToList()
						.First(c => c.Color.Equals(pixelColor)).HexTileType;
					GetScriptableCell(ref map, width, type, x, z, i++);
				}
			}

	        return map;
        }

	    private static void GetScriptableCell(ref HexMap map, int width, HexTileType type, int x, int z, int i)
	    {
		    HexCell cell = new HexCell(map, HexCoordinates.FromOffsetCoordinates(x, z), type);
		    map.Cells.Add(cell);
			if (x > 0)
			{
				cell.SetNeighbor(HexDirection.W, map.Cells[i - 1]);
			}
			if (z > 0)
			{
				if ((z & 1) == 0)
				{
					cell.SetNeighbor(HexDirection.Se, map.Cells[i - width]);
					if (x > 0)
					{
						cell.SetNeighbor(HexDirection.Sw, map.Cells[i - width - 1]);
					}
				}
				else
				{
					cell.SetNeighbor(HexDirection.Sw, map.Cells[i - width]);
					if (x < width - 1)
					{
						cell.SetNeighbor(HexDirection.Se, map.Cells[i - width + 1]);
					}
				}

			}
	    }
    }
}