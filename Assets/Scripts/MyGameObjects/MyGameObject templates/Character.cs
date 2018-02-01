using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Hex;
using MyGameObjects.Abilities;
using MyGameObjects.Effects;
using UIManagers;
using UnityEngine;
namespace MyGameObjects.MyGameObject_templates
{
	public abstract class Character : MyGameObject
	{
		protected Character()
		{
			IsOnMap = false;
			TookActionInPhaseBefore = true;
			HasUsedBasicAttackInPhaseBefore = false;
			HasUsedBasicMoveInPhaseBefore = false;
			HasUsedNormalAbilityInPhaseBefore = false;
			HasUsedUltimatumAbilityInPhaseBefore = false;
			Effects = new List<Effect>();
			JustBeforeFirstAction += () => Active.Turn.CharacterThatTookActionInTurn = this;
		}
		public Stat HealthPoints;
		public Stat AttackPoints;
		public Stat BasicAttackRange;
		public Stat Speed;
		public Stat PhysicalDefense;
		public Stat MagicalDefense;

		public int DeathTimer { get; private set; }

		private bool CanAttackAllies => Abilities.Any(a => a.OverridesFriendAttack);
		public List<Ability> Abilities{ get; private set; }
		public List<Effect> Effects { get; }
		public string Description { get; protected set; }
		public string Quote { get; protected set; }
		public string Author { get; protected set; }
		public FightType Type;
		public GameObject CharacterObject { get; set; }
		public HexCell ParentCell { get; set; }
//		public Item ActiveItem { get; set; }
		public bool IsOnMap { get; set; }

		public bool HasUsedBasicMoveInPhaseBefore { private get; set; }
		public bool HasUsedBasicAttackInPhaseBefore { private get; set; }
		public bool HasUsedNormalAbilityInPhaseBefore { private get; set; }
		public bool HasUsedUltimatumAbilityInPhaseBefore { private get; set; }

		private bool CanUseBasicMove => !HasUsedBasicMoveInPhaseBefore && !HasUsedUltimatumAbilityInPhaseBefore;
		private bool CanUseBasicAttack => !HasUsedBasicAttackInPhaseBefore && !HasUsedNormalAbilityInPhaseBefore && !HasUsedUltimatumAbilityInPhaseBefore;
		public bool CanUseNormalAbility => !HasUsedNormalAbilityInPhaseBefore && !HasUsedBasicAttackInPhaseBefore && !HasUsedUltimatumAbilityInPhaseBefore;
		public bool CanUseUltimatumAbility => !(HasUsedUltimatumAbilityInPhaseBefore || Active.Turn.CharacterThatTookActionInTurn == this);

		public bool TookActionInPhaseBefore { get; set; }
		public bool IsAlive => HealthPoints.Value > 0;

		public bool CanTakeAction => !(TookActionInPhaseBefore || !IsAlive || Active.Turn.CharacterThatTookActionInTurn != null && Active.Turn.CharacterThatTookActionInTurn != this);


		public void BasicMove(HexCell targetCell)
		{
			//Active.Turn.CharacterThatTookActionInTurn = this;
			HasUsedBasicMoveInPhaseBefore = true;
			Move(targetCell);
		}
		public void Move(HexCell targetCell)
		{
			ParentCell.CharacterOnCell = null;
			ParentCell = targetCell;
			targetCell.CharacterOnCell = this;
			CharacterObject.transform.parent = targetCell.transform;
			//Deselect();
			CharacterObject.GetComponent<Animation>().Play("Fade Out Character");
			//yield return new WaitForSeconds(0.2f); TODO
			CharacterObject.transform.localPosition = new Vector3(0, 10, 0);
			CharacterObject.GetComponent<Animation>().Play("Fade In Character");
			//yield return new WaitForSeconds(0.2f); TODO
			//Active.HexCells = null;
			//Active.Action = Action.None;
			//Select();

		}
		public void BasicAttack(Character attackedCharacter)
		{
			if (attackedCharacter.Abilities.All(a => a.BeforeParentBasicAttacked(this)))
			{
				if (attackedCharacter.Owner == Owner)
				{
					if (Abilities.Any(a => a.OverridesFriendAttack))
					{
						if (Abilities.Count(a => a.OverridesFriendAttack) > 1)
							throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać atak!");

						Abilities.Single(a => a.OverridesFriendAttack).AttackFriend(attackedCharacter);
					}
				}
				else
				{
					if (Abilities.Any(a => a.OverridesEnemyAttack))
					{
						if (Abilities.Count(a => a.OverridesEnemyAttack) > 1)
							throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać atak!");

						Abilities.Single(a => a.OverridesEnemyAttack).AttackEnemy(attackedCharacter);
					}
					else
					{
						Attack(attackedCharacter, AttackType.Physical, AttackPoints.Value);
					}
				}
			}

			HasUsedBasicAttackInPhaseBefore = true;
			//HasUsedNormalAbilityInPhaseBefore = true;
		}
		public void Attack(Character attackedCharacter, AttackType attackType, int atkPoints)
		{
			var defense = 0;//isMagic ? attackedCharacter.MagicalDefense : attackedCharacter.PhysicalDefense;
			switch (attackType)
			{
				case AttackType.Physical:
					defense = attackedCharacter.PhysicalDefense.Value;
					break;
				case AttackType.Magical:
					defense = attackedCharacter.MagicalDefense.Value;
					break;
				case AttackType.True:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(attackType), attackType, null);
			}

