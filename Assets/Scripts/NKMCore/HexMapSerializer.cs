using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;

namespace NKMCore
{
    public static class HexMapSerializer
    {
        public static string Serialize(HexMap map)
        {
            string coords = string.Join("\n",
                map.Cells.Select(c =>
                    c.Coordinates.X.ToString() + ':' + c.Coordinates.Z.ToString() + ';' + c.Type.ToString()));
            string spawnPoints = string.Join(";", map.SpawnPoints.Select(s => s.ToString()));
            
            return string.Join("\n\n", coords, spawnPoints);
        }

        public static HexMap Deserialize(string mapString)
        {
            string[] sc = mapString.Split(new [] {"\n\n"}, StringSplitOptions.None);
            string[] cellsInfo = sc[0].Split('\n');
            string spawnPointInfo = sc[1];
            List<HexCell.TileType> spawnPoints = spawnPointInfo.Split(';').Select(s => s.ToEnum<HexCell.TileType>()).ToList();

            var map = new HexMap(new List<HexCell>(), spawnPoints);
            cellsInfo.ToList().ForEach(i =>
            {
                string[] s = i.Split(';');
                int[] coords = s[0].Split(':').Select(int.Parse).ToArray();
                var tileType = s[1].ToEnum<HexCell.TileType>();
                var coordinates = new HexCoordinates(coords[0], coords[1]);
                var cell = new HexCell(map, coordinates, tileType);
                map.Cells.Add(cell);
            });
            
            map.Cells.ForEach(c =>
            {
                map.Cells.FindAll(w =>
                        Math.Abs(w.Coordinates.X - c.Coordinates.X) <= 1 &&
                        Math.Abs(w.Coordinates.Y - c.Coordinates.Y) <= 1 &&
                        Math.Abs(w.Coordinates.Z - c.Coordinates.Z) <= 1 &&
                        w != c) 
                    .ForEach(w => c.SetNeighbor(c.GetDirection(w), w));
            });
            
            return map;
        }
    }
}