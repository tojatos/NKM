using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using NKMObjects.Templates;
using UnityEngine;

namespace Hex
{
	public class HexMapDrawer : SingletonMonoBehaviour<HexMapDrawer>
	{
		private Game _game;
		private Active Active => _game.Active;
		private Action Action => _game.Action;
		public DrawnHexCell CellPrefab;
		public List<DrawnHexCell> Cells;
		private readonly Dictionary<Character, GameObject> _characterObjects = new Dictionary<Character, GameObject>();

		public GameObject GetCharacterObject(Character character)
		{
			GameObject value;
			_characterObjects.TryGetValue(character, out value);
			return value;
		}

		public void SetCharacterObject(Character character, GameObject cObject) => _characterObjects[character] = cObject;
	    
		private HexMesh _hexMesh;
		public void Init(Game game) => _game = game;

		public void CreateMap(HexMap hexMap)
		{
			_hexMesh = GetComponentInChildren<HexMesh>();
			_hexMesh.Init();
			hexMap.Cells.ForEach(CreateCell);
			
			TriangulateCells();
		}

		public void TriangulateCells()
		{
			_hexMesh.Triangulate(Cells);
		}

		private void CreateCell(HexCell hexCell)
		{
			Vector3 position;
			position.x = (hexCell.Coordinates.X + hexCell.Coordinates.Z * 0.5f) * (HexMetrics.InnerRadius * 2f);
			position.y = 0f;
			position.z = hexCell.Coordinates.Z * (HexMetrics.OuterRadius * 1.5f);
			
			DrawnHexCell cell = Instantiate(CellPrefab);
			Cells.Add(cell);
			cell.HexCell = hexCell;
			cell.transform.SetParent(transform, false);
			cell.transform.localPosition = position;

			switch (hexCell.Type)
			{
				case HexCell.TileType.Normal:
                    cell.Color = Color.white;
					break;
				case HexCell.TileType.Wall:
                    cell.Color = Color.black;
					break;
				case HexCell.TileType.SpawnPoint1:
				case HexCell.TileType.SpawnPoint2:
				case HexCell.TileType.SpawnPoint3:
				case HexCell.TileType.SpawnPoint4:
                    cell.Color = Color.green;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void RemoveHighlights(Predicate<GameObject> predicate = null)
		{
			foreach (DrawnHexCell hexCell in Cells)
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
			return _game.HexMap.Cells.First(c => c.Coordinates == coordinates);
		}

		public void Update()
		{
			if(_game == null || !_game.IsInitialized) return;
//			if (Game.UIManager.VisibleUI != Game.UIManager.GameUI) return;

			if (_game.Active.AirSelection.IsEnabled)
			{
				HexCell cellPointed = CellPointed();
				if (cellPointed != null && _game.Active.HexCells.Contains(cellPointed))
				{
					_game.Active.AirSelection.HexCells = new List<HexCell> { cellPointed };
				}
			}

//			if (_game.Active.ActionType == ActionType.AttackAndMove)
			if(_game.Active.Character!=null && _game.Active.Character.CanUseBasicMove && _game.Active.HexCells != null)
			{
				HexCell cellPointed = CellPointed();
				if (cellPointed != null && (_game.Active.HexCells.Contains(cellPointed)||cellPointed==_game.Active.Character.ParentCell))
				{
					HexCell lastMoveCell = _game.Active.MoveCells.LastOrDefault();
					if(lastMoveCell==null) throw new Exception("Move cell is null!");
					if (cellPointed != lastMoveCell)
					{
						if (_game.Active.MoveCells.Contains(cellPointed))
						{
							//remove all cells to pointed
							for (int i = _game.Active.MoveCells.Count - 1; i >= 0; i--)
							{
								if (_game.Active.MoveCells[i] == cellPointed) break;

								//Remove the line
								Destroy(Active.SelectDrawnCell(_game.Active.MoveCells[i]).gameObject
									.GetComponent<LineRenderer>());

								_game.Active.MoveCells.RemoveAt(i);
							}
						}
						else if (_game.Active.Character.Speed.Value >= _game.Active.MoveCells.Count &&
						         lastMoveCell.GetNeighbors(_game.Active.GamePlayer, 1).Contains(cellPointed) && (cellPointed.CharactersOnCell.Count == 0||!cellPointed.CharactersOnCell.Any(c => c.IsEnemyFor(_game.Active.Character.Owner))))
						{
							_game.Active.AddMoveCell(cellPointed);

						}
					}
				}
			}
			if (Input.GetMouseButtonDown(0))
			{
				if (Game.IsPointerOverUiObject()) return; //Do not touch cells if mouse is over UI

				HexCell cellPointed = CellPointed();
				if (cellPointed != null)
				{
					if (_game.Active.AirSelection.IsEnabled && _game.Active.HexCells?.Contains(cellPointed) == true)
					{
						//_game.Active.MakeAction(_game.Active.AirSelection.HexCells);
						
					}
					else
					{
						TouchCell(cellPointed);
					}
				}
			}

			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
			{
				_game.Active.Cancel();
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

		private void TouchCell(HexCell touchedCell)
        {
            Active.SelectedCell = touchedCell;
            if (Active.SelectedCharacterToPlace != null)
            {
                Action.PlaceCharacter(Active.SelectedCharacterToPlace, touchedCell);
            }
            else if (Active.HexCells?.Contains(touchedCell) == true)
            {
                if (Active.AbilityToUse != null)
                {
                    Action.UseAbility(Active.AbilityToUse, Active.AirSelection.IsEnabled ? Active.AirSelection.HexCells : Active.HexCells);
                }
                else if (Active.Character != null)
                {
                    if(!touchedCell.IsEmpty && Active.Character.CanBasicAttack(touchedCell.FirstCharacter))
                        Action.BasicAttack(Active.Character, touchedCell.FirstCharacter);
                    else if(touchedCell.IsFreeToStand && Active.Character.CanBasicMove(touchedCell) && Active.MoveCells.Last() == touchedCell)
                        Action.BasicMove(Active.Character, Active.MoveCells);
                }
            }
            else
            {
                if (Active.AbilityToUse != null)
                {
                    return;
                }
                //possibility of highlighting with control pressed
                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    RemoveHighlights();
                }
                if(!touchedCell.IsEmpty) Action.Select(touchedCell.FirstCharacter);
                else
                {
                    Action.Deselect();
                    Active.SelectDrawnCell(touchedCell).AddHighlight(Highlights.BlackTransparent);
                }
            }
        }
	}
}
