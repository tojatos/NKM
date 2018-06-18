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
		#region Properties
		public readonly Stat HealthPoints;
		public readonly Stat AttackPoints;
		public readonly Stat BasicAttackRange;
		public readonly Stat Speed;
		public readonly Stat PhysicalDefense;
		public readonly Stat MagicalDefense;

		public int DeathTimer { get; private set; }

		public bool CanAttackAllies { get; set; }//=> Abilities.Any(a => a.OverridesFriendAttack);
		public List<Ability> Abilities{ get; private set; }
		public List<Effect> Effects { get; }
		public string Description { get; }
		public string Quote { get; }
		public string Author { get; }
		public readonly FightType Type;
		public GameObject CharacterObject { get; set; }
		public HexCell ParentCell { get; set; }
		public bool IsOnMap { get; set; }

		public bool HasUsedBasicMoveInPhaseBefore { private get; set; }
		public bool HasUsedBasicAttackInPhaseBefore { private get; set; }
		public bool HasUsedNormalAbilityInPhaseBefore { private get; set; }
		public bool HasUsedUltimatumAbilityInPhaseBefore { private get; set; }

		private bool CanUseBasicMove => !HasUsedBasicMoveInPhaseBefore && !HasUsedUltimatumAbilityInPhaseBefore;
		private bool CanUseBasicAttack => !HasUsedBasicAttackInPhaseBefore && !HasUsedNormalAbilityInPhaseBefore && !HasUsedUltimatumAbilityInPhaseBefore && !HasBasicAttackInabilityEffect;
		public bool CanUseNormalAbility => !HasUsedNormalAbilityInPhaseBefore && !HasUsedBasicAttackInPhaseBefore && !HasUsedUltimatumAbilityInPhaseBefore;
		public bool CanUseUltimatumAbility => !(HasUsedUltimatumAbilityInPhaseBefore || HasUsedBasicMoveInPhaseBefore || HasUsedBasicAttackInPhaseBefore || HasUsedNormalAbilityInPhaseBefore || TookActionInPhaseBefore);//Active.Turn.CharacterThatTookActionInTurn == this);

		public bool TookActionInPhaseBefore { get; set; }
		public bool IsAlive => HealthPoints.Value > 0;

		private bool IsStunned => Effects.Any(e => e.GetType() == typeof(Stun));
		private bool CanMove => Effects.All(e => e.GetType() != typeof(MovementDisability));
		private bool HasBasicAttackInabilityEffect => Effects.Any(e => e.GetType() == typeof(BasicAttackInability));

		public bool CanTakeAction => !(TookActionInPhaseBefore || !IsAlive || Active.Turn.CharacterThatTookActionInTurn != null && Active.Turn.CharacterThatTookActionInTurn != this || IsStunned);

		public Action<Character> BasicAttack;
		public Action<List<HexCell>> BasicMove;
		public Func<List<HexCell>> GetBasicMoveCells;
		public Func<List<HexCell>> GetBasicAttackCells;
		
		#endregion
		#region Delegates
        public delegate void VoidDelegate();
		public delegate void DamageDelegate(Damage damage);
		public delegate void CharacterDamageDelegate(Character character, Damage damage);
		public delegate void CharacterIntDelegate(Character targetCharacter, int value);
		public delegate void CharacterRefIntDelegate(Character targetCharacter, ref int value);
		#endregion
		#region Events
		public event VoidDelegate JustBeforeFirstAction;
		public event VoidDelegate OnEnemyKill;
		public event DamageDelegate BeforeBeingDamaged;
		public event DamageDelegate AfterBeingDamaged;
		public event CharacterDamageDelegate BeforeBeingBasicAttacked;
		public event CharacterDamageDelegate BeforeBasicAttack;
		public event CharacterDamageDelegate AfterBasicAttack;
		public event CharacterDamageDelegate BeforeAttack;
		/// <summary>
		/// Triggers after calculating all modifiers and defenses,
		/// useful for modifying `true` damage
		/// </summary>