			var damage = atkPoints;
			DamageModifier(attackedCharacter, ref damage);
			damage -= defense;
			damage = damage < 0 ? 0 : damage;
			TrueDamageModifier(attackedCharacter, ref damage);
			MessageLogger.Log(string.Format("{0} atakuje {1}", this.FormattedFirstName(), attackedCharacter.FormattedFirstName()), false);
			if (damage > 0)
			{
				attackedCharacter.Damage(ref damage);
				MessageLogger.Log(string.Format(" zadając <color=red><b>{0}</b></color> obrażeń!", damage));
				if (!attackedCharacter.IsAlive) OnEnemyKill();
			}
			else
			{
				MessageLogger.Log(", ale nie zadaje żadnych obrażeń!");
			}
			attackedCharacter.AfterBeingAttacked();
			OnDamage(attackedCharacter, damage);
		}
		private void Damage(ref int damage)
		{
			foreach (var a in Abilities)
			{
				a.BeforeParentDamage(ref damage); //TODO event?
			}

			HealthPoints.Value -= damage;
		}
		public void RemoveIfDead()
		{
			if (IsAlive) return;

			MessageLogger.Log(string.Format("{0} umiera!", this.FormattedFirstName()));
			RemoveFromMap();
			DeathTimer = 0;
			if (Active.CharacterOnMap == this) Deselect();
		}

