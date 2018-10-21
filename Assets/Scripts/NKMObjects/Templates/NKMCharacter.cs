using System.Collections.Generic;
using System.Linq;
using Animations;
using Extensions;
using Hex;
using JetBrains.Annotations;
using Managers;
using UI.CharacterUI;
using UnityEngine;

namespace NKMObjects.Templates
{
	public class NKMCharacter : Character
	{
		
		protected static Game Game => GameStarter.Instance.Game;
		protected static Active Active => Game.Active;
		protected static Console Console => Console.Instance;
		#region Properties
		public int DeathTimer { get; private set; }
		public GameObject CharacterObject { get; set; }
		public bool CanBasicAttack(NKMCharacter targetCharacter) =>
			(IsEnemyFor(targetCharacter) || CanAttackAllies) &&
			GetBasicAttackCells().Contains(targetCharacter.ParentCell);


		public bool CanTakeAction => !(TookActionInPhaseBefore || !IsAlive ||
		                               Active.Turn.CharacterThatTookActionInTurn != null &&
		                               Active.Turn.CharacterThatTookActionInTurn != this || IsStunned);

		public bool CanWait => !(Owner != Active.GamePlayer || TookActionInPhaseBefore ||
		                         Active.Turn.CharacterThatTookActionInTurn != null);

		#endregion
		internal NKMCharacter (string name, int id, CharacterProperties properties, List<Ability> abilities) : base(name, id, properties, abilities)
		{
			AddTriggersToEvents();
			Active.Turn.TurnFinished += character =>
			{
				if (character != this) return;
				HasFreeAttackUntilEndOfTheTurn = false;
				HasFreeMoveUntilEndOfTheTurn = false;
				HasFreeUltimatumAbilityUseUntilEndOfTheTurn = false;
			};
		}

		private void AddTriggersToEvents()
		{
			JustBeforeFirstAction += () => Active.Turn.CharacterThatTookActionInTurn = this;
			JustBeforeFirstAction += () => Console.GameLog($"ACTION TAKEN: {this}");
			HealthPoints.StatChanged += RemoveIfDead;
			AfterAttack += (targetCharacter, damage) =>
				Console.Log(
					$"{this.FormattedFirstName()} atakuje {targetCharacter.FormattedFirstName()}, zadając <color=red><b>{damage.Value}</b></color> obrażeń!");
			AfterAttack += (targetCharacter, damage) =>
				AnimationPlayer.Add(new Tilt(((NKMCharacter)targetCharacter).CharacterObject.transform));
			AfterAttack += (targetCharacter, damage) =>
				AnimationPlayer.Add(new ShowInfo(((NKMCharacter)targetCharacter).CharacterObject.transform, damage.Value.ToString(),
					Color.red));
			AfterHeal += (targetCharacter, valueHealed) =>
				AnimationPlayer.Add(new ShowInfo(((NKMCharacter)targetCharacter).CharacterObject.transform, valueHealed.ToString(),
					Color.blue));
			HealthPoints.StatChanged += () =>
			{
				if (Active.CharacterOnMap == this) MainHPBar.Instance.UpdateHPAmount(this);
			};
			OnDeath += () => Effects.Clear();
			AfterHeal += (targetCharacter, value) =>
				Console.Log(targetCharacter != this
					? $"{this.FormattedFirstName()} ulecza {targetCharacter.FormattedFirstName()} o <color=blue><b>{value}</b></color> punktów życia!"
					: $"{this.FormattedFirstName()} ulecza się o <color=blue><b>{value}</b></color> punktów życia!");

			Active.Phase.PhaseFinished += () =>
			{
			if (IsOnMap)
			{
				HasUsedBasicAttackInPhaseBefore = false;
				HasUsedBasicMoveInPhaseBefore = false;
				HasUsedNormalAbilityInPhaseBefore = false;
				HasUsedUltimatumAbilityInPhaseBefore = false;
				TookActionInPhaseBefore = false;
			}
			if (!IsAlive)
			{
				DeathTimer++;
			}
				
			};
		}


		
		public override void MoveTo(HexCell targetCell)
		{
			base.MoveTo(targetCell);
			CharacterObject.transform.parent = targetCell.transform;
			AnimationPlayer.Add(new MoveTo(CharacterObject.transform,
				CharacterObject.transform.parent.transform.TransformPoint(0, 10, 0), 0.13f));
		}
		public override void RemoveIfDead()
		{
			base.RemoveIfDead();

			if(IsAlive) return;
			Console.Log($"{this.FormattedFirstName()} umiera!");
			RemoveFromMap();
			DeathTimer = 0;
			if (Active.CharacterOnMap == this) Deselect();
		}