//		public event CharacterDamageDelegate JustBeforeAttack;
		public event CharacterDamageDelegate AfterAttack;
		public event CharacterIntDelegate OnHeal;
		#endregion
		public Character(string name)
		{
			#region Property definitions
			//Define basic properties
			IsOnMap = false;
			TookActionInPhaseBefore = true;
			HasUsedBasicAttackInPhaseBefore = false;
			HasUsedBasicMoveInPhaseBefore = false;
			HasUsedNormalAbilityInPhaseBefore = false;
			HasUsedUltimatumAbilityInPhaseBefore = false;
			CanAttackAllies = false;
			Effects = new List<Effect>();
			
			BasicAttack = DefaultBasicAttack;
			BasicMove = DefaultBasicMove;
			GetBasicAttackCells = DefaultGetBasicAttackCells;
			GetBasicMoveCells = DefaultGetBasicMoveCells;
			
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
			
			#endregion
			
			AddTriggersToEvents();
			CreateAndInitiateAbilities(name);
		}
		private void AddTriggersToEvents()
		{
			JustBeforeFirstAction += () => Active.Turn.CharacterThatTookActionInTurn = this;
			AfterBeingDamaged += damage => RemoveIfDead();
			OnEnemyKill += () => Abilities.ForEach(a => a.OnEnemyKill());
			AfterAttack += (targetCharacter, damage) =>
				MessageLogger.Log(
					$"{this.FormattedFirstName()} atakuje {targetCharacter.FormattedFirstName()}, zadając <color=red><b>{damage.Value}</b></color> obrażeń!");
//			AfterAttack += (targetCharacter, damage) => Abilities.ForEach(a => a.OnDamage(targetCharacter, damageDealt));
			AfterAttack += (targetCharacter, damage) =>
				AnimationPlayer.Add(new Tilt(targetCharacter.CharacterObject.transform));
			AfterAttack += (targetCharacter, damage) =>
				AnimationPlayer.Add(new ShowInfo(targetCharacter.CharacterObject.transform, damage.Value.ToString(), Color.red));
			OnHeal += (targetCharacter, valueHealed) =>
				AnimationPlayer.Add(new ShowInfo(targetCharacter.CharacterObject.transform, valueHealed.ToString(),
					Color.blue));
		}
		private void CreateAndInitiateAbilities(string name)
		{
			IEnumerable<string> abilityClassNames = GameData.Conn.GetAbilityClassNames(name);
			var abilityNamespaceName = "Abilities." + name.Replace(' ', '_');
			List<Ability> abilities =
				Spawner.Create(abilityNamespaceName, abilityClassNames).ToList().ConvertAll(x => x as Ability);
			InitiateAbilities(abilities);
		}
		public void MoveTo(HexCell targetCell)
		{
			ParentCell.CharacterOnCell = null;
			ParentCell = targetCell;
			targetCell.CharacterOnCell = this;
			CharacterObject.transform.parent = targetCell.transform;
			AnimationPlayer.Add(new MoveTo(CharacterObject.transform, CharacterObject.transform.parent.transform.TransformPoint(0,10,0), 0.13f));
		}
		public void DefaultBasicMove(List<HexCell> cellPath)
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
		public void DefaultBasicAttack(Character attackedCharacter)
		{
			var damage = new Damage(AttackPoints.Value, DamageType.Physical);
			BeforeBasicAttack?.Invoke(attackedCharacter, damage);
			attackedCharacter.BeforeBeingBasicAttacked?.Invoke(attackedCharacter, damage);
			Attack(attackedCharacter, damage);
//			if (attackedCharacter.Abilities.All(a => a.BeforeParentBasicAttacked(this)))
//			{
//				if (attackedCharacter.Owner == Owner)
//				{
//					if (Abilities.Any(a => a.OverridesFriendAttack))
//					{
//						if (Abilities.Count(a => a.OverridesFriendAttack) > 1)
//							throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać atak!");
//
//						Abilities.Single(a => a.OverridesFriendAttack).AttackFriend(attackedCharacter, damage);
//					}
//				}
//				else
//				{
//					if (Abilities.Any(a => a.OverridesEnemyAttack))
//					{
//						if (Abilities.Count(a => a.OverridesEnemyAttack) > 1)
//							throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać atak!");
//
//						Abilities.Single(a => a.OverridesEnemyAttack).AttackEnemy(attackedCharacter, damage);
//					}
//					else
//					{
//						Attack(attackedCharacter, damage));
//					}
//				}
//			}
			AfterBasicAttack?.Invoke(attackedCharacter, damage);
			

			HasUsedBasicAttackInPhaseBefore = true;
			//HasUsedNormalAbilityInPhaseBefore = true;
		}
		public void Attack(Character character, Damage damage)//, AttackType attackType, int atkPoints)
		{
			BeforeAttack?.Invoke(character, damage);
            character.ReceiveDamage(damage);
			AfterAttack?.Invoke(character, damage);
            if (!character.IsAlive) OnEnemyKill?.Invoke();
		}

		private int GetDefense(DamageType damageType)
		{
			switch (damageType)
			{
				case DamageType.Physical:
					return PhysicalDefense.Value;
				case DamageType.Magical:
					return MagicalDefense.Value;
				case DamageType.True:
					return 0;
				default:
					throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null);
			}
		}
		private void ReceiveDamage(Damage damage)
		{
			BeforeBeingDamaged?.Invoke(damage);
			
			var defense = GetDefense(damage.Type);
			damage.Value -= defense;
			damage.Value = damage.Value < 0 ? 0 : damage.Value;
			HealthPoints.Value -= damage.Value;
			
			AfterBeingDamaged?.Invoke(damage);
		}
		public void RemoveIfDead()
		{
			if (IsAlive) return;

			MessageLogger.Log($"{this.FormattedFirstName()} umiera!");
			RemoveFromMap();
			DeathTimer = 0;
			if (Active.CharacterOnMap == this) Deselect();
		}
		public void InvokeJustBeforeFirstAction() => JustBeforeFirstAction?.Invoke();

		private void RemoveFromMap()
		{
			ParentCell.CharacterOnCell = null;
			ParentCell = null;
			IsOnMap = false;
//			Object.Destroy(CharacterObject);//TODO: enqueue that as animation
			AnimationPlayer.Add(new Destroy(CharacterObject));
		}
