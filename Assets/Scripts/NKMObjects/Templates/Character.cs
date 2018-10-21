using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Effects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NKMObjects.Templates
{
	public class Character : NKMObject
	{
		
		#region Properties

		public readonly int ID;
		public readonly Stat HealthPoints;
		public readonly Stat AttackPoints;
		public readonly Stat BasicAttackRange;
		public readonly Stat Speed;
		public readonly Stat PhysicalDefense;
		public readonly Stat MagicalDefense;
		public readonly Stat Shield;

		public List<Ability> Abilities { get; private set; }
		public List<Effect> Effects { get; } = new List<Effect>();
		public string Description { get; }
		public string Quote { get; }
		public string Author { get; }
		public readonly FightType Type;

		public bool CanAttackAllies { get; set; }
		public HexCell ParentCell { get; set; }
		public bool IsOnMap { get; set; }
		
		public bool IsAlive => HealthPoints.Value > 0;
		public Action<NKMCharacter> BasicAttack { protected get; set; }
		public Action<List<HexCell>> BasicMove { protected get; set; }
		public Func<List<HexCell>> GetBasicMoveCells { get; }
		public Func<List<HexCell>> GetBasicAttackCells;

		public bool HasUsedBasicMoveInPhaseBefore { private get; set; }
		public bool HasUsedBasicAttackInPhaseBefore { private get; set; }
		public bool HasUsedNormalAbilityInPhaseBefore { get; set; }
		public bool HasUsedUltimatumAbilityInPhaseBefore { private get; set; }
		
		
        public  bool  IsStunned                      =>  Effects.ContainsType<Stun>();
        public  bool  IsGrounded                     =>  Effects.ContainsType<Ground>();
        public  bool  IsSnared                       =>  Effects.ContainsType<Snare>();
        public  bool  IsFlying                       =>  Effects.ContainsType<Flying>();
        public  bool  HasBasicAttackInabilityEffect  =>  Effects.ContainsType<Disarm>();

		protected bool CanMove => !IsSnared && !IsGrounded;

		public bool IsLeaving { get; set; }

		protected bool CanUseBasicMove => !HasUsedBasicMoveInPhaseBefore && !HasUsedUltimatumAbilityInPhaseBefore ||
		                                HasFreeMoveUntilEndOfTheTurn;

		protected bool CanUseBasicAttack =>
			!HasUsedBasicAttackInPhaseBefore && !HasUsedNormalAbilityInPhaseBefore &&
			!HasUsedUltimatumAbilityInPhaseBefore && !HasBasicAttackInabilityEffect || HasFreeAttackUntilEndOfTheTurn;

		public bool CanUseNormalAbility => !HasUsedNormalAbilityInPhaseBefore && !HasUsedBasicAttackInPhaseBefore &&
		                                   !HasUsedUltimatumAbilityInPhaseBefore;

		public bool CanUseUltimatumAbility =>
			!(HasUsedUltimatumAbilityInPhaseBefore || HasUsedBasicMoveInPhaseBefore ||
			  HasUsedBasicAttackInPhaseBefore || HasUsedNormalAbilityInPhaseBefore || TookActionInPhaseBefore) ||
			HasFreeUltimatumAbilityUseUntilEndOfTheTurn; //Active.Turn.CharacterThatTookActionInTurn == this);

		public bool HasFreeAttackUntilEndOfTheTurn { get; set; }
		public bool HasFreeUltimatumAbilityUseUntilEndOfTheTurn { get; set; }
		public bool HasFreeMoveUntilEndOfTheTurn { get; set; }
		public bool TookActionInPhaseBefore { get; set; }
		#endregion

		#region Delegates

		public delegate void VoidDelegate();

		public delegate void AbilityDelegate(Ability ability);

		public delegate void DamageDelegate(Damage damage);

		public delegate void CharacterDamageDelegate(Character character, Damage damage);

		public delegate void EffectCharacterDamageDelegate(Effect effect, Character character, Damage damage);

		public delegate void AbilityCharacterDamageDelegate(Ability ability, Character character, Damage damage);

		public delegate void CharacterIntDelegate(Character targetCharacter, int value);

		public delegate void CharacterRefIntDelegate(Character targetCharacter, ref int value);

		#endregion

		#region Events

		public event VoidDelegate JustBeforeFirstAction;
		public event VoidDelegate OnKill;
		public event VoidDelegate OnDeath;
		public event VoidDelegate BeforeMove;
		public event VoidDelegate AfterMove;
		public event VoidDelegate AfterBasicMove;
		public event AbilityDelegate AfterBeingHitByAbility;
		public event AbilityDelegate AfterAbilityUse;
		public event DamageDelegate BeforeBeingDamaged;
		public event DamageDelegate AfterBeingDamaged;
		public event CharacterDamageDelegate BeforeBeingBasicAttacked;
		public event CharacterDamageDelegate BeforeBasicAttack;
		public event CharacterDamageDelegate AfterBasicAttack;
		public event CharacterDamageDelegate BeforeAttack;
		public event AbilityCharacterDamageDelegate AfterAbilityAttack;
		public event EffectCharacterDamageDelegate AfterEffectAttack;
		public void InvokeAfterAbilityUse(Ability a) => AfterAbilityUse?.Invoke(a);
		public void InvokeAfterBasicMove() => AfterBasicMove?.Invoke();

		/// <summary>
		/// Triggers after calculating all modifiers and defenses,
		/// useful for modifying `true` damage
		/// </summary>
//		public event CharacterDamageDelegate JustBeforeAttack;
		public event CharacterDamageDelegate AfterAttack;

		public event CharacterIntDelegate AfterHeal;
		public event CharacterRefIntDelegate BeforeHeal;
		public void InvokeBeforeHeal(Character targetCharacter, ref int value) => BeforeHeal?.Invoke(targetCharacter, ref value);

		#endregion
		
		public bool IsEnemyFor(GamePlayer player) => Owner != player;
		public bool IsEnemyFor(NKMCharacter character) => IsEnemyFor(character.Owner);

		internal Character (string name, int id, CharacterProperties properties, List<Ability> abilities)
		{
			ID = id;
			Name = name;
			
            IsOnMap                               =  false;
            TookActionInPhaseBefore               =  true;
            HasUsedBasicAttackInPhaseBefore       =  false;
            HasUsedBasicMoveInPhaseBefore         =  false;
            HasUsedNormalAbilityInPhaseBefore     =  false;
            HasUsedUltimatumAbilityInPhaseBefore  =  false;
            CanAttackAllies                       =  false;

            BasicAttack          =  DefaultBasicAttack;
            BasicMove            =  DefaultBasicMove;
            GetBasicAttackCells  =  DefaultGetBasicAttackCells;
            GetBasicMoveCells    =  DefaultGetBasicMoveCells;

            AttackPoints      =  properties.AttackPoints;
            HealthPoints      =  properties.HealthPoints;
            BasicAttackRange  =  properties.BasicAttackRange;
            Speed             =  properties.Speed;
            PhysicalDefense   =  properties.PhysicalDefense;
            MagicalDefense    =  properties.MagicalDefense;
            Shield            =  properties.Shield;
            Type              =  properties.Type;
            Description       =  properties.Description;
            Quote             =  properties.Quote;
            Author            =  properties.Author;

//			Abilities = new AbilityFactory(this).CreateAndInitiateAbilitiesFromDatabase();
			Abilities = abilities;
		}


		protected void InvokeJustBeforeFirstAction() => JustBeforeFirstAction?.Invoke();

		public virtual void MoveTo(HexCell targetCell)
		{
			BeforeMove?.Invoke();
			if (ParentCell.CharacterOnCell == this)
				ParentCell.CharacterOnCell = null; //checking in case of stepping over a friendly character
			ParentCell = targetCell;
			if (targetCell.IsFreeToStand)
				targetCell.CharacterOnCell = (NKMCharacter)this; //checking in case of stepping over a friendly character TODO
			AfterMove?.Invoke();
		}


		public virtual void RemoveIfDead()
		{
			if (IsAlive) return;
			OnDeath?.Invoke();
		}

		public void DefaultBasicMove(List<HexCell> cellPath)
		{
			HasUsedBasicMoveInPhaseBefore = true;
			if (HasFreeMoveUntilEndOfTheTurn) HasFreeMoveUntilEndOfTheTurn = false;
			Move(cellPath);
		}

		private void Move(IList<HexCell> cellPath)
		{
			cellPath.RemoveAt(0); //Remove parent cell
			foreach (HexCell nextCell in cellPath)
			{
				Object.Destroy(nextCell.gameObject.GetComponent<LineRenderer>()); //Remove the line
				MoveTo(nextCell);
			}

		}

		public void DefaultBasicAttack(NKMCharacter attackedCharacter)
		{
			if (!IsAlive) return; //Dead characters cannot use basic attacks!
			if (HasFreeAttackUntilEndOfTheTurn) HasFreeAttackUntilEndOfTheTurn = false;

			var damage = new Damage(AttackPoints.Value, DamageType.Physical);
			BeforeBasicAttack?.Invoke(attackedCharacter, damage);
			attackedCharacter.BeforeBeingBasicAttacked?.Invoke(this, damage);
			Attack(attackedCharacter, damage); //TODO: Make this default basic attack, maybe other overload
			AfterBasicAttack?.Invoke(attackedCharacter, damage);

			HasUsedBasicAttackInPhaseBefore = true;
		}

		private void Attack(Character character, Damage damage)
		{
			BeforeAttack?.Invoke(character, damage);
			character.ReceiveDamage(damage);
			AfterAttack?.Invoke(character, damage);
			if (!character.IsAlive) OnKill?.Invoke();
		}

		public void Attack(Ability ability, Character character, Damage damage)
		{
			Attack(character, damage);
			character.AfterBeingHitByAbility?.Invoke(ability);
			AfterAbilityAttack?.Invoke(ability, character, damage);
		}

		public void Attack(Effect effect, Character character, Damage damage)
		{
			Attack(character, damage);
			AfterEffectAttack?.Invoke(effect, character, damage);
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

			int defense = GetDefense(damage.Type);
			float reduction = damage.Value * defense / 100f;
			damage.Value -= (int) reduction;
			damage.Value = damage.Value < 0 ? 0 : damage.Value;
			if (Shield.Value >= damage.Value)
			{
				Shield.Value -= damage.Value;
				damage.Value = 0;
			}
			else
			{
				damage.Value -= Shield.Value;
				Shield.Value = 0;
			}

			HealthPoints.Value -= damage.Value;

			AfterBeingDamaged?.Invoke(damage);
		}
		
		public void Heal(NKMCharacter targetCharacter, int amount)
		{
			if(!targetCharacter.IsAlive) return;
			BeforeHeal?.Invoke(targetCharacter, ref amount);
			int hpBeforeHeal = targetCharacter.HealthPoints.Value;
			targetCharacter.HealthPoints.Value += amount;
			int hpAfterHeal = targetCharacter.HealthPoints.Value;
			int diff = hpAfterHeal - hpBeforeHeal;
			AfterHeal?.Invoke(targetCharacter, diff);
		}

		protected List<HexCell> GetPrepareBasicMoveCells()
			=> CanMove ? GetBasicMoveCells() : Enumerable.Empty<HexCell>().ToList();

		protected List<HexCell> GetPrepareBasicAttackCells() => CanAttackAllies
			? GetBasicAttackCells().WhereOnlyCharacters()
			: GetBasicAttackCells().WhereOnlyEnemiesOf(Owner);

		public List<HexCell> DefaultGetBasicAttackCells() => DefaultGetBasicAttackCells(ParentCell);
		public List<HexCell> DefaultGetBasicAttackCells(HexCell fromCell)
		{
			List<HexCell> cellRange = new List<HexCell>();
			switch (Type)
			{
				case FightType.Ranged:
					cellRange = fromCell.GetNeighbors(BasicAttackRange.Value, SearchFlags.StraightLine);
					break;
				case FightType.Melee:
//					cellRange = ParentCell.GetNeighbors(BasicAttackRange.Value, SearchFlags.StraightLine | SearchFlags.StopAtWalls);
					foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
					{
                        List<HexCell> line = fromCell.GetLine(direction, BasicAttackRange.Value);

                        //Remove cells after first character hit
                        int removeAfterIndex = line.Count;
                        for (int index = 0; index < line.Count; index++)
                        {
                            HexCell cell = line[index];
	                        if (cell.Type == HexTileType.Wall)
	                        {
                                removeAfterIndex = index;
                                break;
	                        }
                            if (cell.CharacterOnCell != null && (CanAttackAllies || cell.CharacterOnCell.IsEnemyFor(Owner)))
                            {
                                removeAfterIndex = index + 1;
                                break;
                            }
                        }

                        line = line.GetRange(0, removeAfterIndex);
                        cellRange.AddRange(line);
					}
					

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return cellRange;
		}
		public List<HexCell> DefaultGetBasicMoveCells() => 
			IsFlying 
				? ParentCell.GetNeighbors(Speed.Value, SearchFlags.StopAtEnemyCharacters)
				: ParentCell.GetNeighbors(Speed.Value, SearchFlags.StopAtEnemyCharacters | SearchFlags.StopAtWalls);

		public void Heal(Character targetCharacter, int amount)
		{
			if(!targetCharacter.IsAlive) return;
			BeforeHeal?.Invoke(targetCharacter, ref amount);
			int hpBeforeHeal = targetCharacter.HealthPoints.Value;
			targetCharacter.HealthPoints.Value += amount;
			int hpAfterHeal = targetCharacter.HealthPoints.Value;
			int diff = hpAfterHeal - hpBeforeHeal;
			AfterHeal?.Invoke(targetCharacter, diff);
		}
		public override string ToString() => Name + $" ({ID})";
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

	public class CharacterProperties
	{
        public Stat HealthPoints;
		public Stat AttackPoints;
		public Stat BasicAttackRange;
		public Stat Speed;
		public Stat PhysicalDefense;
		public Stat MagicalDefense;
		public Stat Shield;
		
		public FightType Type;
		
		public string Description;
		public string Quote;
		public string Author;
	}
}
