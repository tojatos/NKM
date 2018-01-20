using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hex
{
	public class HexMapDrawer : SingletonMonoBehaviour<HexMapDrawer> {
		public HexMap HexMap;
		public HexCell CellPrefab;
		public int Width { get; private set; }
		public int Height { get; private set;}
		public static List<HexCell> Cells;
		HexMesh _hexMesh;
		void Awake()
		{
			Width = HexMap.Map.width;
			Height = HexMap.Map.height;
			_hexMesh = GetComponentInChildren<HexMesh>();
			Cells = new List<HexCell>();
			for (int z = 0, i = 0; z < Height; z++)
			{
				for (var x = 0; x < Width; x++)
				{
					CreateCell(x, z, i++);
				}
			}

		}
		void Start () {
			_hexMesh.Triangulate(Cells);

		}
		public void TriangulateCells()
		{
			_hexMesh.Triangulate(Cells);
		}
		void CreateCell(int x, int z, int i)
		{
			var pixelColor = HexMap.Map.GetPixel(x, z);
			if (Math.Abs(pixelColor.a) < 0.001) //transparent pixel
			{
				return;
			}

			Vector3 position;
			// ReSharper disable once PossibleLossOfFraction
			position.x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f);
			position.y = 0f;
			position.z = z * (HexMetrics.OuterRadius * 1.5f);

			var cell = Instantiate(CellPrefab);
			Cells.Add(cell);
			cell.transform.SetParent(transform, false);
			cell.transform.localPosition = position;
			cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
			cell.Color = new Color(255, 255, 255);
			if (i == 85)
			{
				Debug.Log("");
			}
			if (x > 0)
			{
				cell.SetNeighbor(HexDirection.W, Cells[i - 1]);
			}
			if (z > 0)
			{
				if ((z & 1) == 0)
				{
					cell.SetNeighbor(HexDirection.Se, Cells[i - Width]);
					if (x > 0)
					{
						cell.SetNeighbor(HexDirection.Sw, Cells[i - Width - 1]);
					}
				}
				else
				{
					cell.SetNeighbor(HexDirection.Sw, Cells[i - Width]);
					if (x < Width - 1)
					{
						cell.SetNeighbor(HexDirection.Se, Cells[i - Width + 1]);
					}
				}

			}

			foreach (var colorMapping in HexMap.ColorMappings)
			{
				if (colorMapping.Color.Equals(pixelColor))
				{
					cell.Type = colorMapping.HexTileType;
					if(HexMap.SpawnPoints.Contains( cell.Type ))
					{
						cell.Color = Color.green;
					}
					else if (cell.Type == HexTileType.Normal)
					{
						cell.Color = Color.white;
					}
					else if (cell.Type == HexTileType.Wall)
					{
						cell.Color = Color.black;
					}
					else
					{
						Debug.Log(cell.Type);
						cell.Color = pixelColor;
					}
					return;
				}
			}

			Debug.LogError(cell.Coordinates + " nie ma zmapowanego typu!" + '\n' + "Red: " + ((Math.Abs(pixelColor.r - HexMap.ColorMappings[3].Color.r) < 0.001f) ? "Match" : "Nay") + " Green: " + ((Math.Abs(pixelColor.g - HexMap.ColorMappings[3].Color.g) < 0.001f) ? "Match" : "Nay") + " Blue: " + ((Math.Abs(pixelColor.b - HexMap.ColorMappings[3].Color.b) < 0.001f) ? "Match" : "Nay"));

		}

		public static void RemoveAllHighlights()
		{
			foreach (var hexCell in Cells)
			{
				//TODO: Check that somewhere else (change responsibility?)
				if (hexCell.Highlight != null)
				{
					hexCell.ToggleHighlight();
				}
			}
		}
		public static void RemoveAllHelpHighlights()
		{
			foreach (var hexCell in Cells)
			{
				if (hexCell.HelpHighlight != null)
				{
					hexCell.ToggleHelpHighlight();
				}
			}
		}
	}
}
