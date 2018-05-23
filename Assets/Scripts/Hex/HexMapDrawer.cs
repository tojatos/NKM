using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace Hex
{
	public class HexMapDrawer : SingletonMonoBehaviour<HexMapDrawer>
	{
		private Game Game;

		void Awake()
		{
			Game = GameStarter.Instance.Game;
		}
		public HexMap HexMap { get; private set; }
		public HexCell CellPrefab;
		public int Width { get; private set; }
		public int Height { get; private set;}
		public List<HexCell> Cells;
		HexMesh _hexMesh;
		public void CreateMap(HexMap hexMap)
		{
			HexMap = hexMap;
			Width = HexMap.Map.width;
			Height = HexMap.Map.height;
			_hexMesh = GetComponentInChildren<HexMesh>();
			_hexMesh.Init();
			Cells = new List<HexCell>();
			for (int z = 0, i = 0; z < Height; z++)
			{
				for (var x = 0; x < Width; x++)
				{
					CreateCell(x, z, i++);
				}
			}

			TriangulateCells();
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

			Debug.LogError(cell.Coordinates + " nie ma zmapowanego typu!");// + '\n' + "Red: " + ((Math.Abs(pixelColor.r - HexMap.ColorMappings[3].Color.r) < 0.001f) ? "Match" : "Nay") + " Green: " + ((Math.Abs(pixelColor.g - HexMap.ColorMappings[3].Color.g) < 0.001f) ? "Match" : "Nay") + " Blue: " + ((Math.Abs(pixelColor.b - HexMap.ColorMappings[3].Color.b) < 0.001f) ? "Match" : "Nay"));

		}

		public void RemoveAllHighlights()
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
		public void RemoveAllHelpHighlights()
		{
			foreach (var hexCell in Cells)
			{
				if (hexCell.HelpHighlight != null)
				{
					hexCell.ToggleHelpHighlight();
				}
			}
		}
		public HexCell GetCellByPosition(ref Vector3 position)
		{
			position = transform.InverseTransformPoint(position);
			var coordinates = HexCoordinates.FromPosition(position);
			var index = coordinates.X + coordinates.Z * Instance.Width + coordinates.Z / 2;
			var touchedCell = Game.HexMapDrawer.Cells[index];
			return touchedCell;
		}

		public void Update()
		{
			if(!Game.IsInitialized) return;
			if (Game.UIManager.VisibleUI != Game.UIManager.GameUI) return;

			if (Game.Active.AirSelection.IsEnabled)
			{
				var cellPointed = CellPointed();
				if (cellPointed != null && Game.Active.HexCells.Contains(cellPointed))
				{
					Game.Active.AirSelection.HexCells = new List<HexCell> { cellPointed };
				}
			}

			if (Game.Active.Action == Action.AttackAndMove)
			{
				var cellPointed = CellPointed();
				if (cellPointed != null && (Game.Active.HexCells.Contains(cellPointed)||cellPointed==Game.Active.CharacterOnMap.ParentCell))
				{
					var lastMoveCell = Game.Active.MoveCells.LastOrDefault();
					if(lastMoveCell==null) throw new Exception("Move cell is null!");
					if (cellPointed != lastMoveCell)
					{
						if (Game.Active.MoveCells.Contains(cellPointed))
						{
							//remove all cells to pointed
							for (int i = Game.Active.MoveCells.Count - 1; i >= 0; i--)
							{
								if (Game.Active.MoveCells[i] == cellPointed) break;

								//Remove the line
								Destroy(Game.Active.MoveCells[i].gameObject.GetComponent<LineRenderer>());

								Game.Active.MoveCells.RemoveAt(i);
							}
						}
						else if (Game.Active.CharacterOnMap.Speed.Value >= Game.Active.MoveCells.Count &&
						         lastMoveCell.GetNeighbors(1).Contains(cellPointed) && cellPointed.CharacterOnCell == null)
						{
							Game.Active.AddMoveCell(cellPointed);

						}
					}
				}
			}
			if (Input.GetMouseButtonDown(0))
			{
				if (Game.Active.IsPointerOverUIObject()) return; //Do not touch cells if mouse is over UI

				var cellPointed = CellPointed();
				if (cellPointed != null)
				{
					if (Game.Active.AirSelection.IsEnabled && Game.Active.HexCells.Contains(cellPointed))
					{
						Game.Active.MakeAction(Game.Active.AirSelection.HexCells);
					}
					else
					{
						Game.TryTouchingCell(cellPointed);
					}
				}
			}

			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
			{
				Game.Active.Cancel();
			}
		}
		private HexCell CellPointed()
		{
			var inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (!Physics.Raycast(inputRay, out hit)) return null;

			var position = hit.point;
			return Instance.GetCellByPosition(ref position);
		}
	}
}
