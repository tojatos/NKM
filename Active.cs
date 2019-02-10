using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity;
using Unity.Hex;

namespace NKMCore
{
	/// <summary>
	/// Main utility class.
	/// Contains information about active game state.
	/// </summary>
	public class Active
	{
		private readonly Game _game;
		public Active(Game game)
		{
			_game = game;
			Phase = new Phase();
			Turn = new Turn(game);
			AirSelection = new AirSelection(game);
		}

		public readonly Turn Turn;
		public readonly Phase Phase;
		public AirSelection AirSelection { get; }

		public GamePlayer GamePlayer;
		public Ability AbilityToUse;
		public Character SelectedCharacterToPlace;
		public Character Character;
		public HexCell SelectedCell;
		public readonly List<HexCell> MoveCells = new List<HexCell>();
		public event Delegates.CharacterD AfterSelect;
		public event Delegates.Void AfterDeselect;
		public event Delegates.CellList BeforeMoveCellsRemoved;
		public event Delegates.Cell AfterMoveCellAdded;
	
		public List<HexCell> HexCells { get; private set; }

		public bool IsActiveUse => !(AbilityToUse == null && SelectedCharacterToPlace == null);

		public void Reset()
		{
			if (IsActiveUse || Turn.IsDone) Deselect();
			AbilityToUse = null;
			HexCells = null;
			SelectedCharacterToPlace = null;
			SelectedCell = null;
			if (AirSelection.IsEnabled) AirSelection.Disable();
		}

		public void Select(Character character)
		{
			Clean();
			Character = character;
			AfterSelect?.Invoke(character);
		
			if (!CanTakeAction(character)) return;
		
			Prepare(character.GetPrepareBasicAttackCells());
			Prepare(character.GetPrepareBasicMoveCells(), true);

			HexCells.Distinct().ToList().ForEach(c =>
				SelectDrawnCell(c).AddHighlight(!c.IsEmpty && character.CanBasicAttack(c.FirstCharacter)
					? Highlights.RedTransparent : Highlights.GreenTransparent));
			RemoveMoveCells();
			MoveCells.Add(character.ParentCell);
		}
		public void Deselect()
		{
			Character = null;
			HexCells = null;
			RemoveMoveCells();
			AfterDeselect?.Invoke();
		}
		public void Cancel()
		{
			if (AbilityToUse != null)
			{
				AbilityToUse.Cancel();
				//Console.GameLog($"ABILITY CANCEL");
			}
			else if (SelectedCharacterToPlace != null)
			{
				HexMapDrawer.Instance.RemoveHighlights();
				SelectedCharacterToPlace = null;
			}
			else
			{
				Deselect();
				SelectedCell = null;
			}
		}

		public void Prepare(Ability a)
		{
			if(a is IUseableCellList || a is IUseableCell || a is IUseableCharacter) AbilityToUse = a;
		}

		private void Prepare(List<HexCell> cellRange, bool addToRange = false)
		{
			if (cellRange == null) cellRange = new List<HexCell>();
			if (!addToRange)
				HexCells = cellRange;
			else
				HexCells.AddRange(cellRange);
		}
		public void Prepare(Ability a, List<HexCell> cellRange, bool addToRange = false, bool toggleToRed = true)
		{
			Prepare(cellRange, addToRange);

			Prepare(a);
			if (!toggleToRed) return;
			HexMapDrawer.Instance.RemoveHighlights();
			SelectDrawnCells(HexCells).ForEach(c => c.AddHighlight(Highlights.RedTransparent));
		}
		public static List<DrawnHexCell> SelectDrawnCells(IEnumerable<HexCell> cells) => cells.Select(SelectDrawnCell).ToList();
		public static DrawnHexCell SelectDrawnCell(HexCell cell) =>
			HexMapDrawer.Instance.Cells.FirstOrDefault(g => g.HexCell == cell);


		public void RemoveMoveCells()
		{
			if(MoveCells==null || MoveCells.Count==0) return;
		
			BeforeMoveCellsRemoved?.Invoke(MoveCells);
			MoveCells.Clear();
		}

		public void AddMoveCell(HexCell c)
		{
			if(MoveCells==null||MoveCells.Count<1) throw new Exception("Unable to add move cell!");
		
			_game.Active.MoveCells.Add(c);
			AfterMoveCellAdded?.Invoke(c);
		
		}

		private void Clean()
		{
			RemoveMoveCells();
			if(AirSelection.IsEnabled) AirSelection.Disable();
			AbilityToUse = null;
			HexCells = null;
			HexMapDrawer.Instance.RemoveHighlights();
			HexMapDrawer.Instance.RemoveHighlightsOfColor(Highlights.BlueTransparent);
		}
		public void CleanAndTrySelecting()
		{
			Clean();	
			Select(Character);
		}
		
		public bool CanTakeAction(Character character) => 
			!(character.TookActionInPhaseBefore || !character.IsAlive ||
           Turn.CharacterThatTookActionInTurn != null &&
           Turn.CharacterThatTookActionInTurn != character || character.IsStunned ||
           GamePlayer != character.Owner);
		
		public bool CanWait(Character character) => !(character.Owner != GamePlayer || character.TookActionInPhaseBefore ||
		                         Turn.CharacterThatTookActionInTurn != null);
	}
}

