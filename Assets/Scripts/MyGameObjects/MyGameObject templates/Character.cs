using System;
using System.Collections.Generic;
using System.Linq;
using Animations;
using Helpers;
using Hex;
using MyGameObjects.Abilities;
using MyGameObjects.Effects;
using UIManagers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyGameObjects.MyGameObject_templates
{
	public class Character : MyGameObject
	{
		public Character(string name)
		{
			//Define basic properties
			IsOnMap = false;
			TookActionInPhaseBefore = true;
			HasUsedBasicAttackInPhaseBefore = false;
			HasUsedBasicMoveInPhaseBefore = false;
			HasUsedNormalAbilityInPhaseBefore = false;
			HasUsedUltimatumAbilityInPhaseBefore = false;
			Effects = new List<Effect>();
			
			//Add triggers to events
			JustBeforeFirstAction += () => Active.Turn.CharacterThatTookActionInTurn = this;
			AfterBeingAttacked += RemoveIfDead;
			OnEnemyKill += () => Abilities.ForEach(a => a.OnEnemyKill());
			OnDamage += (targetCharacter, damageDealt) => Abilities.ForEach(a => a.OnDamage(targetCharacter, damageDealt));
			OnDamage += (targetCharacter, damageDealt) => AnimationPlayer.Add(new Tilt(targetCharacter.CharacterObject.transform));
			OnDamage += (targetCharacter, damageDealt) => AnimationPlayer.Add(new ShowInfo(targetCharacter.CharacterObject.transform, damageDealt.ToString(), Color.red));
			OnHeal += (targetCharacter, valueHealed) => AnimationPlayer.Add(new ShowInfo(targetCharacter.CharacterObject.transform, valueHealed.ToString(), Color.blue));

			//Define database properties
			SqliteRow characterData = GameData.Conn.GetCharacterData(name);

			Name = name;
			AttackPoints = new Stat(this, StatType.AttackPoints, int.Parse(characterData.GetValue("AttackPoints")));
			HealthPoints = new Stat(this, StatType.HealthPoints, int.Parse(characterData.GetValue("HealthPoints")));
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, int.Parse(characterData.GetValue("BasicAttackRange")));
			Speed = new Stat(this, StatType.Speed, int.Parse(characterData.GetValue("Speed")));
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, int.Parse(characterData.GetValue("PhysicalDefense")));
			MagicalDefense = new Stat(this, StatType.MagicalDefense, int.Parse(characterData.GetValue("MagicalDefense")));

			Type = characterData.GetValue("FightType").ToFightType();

			Description = characterData.GetValue("Description");
			Quote = characterData.GetValue("Quote");
			Author = characterData.GetValue("Author.Name");
			
			//Create and initiate abilites
			IEnumerable<string> abilityClassNames = GameData.Conn.GetAbilityClassNames(name);
			var abilityNamespaceName = "Abilities." + name.Replace(' ', '_');
			List<Ability> abilities = Spawner.Create(abilityNamespaceName, abilityClassNames).ToList().ConvertAll(x=>x as Ability);
			InitiateAbilities(abilities);

		}

		#region Properties
		public readonly Stat HealthPoints;
		public readonly Stat AttackPoints;
		public readonly Stat BasicAttackRange;
		public readonly Stat Speed;
		public readonly Stat PhysicalDefense;
		public readonly Stat MagicalDefense;

		public int DeathTimer { get; private set; }

		private bool CanAttackAllies => Abilities.Any(a => a.OverridesFriendAttack);
		public List<Ability> Abilities{ get; private set; }
		public List<Effect> Effects { get; }
		public string Description { get; }
		public string Quote { get; }
		public string Author { get; }
		public readonly FightType Type;
		public GameObject CharacterObject { get; set; }
		public HexCell ParentCell { get; set; }
