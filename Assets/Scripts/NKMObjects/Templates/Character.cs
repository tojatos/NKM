using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using JetBrains.Annotations;
using NKMObjects.Effects;
using UnityEngine;

namespace NKMObjects.Templates
{
	public class Character
	{
		private readonly Game _game;
		private Active Active => _game.Active;
		private Console Console => _game.Console;
		
		public string Name;
		public override string ToString() => Name + $" ({ID})";
		
		
		public Action<Character> BasicAttack { private get; set; }
		public Action<List<HexCell>> BasicMove { private get; set; }
		public Func<List<HexCell>> GetBasicMoveCells { get; }
		public Func<List<HexCell>> GetBasicAttackCells;
		
		#region Readonly Properties
		public readonly uint ID;
		public readonly Stat HealthPoints;
		public readonly Stat AttackPoints;
		public readonly Stat BasicAttackRange;
		public readonly Stat Speed;
		public readonly Stat PhysicalDefense;
		public readonly Stat MagicalDefense;
		public readonly Stat Shield;
		public readonly FightType Type;

		public List<Ability> Abilities { get; }
		public List<Effect> Effects { get; } = new List<Effect>();

		public GamePlayer Owner => _game.Players.First(p => p.Characters.Contains(this));
		public HexCell ParentCell => _game.HexMap.GetCell(this);
		public bool IsAlive => HealthPoints.Value > 0;
		
        public  bool  IsStunned                      =>  Effects.ContainsType<Stun>();
        public  bool  IsGrounded                     =>  Effects.ContainsType<Ground>();
        public  bool  IsSnared                       =>  Effects.ContainsType<Snare>();
        public  bool  IsFlying                       =>  Effects.ContainsType<Flying>();
        public  bool  HasBasicAttackInabilityEffect  =>  Effects.ContainsType<Disarm>();

		private bool CanMove => !IsSnared && !IsGrounded;

		public bool IsLeaving { get; set; }

		private bool CanUseBasicMove => !HasUsedBasicMoveInPhaseBefore && !HasUsedUltimatumAbilityInPhaseBefore ||
		                                HasFreeMoveUntilEndOfTheTurn;

		private bool CanUseBasicAttack =>
			!HasUsedBasicAttackInPhaseBefore && !HasUsedNormalAbilityInPhaseBefore &&
			!HasUsedUltimatumAbilityInPhaseBefore && !HasBasicAttackInabilityEffect || HasFreeAttackUntilEndOfTheTurn;

		public bool CanUseNormalAbility => !HasUsedNormalAbilityInPhaseBefore && !HasUsedBasicAttackInPhaseBefore &&
		                                   !HasUsedUltimatumAbilityInPhaseBefore;

		public bool CanUseUltimatumAbility =>
			!(HasUsedUltimatumAbilityInPhaseBefore || HasUsedBasicMoveInPhaseBefore ||
			  HasUsedBasicAttackInPhaseBefore || HasUsedNormalAbilityInPhaseBefore || TookActionInPhaseBefore) ||
			HasFreeUltimatumAbilityUseUntilEndOfTheTurn;

		public bool CanBasicAttack(Character targetCharacter) =>
			(this.IsEnemyFor(targetCharacter) || CanAttackAllies) &&
			GetBasicAttackCells().Contains(targetCharacter.ParentCell);


		public bool CanTakeAction => !(TookActionInPhaseBefore || !IsAlive ||
		                               Active.Turn.CharacterThatTookActionInTurn != null &&
		                               Active.Turn.CharacterThatTookActionInTurn != this || IsStunned);

		public bool CanWait => !(Owner != Active.GamePlayer || TookActionInPhaseBefore ||
		                         Active.Turn.CharacterThatTookActionInTurn != null);
		#endregion
		
		#region Other Properties
		public bool CanAttackAllies { get; set; }
		public bool IsOnMap => _game.HexMap.GetCell(this) != null;// { get; set; }

		public bool HasUsedBasicMoveInPhaseBefore { private get; set; }
		public bool HasUsedBasicAttackInPhaseBefore { private get; set; }
		public bool HasUsedNormalAbilityInPhaseBefore { get; set; }
		public bool HasUsedUltimatumAbilityInPhaseBefore { private get; set; }
		
		public bool HasFreeAttackUntilEndOfTheTurn { get; set; }
		public bool HasFreeUltimatumAbilityUseUntilEndOfTheTurn { get; set; }
		public bool HasFreeMoveUntilEndOfTheTurn { get; set; }
		public bool TookActionInPhaseBefore { get; set; }
		
		public int DeathTimer { get; set; }
		#endregion

