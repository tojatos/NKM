using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace Hex
{
	public class HexMapDrawer : SingletonMonoBehaviour<HexMapDrawer>
	{
		private static Game Game => GameStarter.Instance.Game;

		public HexMapScriptable HexMapScriptable { get; set; }
		public HexCell CellPrefab;
		public int Width { get; private set; }
		public int Height { get; private set;}
		public List<HexCell> Cells;
		private HexMesh _hexMesh;

		public void CreateMap(HexMap hexMap)
		{
			_hexMesh = GetComponentInChildren<HexMesh>();
			_hexMesh.Init();
			hexMap.Cells.ForEach(CreateCell);
			
			TriangulateCells();
		}
//		public void CreateMap(HexMapScriptable hexMapScriptable)
//		{
//			HexMapScriptable = hexMapScriptable;
//			Width = HexMapScriptable.Map.width;
//			Height = HexMapScriptable.Map.height;
//			_hexMesh = GetComponentInChildren<HexMesh>();
//			_hexMesh.Init();
//			Cells = new List<HexCell>();
//			for (int z = 0, i = 0; z < Height; z++)
//			{
//				for (var x = 0; x < Width; x++)
//				{
//					CreateCell(x, z, i++);
//				}
//			}
//
//			TriangulateCells();
//		}

		public void TriangulateCells()
		{
			_hexMesh.Triangulate(Cells);
		}

		private void CreateCell(BetterHexCell hexCell)
		{
			Vector3 position;
			position.x = (hexCell.Coordinates.X + hexCell.Coordinates.Z * 0.5f) * (HexMetrics.InnerRadius * 2f);
			position.y = 0f;
			position.z = hexCell.Coordinates.Z * (HexMetrics.OuterRadius * 1.5f);
			
			HexCell cell = Instantiate(CellPrefab);
			Cells.Add(cell);
			cell.BetterHexCell = hexCell;
			cell.transform.SetParent(transform, false);
			//cell.Type = hexCell.Type;
			cell.transform.localPosition = position;
			//TODO: neighbors not set

			switch (cell.Type)
			{
				case HexTileType.Normal:
                    cell.Color = Color.white;
					break;
				case HexTileType.Wall:
                    cell.Color = Color.black;
					break;
				case HexTileType.SpawnPoint1:
				case HexTileType.SpawnPoint2:
				case HexTileType.SpawnPoint3:
				case HexTileType.SpawnPoint4:
                    cell.Color = Color.green;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
//		private void CreateCell(int x, int z, int i)
//		{
//			Color pixelColor = HexMapScriptable.Map.GetPixel(x, z);
//			if (Math.Abs(pixelColor.a) < 0.001) //transparent pixel
//			{
//				return;
//			}
//
//			Vector3 position;
//			// ReSharper disable once PossibleLossOfFraction
//			position.x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f);
//			position.y = 0f;
//			position.z = z * (HexMetrics.OuterRadius * 1.5f);
//
//			HexCell cell = Instantiate(CellPrefab);
//			Cells.Add(cell);
//			cell.transform.SetParent(transform, false);
//			cell.transform.localPosition = position;
//			cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
//			cell.Color = new Color(255, 255, 255);
//			if (x > 0)
//			{
//				cell.SetNeighbor(HexDirection.W, Cells[i - 1]);
//			}
//			if (z > 0)
//			{
//				if ((z & 1) == 0)
//				{
//					cell.SetNeighbor(HexDirection.Se, Cells[i - Width]);
//					if (x > 0)
//					{
//						cell.SetNeighbor(HexDirection.Sw, Cells[i - Width - 1]);
//					}
//				}
//				else
//				{
//					cell.SetNeighbor(HexDirection.Sw, Cells[i - Width]);
//					if (x < Width - 1)
//					{
//						cell.SetNeighbor(HexDirection.Se, Cells[i - Width + 1]);
//					}
//				}
//
//			}
//
//			foreach (ColorToTileType colorMapping in HexMapScriptable.ColorMappings)
//			{
//				if (colorMapping.Color.Equals(pixelColor))
//				{
//					cell.Type = colorMapping.HexTileType;
//					if(HexMapScriptable.SpawnPoints.Contains( cell.Type ))
//					{
//						cell.Color = Color.green;
//					}
//					else if (cell.Type == HexTileType.Normal)
//					{
//						cell.Color = Color.white;
//					}
//					else if (cell.Type == HexTileType.Wall)
//					{
//						cell.Color = Color.black;
//					}
//					else
//					{
//						Debug.Log(cell.Type);
//						cell.Color = pixelColor;
//					}
//					return;
//				}
//			}
//
//			Debug.LogError(cell.Coordinates + " nie ma zmapowanego typu!");// + '\n' + "Red: " + ((Math.Abs(pixelColor.r - HexMap.ColorMappings[3].Color.r) < 0.001f) ? "Match" : "Nay") + " Green: " + ((Math.Abs(pixelColor.g - HexMap.ColorMappings[3].Color.g) < 0.001f) ? "Match" : "Nay") + " Blue: " + ((Math.Abs(pixelColor.b - HexMap.ColorMappings[3].Color.b) < 0.001f) ? "Match" : "Nay"));
//
//		}

		public void RemoveHighlights(Predicate<GameObject> predicate = null)
		{
			foreach (HexCell hexCell in Cells)
			{
				if (predicate == null)
				{
                    hexCell.Highlights.ForEach(Destroy);
					hexCell.Highlights.Clear();
				}
				else
				{
					hexCell.Highlights.FindAll(predicate).ForEach(Destroy);
					hexCell.Highlights.RemoveAll(predicate);
				}
				//TODO: Check that somewhere else (change responsibility?)
//				if (hexCell.Highlight != null)
//				{
//					hexCell.AddHighlight();
//				}
			}
		}
		public void RemoveHighlightsOfColor(string colorName) => RemoveHighlights(h => h.GetComponent<SpriteRenderer>().sprite.name == colorName);

		private HexCell GetCellByPosition(ref Vector3 position)
		{
			position = transform.InverseTransformPoint(position);
			HexCoordinates coordinates = HexCoordinates.FromPosition(position);
			return Cells.First(c => c.Coordinates == coordinates);
		}

		public void Update()
		{
			if(!Game.IsInitialized) return;
//			if (Game.UIManager.VisibleUI != Game.UIManager.GameUI) return;

			if (Game.Active.AirSelection.IsEnabled)
			{
				HexCell cellPointed = CellPointed();
				if (cellPointed != null && Game.Active.HexCells.Contains(cellPointed))
				{
					Game.Active.AirSelection.HexCells = new List<HexCell> { cellPointed };
				}
			}

			if (Game.Active.Action == Action.AttackAndMove)
			{
				HexCell cellPointed = CellPointed();
				if (cellPointed != null && (Game.Active.HexCells.Contains(cellPointed)||cellPointed==Game.Active.CharacterOnMap.ParentCell))
				{
					HexCell lastMoveCell = Game.Active.MoveCells.LastOrDefault();
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
						         lastMoveCell.GetNeighbors(1).Contains(cellPointed) && (cellPointed.CharacterOnCell == null||!cellPointed.CharacterOnCell.IsEnemyFor(Game.Active.CharacterOnMap.Owner)))
						{
							Game.Active.AddMoveCell(cellPointed);

						}
					}
				}
			}
			if (Input.GetMouseButtonDown(0))
			{
				if (Active.IsPointerOverUiObject()) return; //Do not touch cells if mouse is over UI

				HexCell cellPointed = CellPointed();
				if (cellPointed != null)
				{
					if (Game.Active.AirSelection.IsEnabled && Game.Active.HexCells.Contains(cellPointed))
					{
						Game.Active.MakeAction(Game.Active.AirSelection.HexCells);
					}
					else
					{
						Game.TouchCell(cellPointed);
					}
				}
			}

			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
			{
				Game.Active.Cancel();
			}
		}
		private static HexCell CellPointed()
		{
			Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (!Physics.Raycast(inputRay, out hit)) return null;

			Vector3 position = hit.point;
			return Instance.GetCellByPosition(ref position);
		}
	}
}
