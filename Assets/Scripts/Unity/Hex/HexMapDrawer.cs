using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity.Animations;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using Action = NKMCore.Action;

namespace Unity.Hex
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
	    
		public HexMesh HexMesh { get; private set; }
		public void Init(Game game)
		{
			_game = game;
			AddTriggersToEvents();
		}

		private void AddTriggersToEvents()
		{
            _game.HexMap.AfterMove += (character, cell) =>
            {
                if (GetCharacterObject(character) == null) return;
                GetCharacterObject(character).transform.parent = Active.SelectDrawnCell(cell).transform;
                AnimationPlayer.Add(new Destroy(Active.SelectDrawnCell(cell).gameObject.GetComponent<LineRenderer>())); //Remove the line
                AnimationPlayer.Add(new MoveTo(GetCharacterObject(character).transform,
                    GetCharacterObject(character).transform.parent.transform.TransformPoint(0, 10, 0), 0.13f));
                
            };
            _game.HexMap.AfterCharacterPlace += (character, cell) => Spawner.Instance.Spawn(Active.SelectDrawnCell(cell), character);
            Active.BeforeMoveCellsRemoved += cells => Active.SelectDrawnCells(cells).ForEach(c => Destroy(c.gameObject.GetComponent<LineRenderer>()));
            Active.AfterMoveCellAdded += hexcell =>
            {
                //Draw a line between two hexcell centres

                //Check for component in case of Zoro's Lack of Orientation
                DrawnHexCell cell = Active.SelectDrawnCell(hexcell);
                LineRenderer lRend = cell.gameObject.GetComponent<LineRenderer>() != null
                    ? cell.gameObject.GetComponent<LineRenderer>()
                    : cell.gameObject.AddComponent<LineRenderer>();
                lRend.SetPositions(new[]
                {
                    Active.SelectDrawnCell(Active.MoveCells.SecondLast()).transform.position + Vector3.up * 20,
                    cell.transform.position + Vector3.up * 20
                });
                lRend.material = new Material(Shader.Find("Standard")) {color = Color.black};
                lRend.startColor = Color.black;
                lRend.endColor = Color.black;
                lRend.widthMultiplier = 2;
            };
		}

		public void CreateMap(HexMap hexMap)
		{
			HexMesh = GetComponentInChildren<HexMesh>();
			HexMesh.Init();
			hexMap.Cells.ForEach(CreateCell);
			
			TriangulateCells();
		}

		public void TriangulateCells()
		{
			HexMesh.Triangulate(Cells);
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
			HexCoordinates coordinates = position.ToCoordinates();
			return _game.HexMap.Cells.First(c => c.Coordinates == coordinates);
		}

		public void Update()
		{
			if(_game == null) return;

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
				if (IsPointerOverUiObject()) return; //Do not touch cells if mouse is over UI

				HexCell cellPointed = CellPointed();
				if (cellPointed != null) TouchCell(cellPointed);
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

		
		public event Delegates.Cell AfterCellSelect;
		private void TouchCell(HexCell touchedCell)
        {
            Active.SelectedCell = touchedCell;
	        AfterCellSelect?.Invoke(touchedCell);
            if (Active.SelectedCharacterToPlace != null)
            {
	            if(!Active.GamePlayer.GetSpawnPoints(_game).Contains(touchedCell)) return;
                Action.PlaceCharacter(Active.SelectedCharacterToPlace, touchedCell);
	            if (Active.Phase.Number != 0) return;
	            Active.Turn.Finish();
            }
            else if (Active.HexCells?.Contains(touchedCell) == true)
            {
                if (Active.AbilityToUse != null)
                {
	                //It is important to check in that order, in case ability uses multiple interfaces!
	                if(Active.AbilityToUse is IUseableCharacter && touchedCell.FirstCharacter != null)
		                Action.UseAbility((IUseableCharacter)Active.AbilityToUse, touchedCell.FirstCharacter);
	                else if(Active.AbilityToUse is IUseableCell)
		                Action.UseAbility((IUseableCell)Active.AbilityToUse, touchedCell);
	                else if(Active.AbilityToUse is IUseableCellList)
                        Action.UseAbility((IUseableCellList)Active.AbilityToUse, Active.AirSelection.IsEnabled ? Active.AirSelection.HexCells : Active.HexCells);
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

	            if (!touchedCell.IsEmpty) Action.Select(touchedCell.FirstCharacter);
	            else
	            {
		            Action.Deselect();
		            Active.SelectDrawnCell(touchedCell).AddHighlight(Highlights.BlackTransparent);
	            }
            }
        }
        public static bool IsPointerOverUiObject()
        {
            var eventDataCurrentPosition =
                new PointerEventData(EventSystem.current) {position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)};
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
        public static void ShowHelpHexCells(List<DrawnHexCell> cells) => cells.ForEach(c => c.AddHighlight(Highlights.BlueTransparent));
        public void HideHelpHexCells() => RemoveHighlightsOfColor(Highlights.BlueTransparent);
	}
}