		#region Events
		public event Delegates.Void JustBeforeFirstAction;
		public event Delegates.Void OnKill;
		public event Delegates.Void OnDeath;
		public event Delegates.Void BeforeMove;
		public event Delegates.Void AfterMove;
		public event Delegates.CellList AfterBasicMove;
		public event Delegates.AbilityD AfterBeingHitByAbility;
		public event Delegates.AbilityD AfterAbilityUse;
		public event Delegates.DamageD BeforeBeingDamaged;
		public event Delegates.DamageD AfterBeingDamaged;
		public event Delegates.CharacterDamage BeforeBeingBasicAttacked;
		public event Delegates.CharacterDamage BeforeBasicAttack;
		public event Delegates.CharacterDamage AfterBasicAttack;
		public event Delegates.CharacterDamage BeforeAttack;
		public event Delegates.AbilityCharacterDamage AfterAbilityAttack;
		public event Delegates.EffectCharacterDamage AfterEffectAttack;
		public event Delegates.CharacterDamage AfterAttack;
		public event Delegates.CharacterInt AfterHeal;
		public event Delegates.CharacterRefInt BeforeHeal;
		
		public void InvokeAfterAbilityUse(Ability a) => AfterAbilityUse?.Invoke(a);
		public void InvokeOnDeath() => OnDeath?.Invoke();
		#endregion


		public Character (Game game, string name, uint id, Properties properties, List<Ability> abilities)
		{
			_game = game;
				
			ID = id;
			Name = name;
			
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

			Abilities = abilities;
			
			_game.AddTriggersToEvents(this);
		}

		public void MoveTo(HexCell targetCell)
		{
			BeforeMove?.Invoke();
			_game.HexMap.Move(this, targetCell);
			AfterMove?.Invoke();
		}

		public void RemoveIfDead()
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
				MoveTo(nextCell);
			}

		}

		public void DefaultBasicAttack(Character attackedCharacter)
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

		private List<HexCell> GetPrepareBasicMoveCells()
			=> CanMove ? GetBasicMoveCells() : Enumerable.Empty<HexCell>().ToList();

		private List<HexCell> GetPrepareBasicAttackCells() => CanAttackAllies
			? GetBasicAttackCells().WhereCharacters()
			: GetBasicAttackCells().WhereEnemiesOf(Owner);

		public List<HexCell> DefaultGetBasicAttackCells() => DefaultGetBasicAttackCells(ParentCell);
		public List<HexCell> DefaultGetBasicAttackCells(HexCell fromCell)
		{
			List<HexCell> cellRange = new List<HexCell>();
			switch (Type)
			{
				case FightType.Ranged:
					cellRange = fromCell.GetNeighbors(Owner, BasicAttackRange.Value, SearchFlags.StraightLine);
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
	                        if (cell.Type == HexCell.TileType.Wall)
	                        {
                                removeAfterIndex = index;
                                break;
	                        }
                            if (!cell.IsEmpty && (CanAttackAllies || cell.CharactersOnCell[0].IsEnemyFor(Owner)))
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
				? ParentCell.GetNeighbors(Owner, Speed.Value, SearchFlags.StopAtEnemyCharacters)
				: ParentCell.GetNeighbors(Owner, Speed.Value, SearchFlags.StopAtEnemyCharacters | SearchFlags.StopAtWalls);


		public event Delegates.Void AfterSelect;
		public event Delegates.Void AfterDeselect;
		public void Select()
		{
			Active.Clean();
			Active.CharacterOnMap = this;
			AfterSelect?.Invoke();
			if (Active.GamePlayer != Owner) return;
			PrepareAttackAndMove();
		}
		public void Deselect()
		{
			Active.CharacterOnMap = null;
			Active.Action = Action.None;
			Active.HexCells = null;
			Active.RemoveMoveCells();
			AfterDeselect?.Invoke();
		}

		public void TryToInvokeJustBeforeFirstAction()
		{
			if (Active.Turn.CharacterThatTookActionInTurn == null) JustBeforeFirstAction?.Invoke();
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
				Active.SelectDrawnCell(c).AddHighlight(
					!c.IsEmpty&&
					(c.CharactersOnCell[0].IsEnemyFor(Owner) || CanAttackAllies && CanUseBasicAttack && GetBasicAttackCells().Contains(c))
						? Highlights.RedTransparent
						: Highlights.GreenTransparent));
				
			Active.RemoveMoveCells();
			Active.MoveCells.Add(ParentCell);

		}

		public void MakeActionBasicAttack([NotNull] Character target)
		{
			TryToInvokeJustBeforeFirstAction();
            BasicAttack(target);
			Console.GameLog($"BASIC ATTACK: {target}"); //logging after action to make reading rng work
		}
		public void MakeActionBasicMove([NotNull] List<HexCell> moveCells)
		{
			TryToInvokeJustBeforeFirstAction();
			BasicMove(new List<HexCell>(moveCells)); //work on a new list to log unmodified list below
			AfterBasicMove?.Invoke(moveCells);
		}

		public GameObject CharacterObject => _game.HexMapDrawer.GetCharacterObject(this);// { get; set; }
		
        public class Properties
        {
            public Stat HealthPoints;
            public Stat AttackPoints;
            public Stat BasicAttackRange;
            public Stat Speed;
            public Stat PhysicalDefense;
            public Stat MagicalDefense;
            public Stat Shield;
            
            public FightType Type;
        }
	}
}
