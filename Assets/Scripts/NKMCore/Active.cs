using System;
using System.Collections.Generic;
using NKMCore.Hex;
using NKMCore.Templates;

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
		public readonly AirSelection AirSelection;
		public GamePlayer GamePlayer;
		public Ability AbilityToUse;
		public Character SelectedCharacterToPlace;
		public Character Character;
		public HexCell SelectedCell;
	
		public readonly List<HexCell> MoveCells = new List<HexCell>();
		public event Delegates.CharacterD AfterCharacterSelect;
		public event Delegates.Void AfterDeselect;
		public event Delegates.Void AfterCancelPlacingCharacter;
		public event Delegates.Void AfterClean;
		public event Delegates.CellList BeforeMoveCellsRemoved;
		public event Delegates.Cell AfterMoveCellAdded;
		public event Delegates.CharacterCellHashSet AfterCharacterSelectPrepare;
		public event Delegates.CellHashSet AfterCharacterPlacePrepare;
		public event Delegates.AbilityHashSet AfterAbilityPrepare;
	
		public HashSet<HexCell> HexCells { get; private set; }

		public bool IsActiveUse => !(AbilityToUse == null && SelectedCharacterToPlace == null);

		public void Select(Character character)
		{
			Clean();
			Character = character;
			AfterCharacterSelect?.Invoke(character);
		
			if (!CanTakeAction(character)) return;
		
			Prepare(character.GetPrepareBasicAttackCells());
			AdditionalPrepare(character.GetPrepareBasicMoveCells());
			
			AfterCharacterSelectPrepare?.Invoke(character, HexCells);
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
			if (AbilityToUse != null) AbilityToUse.Cancel();
			else if (SelectedCharacterToPlace != null) CancelPlacingCharacter();
			else
			{
				Deselect();
				SelectedCell = null;
			}

		}

		private void CancelPlacingCharacter()
		{
			SelectedCharacterToPlace = null;
			AfterCancelPlacingCharacter?.Invoke();
		}

		private void Prepare(Ability a)
		{
			if(a is IUseableCellList || a is IUseableCell || a is IUseableCharacter) AbilityToUse = a;
		}

		public void PrepareToPlaceCharacter(IEnumerable<HexCell> cellRange)
		{
			Prepare(cellRange);
			AfterCharacterPlacePrepare?.Invoke(HexCells);
		}
		private void Prepare(IEnumerable<HexCell> cellRange) => HexCells = new HashSet<HexCell>(cellRange);
		private void AdditionalPrepare(IEnumerable<HexCell> cellRange) => HexCells.UnionWith(cellRange);

		public void PrepareAirSelection(Ability a, IEnumerable<HexCell> cellRange, AirSelection.SelectionShape shape, int radius)
		{
            Prepare(a, cellRange);
            AirSelection.Enable(shape, radius);
		}

		public void Prepare(Ability a, IEnumerable<HexCell> cellRange)
		{
			Prepare(a);
			Prepare(cellRange);
			AfterAbilityPrepare?.Invoke(a, HexCells);
		}

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

		public void Reset()
		{
			if (IsActiveUse || Turn.IsDone) Deselect();
			SelectedCharacterToPlace = null;
			SelectedCell = null;
			if (AirSelection.IsEnabled) AirSelection.Disable();
			Clean();
		}

		private void Clean()
		{
			RemoveMoveCells();
			if(AirSelection.IsEnabled) AirSelection.Disable();
			AbilityToUse = null;
			HexCells = null;
			AfterClean?.Invoke();
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

		public void Select<T>(SelectableProperties<T> props) => _game.Selectable.Select(props);
		public bool CanSpawn(Character character, HexCell cell) => cell.IsFreeToStand && cell.IsSpawnFor(character.Owner, _game);
	}
}

