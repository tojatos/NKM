using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore;
using NKMCore.Abilities.Ononoki_Yotsugi;
using NKMCore.Abilities.Roronoa_Zoro;
using NKMCore.Abilities.Ryuko_Matoi;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.HexCellEffects;
using NKMCore.Templates;
using Unity.Animations;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity.Hex
{
	public class HexMapDrawer : SingletonMonoBehaviour<HexMapDrawer>
	{
		private Game _game;
		private Active Active => _game.Active;
		public DrawnHexCell CellPrefab;
		public List<DrawnHexCell> Cells;
		private readonly Dictionary<Character, GameObject> _characterObjects = new Dictionary<Character, GameObject>();
        public static readonly Dictionary<Character, GameObject> Dims = new Dictionary<Character, GameObject>();

		public GameObject GetCharacterObject(Character character)
		{
			_characterObjects.TryGetValue(character, out GameObject value);
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
                GetCharacterObject(character).transform.parent = SelectDrawnCell(cell).transform;
                AnimationPlayer.Add(new Destroy(SelectDrawnCell(cell).gameObject.GetComponent<LineRenderer>())); //Remove the line
                AnimationPlayer.Add(new MoveTo(GetCharacterObject(character).transform,
                    GetCharacterObject(character).transform.parent.transform.TransformPoint(0, 10, 0), 0.13f));

            };
            _game.HexMap.AfterCharacterPlace += (character, cell) => Spawner.Instance.Spawn(SelectDrawnCell(cell), character);
            Active.BeforeMoveCellsRemoved += cells => SelectDrawnCells(cells).ForEach(c => Destroy(c.gameObject.GetComponent<LineRenderer>()));
            Active.AfterMoveCellAdded += hexcell =>
            {
                //Draw a line between two hexcell centres

                //Check for component in case of Zoro's Lack of Orientation
                DrawnHexCell cell = SelectDrawnCell(hexcell);
                LineRenderer lRend = cell.gameObject.GetComponent<LineRenderer>() != null
                    ? cell.gameObject.GetComponent<LineRenderer>()
                    : cell.gameObject.AddComponent<LineRenderer>();
                lRend.SetPositions(new[]
                {
                    SelectDrawnCell(Active.MoveCells.SecondLast()).transform.position + Vector3.up * 20,
                    cell.transform.position + Vector3.up * 20
                });
                lRend.material = new Material(Shader.Find("Standard")) {color = Color.black};
                lRend.startColor = Color.black;
                lRend.endColor = Color.black;
                lRend.widthMultiplier = 2;
            };
			Active.AirSelection.AfterEnable += set => SelectDrawnCells(set).ForEach(c => c.AddHighlight(Highlights.BlueTransparent));
			Active.AirSelection.AfterCellsSet += set =>
			{
				RemoveHighlights();
				if (_game.Active.HexCells != null && set != null)
				{
					_game.Active.HexCells.ToList().ForEach(c =>
					{
						if (set.All(ac => ac != c))
						{
							SelectDrawnCell(c).AddHighlight(Highlights.BlueTransparent);
						}
					});
				}

				set?.ToList().ForEach(c => SelectDrawnCell(c).AddHighlight(Highlights.RedTransparent));
			};
			Active.AfterAbilityPrepare += (ability, list) =>
			{
				RemoveHighlights();
				SelectDrawnCells(list).ForEach(c => c.AddHighlight(Highlights.RedTransparent));
			};
			Active.AfterCharacterSelectPrepare += (character, list) =>
			{
				SelectDrawnCells(list.Distinct()).ForEach(c =>
					c.AddHighlight(!c.HexCell.IsEmpty && character.CanBasicAttack(c.HexCell.FirstCharacter)
						? Highlights.RedTransparent
						: Highlights.GreenTransparent));
			};
			Active.AfterCharacterPlacePrepare += set => SelectDrawnCells(set).ForEach(c => c.AddHighlight(Highlights.RedTransparent));
			Active.AfterCancelPlacingCharacter += () => RemoveHighlights();
			Active.AfterClean += () => RemoveHighlights();
			_game.AfterCellSelect += touchedCell =>
			{
				if (Active.SelectedCharacterToPlace != null) return;
				if (Active.HexCells?.Contains(touchedCell) == true) return;
				if (Active.AbilityToUse != null) return;
				//possibility of highlighting with control pressed
				if (!Input.GetKey(KeyCode.LeftControl)) RemoveHighlights();
				if (touchedCell.IsEmpty) SelectDrawnCell(touchedCell).AddHighlight(Highlights.BlackTransparent);
			};
			_game.HexMap.AfterCellEffectCreate += effect =>
			{
				if (effect is Conflagration)
					SelectDrawnCell(effect.ParentCell).AddEffectHighlight(effect.Name);
				Unity.UI.HexCellUI.Effects.Instance.UpdateButtons(Active.SelectedCell);
			};
			_game.HexMap.AfterCellEffectRemove += effect =>
			{
				if (effect is Conflagration)
					SelectDrawnCell(effect.ParentCell).RemoveEffectHighlight(effect.Name);
				Unity.UI.HexCellUI.Effects.Instance.UpdateButtons(Active.SelectedCell);
			};


		}

		public void AddTriggers(Ability ability)
		{
			if (ability is UrbCrunch)
			{
				((UrbCrunch) ability).AfterCrunch += list =>
				{
					//repaint crushed cells
					List<HexCell> normalCells = list.FindAll(c => c.Type == HexCell.TileType.Normal);
					SelectDrawnCells(normalCells).ForEach(c => c.Color = Color.white); //TODO: set the color depending on maps normal cell color
					TriangulateCells();
				};
			}

			if (ability is OniGiri)
			{
				((OniGiri) ability).AfterOniGiriPrepare += list =>
				{
					//Show highlights on move cells
					list.ForEach(c =>
					{
						HexDirection direction = ability.ParentCharacter.ParentCell.GetDirection(c);
						HexCell moveCell = c.GetCell(direction, 2);
						SelectDrawnCell(moveCell).AddHighlight(Highlights.BlueTransparent);
					});
				};
			}

			if (ability is FiberDecapitation)
			{
				var ab = (FiberDecapitation) ability;
				ab.AfterPrepare += list =>
				{
					//Show highlights on move cells
					//TODO: redundant code
					list.ForEach(c =>
					{
						HexDirection direction = ability.ParentCharacter.ParentCell.GetDirection(c);
						HexCell moveCell = c.GetCell(direction, FiberDecapitation.TargetCellOffset);
						SelectDrawnCell(moveCell).AddHighlight(Highlights.BlueTransparent);
					});
				};
			}
		}

		public void CreateMap(HexMap hexMap)
		{
			HexMesh = GetComponentInChildren<HexMesh>();
			HexMesh.Init();
			hexMap.Cells.ForEach(CreateCell);

			TriangulateCells();
		}

		private void TriangulateCells()
		{
			HexMesh.Triangulate(Cells);
		}

		private void CreateCell(HexCell hexCell)
		{
			Vector3 position;
			position.x = (hexCell.Coordinates.X + hexCell.Coordinates.Z * 0.5f) * (HexMetrics.InnerRadius * 2f);
			position.y = 0f;
			position.z = hexCell.Coordinates.Z * (HexMetrics.OuterRadius * 1.5f);

			DrawnHexCell cell = Instantiate(CellPrefab, transform, false);
			Cells.Add(cell);
			cell.HexCell = hexCell;
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
					_game.Active.AirSelection.HexCells = new HashSet<HexCell> { cellPointed };
				}
			}

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
								Destroy(SelectDrawnCell(_game.Active.MoveCells[i]).gameObject
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
				if (cellPointed != null) _game.TouchCell(cellPointed);
			}

			if (Input.GetMouseButtonDown(1))
			{
				_game.Action.Cancel();
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

		public List<DrawnHexCell> SelectDrawnCells(IEnumerable<HexCell> cells) => cells.Select(SelectDrawnCell).ToList();
		public DrawnHexCell SelectDrawnCell(HexCell cell) =>
			Cells.FirstOrDefault(g => g.HexCell == cell);

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