//		public Item ActiveItem { get; set; }
		public bool IsOnMap { get; set; }

		public bool HasUsedBasicMoveInPhaseBefore { private get; set; }
		public bool HasUsedBasicAttackInPhaseBefore { private get; set; }
		public bool HasUsedNormalAbilityInPhaseBefore { private get; set; }
		public bool HasUsedUltimatumAbilityInPhaseBefore { private get; set; }

		private bool CanUseBasicMove => !HasUsedBasicMoveInPhaseBefore && !HasUsedUltimatumAbilityInPhaseBefore;
		private bool CanUseBasicAttack => !HasUsedBasicAttackInPhaseBefore && !HasUsedNormalAbilityInPhaseBefore && !HasUsedUltimatumAbilityInPhaseBefore && !HasBasicAttackInabilityEffect;
		public bool CanUseNormalAbility => !HasUsedNormalAbilityInPhaseBefore && !HasUsedBasicAttackInPhaseBefore && !HasUsedUltimatumAbilityInPhaseBefore;
		public bool CanUseUltimatumAbility => !(HasUsedUltimatumAbilityInPhaseBefore || Active.Turn.CharacterThatTookActionInTurn == this);

		public bool TookActionInPhaseBefore { get; set; }
		public bool IsAlive => HealthPoints.Value > 0;

		private bool IsStunned => Effects.Any(e => e.GetType() == typeof(Stun));
		private bool CanMove => Effects.All(e => e.GetType() != typeof(MovementDisability));
		private bool HasBasicAttackInabilityEffect => Effects.Any(e => e.GetType() == typeof(BasicAttackInability));
		
		public bool CanTakeAction => !(TookActionInPhaseBefore || !IsAlive || Active.Turn.CharacterThatTookActionInTurn != null && Active.Turn.CharacterThatTookActionInTurn != this || IsStunned);

		#endregion

		public void MoveTo(HexCell targetCell)
		{
			ParentCell.CharacterOnCell = null;
			ParentCell = targetCell;
			targetCell.CharacterOnCell = this;
			CharacterObject.transform.parent = targetCell.transform;
			AnimationPlayer.Add(new MoveTo(CharacterObject.transform, CharacterObject.transform.parent.transform.TransformPoint(0,10,0), 0.13f));
		}
		public void BasicMove(List<HexCell> cellPath)
		{
			HasUsedBasicMoveInPhaseBefore = true;
			Move(cellPath);
		}

		private void Move(IList<HexCell> cellPath)
		{
			cellPath.RemoveAt(0);//Remove parent cell
			foreach (HexCell nextCell in cellPath)
			{
				Object.Destroy(nextCell.gameObject.GetComponent<LineRenderer>());//Remove the line
				MoveTo(nextCell);
			}
		}

		public void BasicAttack(Character attackedCharacter)
		{
			int damage = AttackPoints.Value;
			BeforeBasicAttack?.Invoke(attackedCharacter, ref damage);
			if (attackedCharacter.Abilities.All(a => a.BeforeParentBasicAttacked(this)))
			{
				if (attackedCharacter.Owner == Owner)
				{
					if (Abilities.Any(a => a.OverridesFriendAttack))
					{
						if (Abilities.Count(a => a.OverridesFriendAttack) > 1)
							throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać atak!");

						Abilities.Single(a => a.OverridesFriendAttack).AttackFriend(attackedCharacter, damage);
					}
				}
				else
				{
					if (Abilities.Any(a => a.OverridesEnemyAttack))
					{
						if (Abilities.Count(a => a.OverridesEnemyAttack) > 1)
							throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać atak!");

						Abilities.Single(a => a.OverridesEnemyAttack).AttackEnemy(attackedCharacter, damage);
					}
					else
					{
						Attack(attackedCharacter, AttackType.Physical, damage);
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
			if (damage > 0)
			{
				attackedCharacter.Damage(ref damage);
                MessageLogger.Log($"{this.FormattedFirstName()} atakuje {attackedCharacter.FormattedFirstName()}, zadając <color=red><b>{damage}</b></color> obrażeń!");
				if (!attackedCharacter.IsAlive) OnEnemyKill?.Invoke();
			}
			else
			{
                MessageLogger.Log($"{this.FormattedFirstName()} atakuje {attackedCharacter.FormattedFirstName()}, ale nie zadaje żadnych obrażeń!");
			}
			OnDamage?.Invoke(attackedCharacter, damage);
			attackedCharacter.AfterBeingAttacked();
		}
		private void Damage(ref int damage)
		{
			foreach (Ability a in Abilities)
			{
				a.BeforeParentDamage(ref damage); //TODO event?
			}

			BeforeParentDamage?.Invoke();
			HealthPoints.Value -= damage;
			OnParentDamage?.Invoke(damage);
		}
		public void RemoveIfDead()
		{
			if (IsAlive) return;

			MessageLogger.Log($"{this.FormattedFirstName()} umiera!");
			RemoveFromMap();
			DeathTimer = 0;
			if (Active.CharacterOnMap == this) Deselect();
		}
		public delegate void VoidDelegate();
		public delegate void IntDelegate(int value);
		public delegate void CharacterIntDelegate(Character targetCharacter, int value);
		public delegate void CharacterRefIntDelegate(Character targetCharacter, ref int value);
		public event VoidDelegate JustBeforeFirstAction;
		public event VoidDelegate AfterBeingAttacked;
		public event VoidDelegate OnEnemyKill;
		public event VoidDelegate BeforeParentDamage;
		public event CharacterRefIntDelegate BeforeBasicAttack;
		public event IntDelegate OnParentDamage;
		public event CharacterIntDelegate OnDamage;
		public event CharacterIntDelegate OnHeal;
		public void InvokeJustBeforeFirstAction() => JustBeforeFirstAction?.Invoke();

		private void RemoveFromMap()
		{
			ParentCell.CharacterOnCell = null;
			ParentCell = null;
			IsOnMap = false;
//			Object.Destroy(CharacterObject);//TODO: enqueue that as animation
			AnimationPlayer.Add(new Destroy(CharacterObject));
		}
		private void DamageModifier(Character targetCharacter, ref int damage)
		{
			foreach (Ability ability in Abilities)
			{
				ability.DamageModifier(targetCharacter, ref damage);
			}
		}
		private void TrueDamageModifier(Character targetCharacter, ref int damage)
		{
			foreach (Ability ability in Abilities)
			{
				ability.DamageModifier(targetCharacter, ref damage);
			}
		}
		private void PrepareAttackAndMove()
		{
//			Game.HexMapDrawer.RemoveAllHighlights();
//			Active.Clean();
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
				//there are no cells to move or attack
				return;
			}

			Active.HexCells.ForEach(c=>c.ToggleHighlight(c.CharacterOnCell!=null ? HiglightColor.Red: HiglightColor.GreenTransparent));
			Active.RemoveMoveCells();
			Active.MoveCells.Add(ParentCell);
			
		}
		private List<HexCell> GetPrepareMoveCells()
		{
			if (CanMove) return GetMoveCells();
			MessageLogger.DebugLog("Postać posiada efekt uniemożliwiający ruch!");
			return Enumerable.Empty<HexCell>().ToList();

		}
		private List<HexCell> GetPrepareBasicAttackCells()
		{
//			if (Effects.Any(e => e.GetType() == typeof(BasicAttackInability)))
//			{
//				MessageLogger.DebugLog("Postać posiada efekt uniemożliwiający atak!");
//				return Enumerable.Empty<HexCell>().ToList();
//			}

			List<HexCell> cellRange = GetBasicAttackCells();
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

			List<HexCell> cellRange = ParentCell.GetNeighbors(Speed.Value, true, true);
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
			OnHeal?.Invoke(targetCharacter, diff);
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
		
		public void Select()
		{
			Active.Clean();
			CharacterStats.Instance.UpdateCharacterStats(this);
			Active.CharacterOnMap = this;
			CharacterAbilities.Instance.UpdateButtons();
			CharacterEffects.Instance.UpdateButtons();
			List<GameObject> characterButtons = new List<GameObject>(CharacterAbilities.Instance.Buttons);
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
			Active.RemoveMoveCells();
		}

		private void InitiateAbilities(List<Ability> abilities)
		{
			Abilities = new List<Ability>();
			foreach (AbilityType type in Enum.GetValues(typeof(AbilityType)))
			{
				var abilitiesOfType = abilities.Count(a => a.Type == type);
				switch (abilitiesOfType)
				{
					case 0:
						Abilities.Add(new Empty(type));
						break;
					case 1:
						Abilities.Add(abilities.First(a=>a.Type == type));
						break;
					default:
						Abilities.AddRange(abilities.FindAll(a=>a.Type==type));
						break;
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