		public void TryToInvokeJustBeforeFirstAction()
		{
			if (Active.Turn.CharacterThatTookActionInTurn == null) InvokeJustBeforeFirstAction();
		}
		private void RemoveFromMap()
		{
			if(!IsOnMap) return;
			ParentCell.CharacterOnCell = null;
			ParentCell = null;
			IsOnMap = false;
			AnimationPlayer.Add(new Destroy(CharacterObject));
		}
		private void PrepareAttackAndMove()
		{
			if (Active.GamePlayer != Owner)
			{
				Console.DebugLog("Nie jesteś właścicielem! Wara!");
				return;
			}

			bool isPreparationSuccessful;
			if (!CanTakeAction || !CanUseBasicMove && !CanUseBasicAttack)
			{
				Console.DebugLog("Ta postać nie może się ruszać ani atakować!");
				return;
			}

			if (!CanUseBasicMove && CanUseBasicAttack)
			{
				isPreparationSuccessful = Active.Prepare(Action.AttackAndMove, GetPrepareBasicAttackCells());
			}
			else if (CanUseBasicMove && !CanUseBasicAttack)
			{
				isPreparationSuccessful = Active.Prepare(Action.AttackAndMove, GetPrepareBasicMoveCells());
			}
			else
			{
				var p1 = Active.Prepare(Action.AttackAndMove, GetPrepareBasicMoveCells());
				var p2 = Active.Prepare(Action.AttackAndMove, GetPrepareBasicAttackCells(), true);
				isPreparationSuccessful = p1 || p2;
			}
			if (!isPreparationSuccessful)
			{
				//there are no cells to move or attack
				return;
			}

			Active.HexCells.Distinct().ToList().ForEach(c =>
				c.AddHighlight(
					c.CharacterOnCell != null &&
					(c.CharacterOnCell.IsEnemyFor(Owner) || CanAttackAllies && CanUseBasicAttack && GetBasicAttackCells().Contains(c))
						? Highlights.RedTransparent
						: Highlights.GreenTransparent));
			Active.RemoveMoveCells();
			Active.MoveCells.Add(ParentCell);

		}

		public void Select()
		{
			Active.Clean();
			Stats.Instance.UpdateCharacterStats(this);
			MainHPBar.Instance.UpdateHPAmount(this);
			Active.CharacterOnMap = this;
			UI.CharacterUI.Abilities.Instance.UpdateButtons();
			UI.CharacterUI.Effects.Instance.UpdateButtons();
//			List<GameObject> characterButtons = new List<GameObject>(CharacterAbilities.Instance.Buttons);
//			characterButtons.AddRange(new List<GameObject>(CharacterEffects.Instance.Buttons));
//			Active.Buttons = characterButtons;
			if (Active.GamePlayer != Owner) return;

			PrepareAttackAndMove();
		}
		public void Deselect()
		{
			Stats.Instance.UpdateCharacterStats(null);
			Active.CharacterOnMap = null;
			Active.Action = Action.None;
			Active.HexCells = null;
			Game.HexMapDrawer.RemoveHighlights();
			Active.RemoveMoveCells();
		}


		public void MakeActionBasicAttack([NotNull] NKMCharacter target)
		{
			TryToInvokeJustBeforeFirstAction();
            BasicAttack(target);
			Console.GameLog($"BASIC ATTACK: {target}"); //logging after action to make reading rng work
		}
		public void MakeActionBasicMove([NotNull] List<HexCell> moveCells)
		{
			TryToInvokeJustBeforeFirstAction();
			BasicMove(new List<HexCell>(moveCells)); //work on a new list to log unmodified list below
			Console.GameLog($"MOVE: {string.Join("; ", moveCells.Select(p => p.Coordinates))}"); //logging after action to make reading rng work
//			AfterBasicMove?.Invoke();
			InvokeAfterBasicMove();
		}

		public override string ToString() => Name + $" ({ID})";
	}
}