//		private void DamageModifier(Character targetCharacter, ref int damage)
//		{
//			foreach (Ability ability in Abilities)
//			{
//				ability.DamageModifier(targetCharacter, ref damage);
//			}
//		}
//		private void TrueDamageModifier(Character targetCharacter, ref int damage)
//		{
//			foreach (Ability ability in Abilities)
//			{
//				ability.DamageModifier(targetCharacter, ref damage);
//			}
//		}
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

			Active.HexCells.ForEach(c=>c.ToggleHighlight(c.CharacterOnCell!=null ? HiglightColor.Red: HiglightColor.GreenTransparent));
			Active.RemoveMoveCells();
			Active.MoveCells.Add(ParentCell);

		}
		private List<HexCell> GetPrepareBasicMoveCells()
		{
			if (CanMove) return GetBasicMoveCells();
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
				cellRange.RemoveNonCharacters();
			else
				cellRange.RemoveNonEnemies();

			return cellRange;
		}
		public List<HexCell> DefaultGetBasicAttackCells()
		{
//			if (Abilities.Any(a => a.OverridesGetBasicAttackCells))
//			{
//				if (Abilities.Count(a => a.OverridesGetBasicAttackCells) > 1)
//					throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać komórki podstawowego ataku!");
//
//				return Abilities.Single(a => a.OverridesGetBasicAttackCells).GetBasicAttackCells();
//			}

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
		public List<HexCell> DefaultGetBasicMoveCells()
		{
//			if (Abilities.Any(a => a.OverridesGetMoveCells))
//			{
//				if (Abilities.Count(a => a.OverridesGetMoveCells) > 1)
//					throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać komórki ruchu!");
//
//				return Abilities.Single(a => a.OverridesGetMoveCells).GetMoveCells();
//			}

			List<HexCell> cellRange = ParentCell.GetNeighbors(Speed.Value, SearchFlags.StopAtEnemyCharacters | SearchFlags.StopAtFriendlyCharacters | SearchFlags.StopAtWalls);
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
//			List<GameObject> characterButtons = new List<GameObject>(CharacterAbilities.Instance.Buttons);
//			characterButtons.AddRange(new List<GameObject>(CharacterEffects.Instance.Buttons));
//			Active.Buttons = characterButtons;
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
//	public enum AttackType
//	{
//		Physical,
//		Magical,
//		True
//	}

}