		private void AfterBeingAttacked()
		{
			RemoveIfDead();
		}
		private void RemoveFromMap()
		{
			ParentCell.CharacterOnCell = null;
			ParentCell = null;
			IsOnMap = false;
			UnityEngine.Object.Destroy(CharacterObject);
		}
		private void DamageModifier(Character targetCharacter, ref int damage)
		{
			foreach (var ability in Abilities)
			{
				ability.DamageModifier(targetCharacter, ref damage);
			}
		}
		private void TrueDamageModifier(Character targetCharacter, ref int damage)
		{
			foreach (var ability in Abilities)
			{
				ability.DamageModifier(targetCharacter, ref damage);
			}
		}
		private void OnEnemyKill()
		{
			Abilities.ForEach(a => a.OnEnemyKill());
		}
		private void OnDamage(Character targetCharacter, int damageDealt)
		{
			Abilities.ForEach(a => a.OnDamage(targetCharacter, damageDealt));
		}
		private void PrepareAttackAndMove()
		{
			Game.HexMapDrawer.RemoveAllHighlights();
			if (Active.GamePlayer != Owner)
			{
				MessageLogger.DebugLog("Nie jesteś właścicielem! Wara!");
				return;
			}

			bool isPreparationSuccessful;
			if (!CanTakeAction || !CanUseBasicMove && !CanUseBasicAttack)
			{
				MessageLogger.DebugLog("Ta postać nie może się ruszać ani atakować!");
				return;
			}

			if (!CanUseBasicMove && CanUseBasicAttack)
			{
				isPreparationSuccessful = Active.Prepare(Action.AttackAndMove, GetPrepareBasicAttackCells());
			}
			else if (CanUseBasicMove && !CanUseBasicAttack)
			{
				isPreparationSuccessful = Active.Prepare(Action.AttackAndMove, GetPrepareMoveCells());
			}
			else
			{
				var p1 = Active.Prepare(Action.AttackAndMove, GetPrepareMoveCells());
				var p2 = Active.Prepare(Action.AttackAndMove, GetPrepareBasicAttackCells(), true);
				isPreparationSuccessful = p1 || p2;
			}
			if (!isPreparationSuccessful)
			{
				return;
			}

			Active.HexCells.ForEach(c=>c.ToggleHighlight(HiglightColor.Red));
		}
		private List<HexCell> GetPrepareMoveCells()
		{
			if (Effects.Any(e => e.GetType() == typeof(MovementDisability)))
			{
				MessageLogger.DebugLog("Postać posiada efekt uniemożliwiający ruch!");
				return Enumerable.Empty<HexCell>().ToList();
			}

			return GetMoveCells();
		}
		private List<HexCell> GetPrepareBasicAttackCells()
		{
			if (Effects.Any(e => e.GetType() == typeof(BasicAttackInability)))
			{
				MessageLogger.DebugLog("Postać posiada efekt uniemożliwiający atak!");
				return Enumerable.Empty<HexCell>().ToList();
			}

			var cellRange = GetBasicAttackCells();
			if (CanAttackAllies)
			{
					cellRange.RemoveNonCharacters();
			}
			else
			{
					cellRange.RemoveNonEnemies();
			}
			return cellRange;
		}
		public List<HexCell> GetBasicAttackCells()
		{
			if (Abilities.Any(a => a.OverridesGetBasicAttackCells))
			{
				if (Abilities.Count(a => a.OverridesGetBasicAttackCells) > 1)
					throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać komórki podstawowego ataku!");

				return Abilities.Single(a => a.OverridesGetBasicAttackCells).GetBasicAttackCells();
			}

			List<HexCell> cellRange;
			switch (Type)
			{
				case FightType.Ranged:
					cellRange = ParentCell.GetNeighbors(BasicAttackRange.Value, false, false, true);
					break;
				case FightType.Melee:
					cellRange = ParentCell.GetNeighbors(BasicAttackRange.Value, true, false, true);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return cellRange;
		}
		public List<HexCell> GetMoveCells()
		{
			if (Abilities.Any(a => a.OverridesGetMoveCells))
			{
				if (Abilities.Count(a => a.OverridesGetMoveCells) > 1)
					throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać komórki ruchu!");

				return Abilities.Single(a => a.OverridesGetMoveCells).GetMoveCells();
			}

			var cellRange = ParentCell.GetNeighbors(Speed.Value, true, true);
			cellRange.RemoveAll(cell => cell.CharacterOnCell != null); //we don't want to allow stepping into our characters!
			return cellRange;
		}
		public void Heal(Character targetCharacter, int amount)
		{
			var hpBeforeHeal = targetCharacter.HealthPoints.Value;
			targetCharacter.HealthPoints.Value += amount;
			var hpAfterHeal = targetCharacter.HealthPoints.Value;
			var diff = hpAfterHeal - hpBeforeHeal;
			MessageLogger.Log(targetCharacter != this
				? $"{this.FormattedFirstName()} ulecza {targetCharacter.FormattedFirstName()} o <color=blue><b>{diff}</b></color> punktów życia!"
				: $"{this.FormattedFirstName()} ulecza się o <color=blue><b>{diff}</b></color> punktów życia!");
		}
		public void OnPhaseFinish()
		{
			if (IsOnMap)
			{
				HasUsedBasicAttackInPhaseBefore = false;
				HasUsedBasicMoveInPhaseBefore = false;
				HasUsedNormalAbilityInPhaseBefore = false;
				HasUsedUltimatumAbilityInPhaseBefore = false;
				TookActionInPhaseBefore = false;
				Abilities?.ForEach(a => a.OnPhaseFinish());
				//if (Effects != null)
				//{
				//	Effects.ForEach(e => e.OnPhaseFinish());
				//	Effects.Where(e => e.CurrentCooldown <= 0).ToList().ForEach(e =>
				//	{
				//		Effects.Remove(e);
				//		OnRemoveHandler();
				//	});
				//}
			}
			if (!IsAlive)
			{
				DeathTimer++;
			}
		}
		public delegate void OnJustBeforeFirstAction();
		public event OnJustBeforeFirstAction JustBeforeFirstAction;
		public void InvokeJustBeforeFirstAction()
		{
			JustBeforeFirstAction?.Invoke();
		}
		public void Select()
		{
			CharacterStats.Instance.UpdateCharacterStats(this);
			Active.CharacterOnMap = this;
			CharacterAbilities.Instance.UpdateButtons();
			CharacterEffects.Instance.UpdateButtons();
			var characterButtons = new List<GameObject>(CharacterAbilities.Instance.Buttons);
			characterButtons.AddRange(new List<GameObject>(CharacterEffects.Instance.Buttons));
			Active.Buttons = characterButtons;
			if (Active.GamePlayer != Owner) return;

			PrepareAttackAndMove();
		}
		public void Deselect()
		{
			CharacterStats.Instance.UpdateCharacterStats(null);
			Active.CharacterOnMap = null;
			Active.Action = Action.None;
			Active.HexCells = null;
			Game.HexMapDrawer.RemoveAllHighlights();
		}
		protected void InitiateAbilities(List<Ability> abilities)
		{
			Abilities = abilities ?? new List<Ability>();
			foreach (AbilityType type in Enum.GetValues(typeof(AbilityType)))
			{
				if (Abilities.All(a => a.Type != type))
				{
					Abilities.Add(new Empty(type));
				}
			}

			Abilities.ForEach(a => a.ParentCharacter = this);
			Abilities.ForEach(a => a.Awake());
		}
	}
	public enum FightType
	{
		Ranged,
		Melee
	}
	public enum AttackType
	{
		Physical,
		Magical,
		True
	}

}
