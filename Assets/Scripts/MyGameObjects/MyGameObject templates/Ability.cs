using System;
using System.Collections.Generic;
using Hex;
using UIManagers;

namespace MyGameObjects.MyGameObject_templates
{
	public abstract class Ability : MyGameObject
	{
		public AbilityType Type { get; protected set; }
		public abstract string GetDescription();
		public virtual List<HexCell> GetRangeCells() => new List<HexCell>();

		protected int Cooldown;

		private int _currentCooldown;
		public int CurrentCooldown
		{
			get { return _currentCooldown; }
			protected set
			{
				_currentCooldown = value;
				CharacterAbilities.Instance.UpdateButtonData();
			}
		}

		public Character ParentCharacter { protected get; set; }
		public virtual bool CanUse
		{
			get
			{
				if (Type == AbilityType.Passive)
				{
					return false;
				}

				try
				{
					CheckIfCanBePrepared();
					return true;
				}
				catch
				{
					return false;
				}
			}
		}
		public bool OverridesFriendAttack { get; protected set; }
		public bool OverridesEnemyAttack { get; protected set; }
		public bool OverridesGetBasicAttackCells { get; protected set; }
		public bool OverridesGetMoveCells { get; protected set; }
		public bool OverridesMove { get; protected set; }

		/// <summary>
		/// Throws an exception when you cannot use this ability.
		/// </summary>
		protected virtual void CheckIfCanBePrepared()
		{
			if (ParentCharacter.Owner != Active.GamePlayer)
			{
				throw new Exception("Nie możesz używać umiejętności innej postaci!");
			}
			if (Type == AbilityType.Passive)
			{
				throw new Exception("Nie można używać pasywnych umiejętności");
			}
			if (CurrentCooldown != 0)
			{
				throw new Exception("Umiejętność nie jest jeszcze odnowiona");
			}
			if (!ParentCharacter.CanTakeAction||
					Type == AbilityType.Normal && !ParentCharacter.CanUseNormalAbility ||
			    Type == AbilityType.Ultimatum && !ParentCharacter.CanUseUltimatumAbility)
			{
				throw new Exception("Postać posiadająca tą umiejętność nie może jej użyć");
			}
			if (Type==AbilityType.Ultimatum && Active.Phase.Number < 4)
			{
				throw new Exception("Nie można używać superumiejętności w pierwszych trzech fazach!");
			}
		}
		public void TryPrepare()
		{
			try
			{
				CheckIfCanBePrepared();
			}
			catch (Exception e)
			{
				MessageLogger.DebugLog("Umiejetność " + Name + " nie może zostać użyta:");
				MessageLogger.DebugLog("\t" + e.Message);
				return;
			}

			Game.HexMapDrawer.RemoveAllHighlights();
			Use();
		}
		protected virtual void Use()
		{
			throw new NotImplementedException();
		}
		public virtual void Use(List<Character> characters)
		{
			throw new NotImplementedException();
		}
		public virtual void Use(Character character)
		{
			throw new NotImplementedException();
		}
		public virtual void Use(HexCell cell)
		{
			throw new NotImplementedException();
		}
		public virtual void OnPhaseFinish()
		{
			if (CurrentCooldown > 0) CurrentCooldown--;
		}
		public virtual void OnUseFinish() => OnUseFinish(Cooldown);

		protected virtual void OnUseFinish(int cooldown)
		{
			CurrentCooldown = cooldown;
			switch (Type)
			{
				case AbilityType.Normal:
					ParentCharacter.HasUsedNormalAbilityInPhaseBefore = true;
					//ParentCharacter.HasUsedBasicAttackInPhaseBefore = true;
					break;
				case AbilityType.Ultimatum:
					ParentCharacter.HasUsedUltimatumAbilityInPhaseBefore = true;
					break;
				case AbilityType.Passive:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			Active.Clean();
		}
		protected void OnFailedUseFinish()
		{
			Active.Clean();
		}

		public virtual void DamageModifier(Character targetCharacter, ref int damage){}
		public virtual void TrueDamageModifier(Character targetCharacter, ref int damage){}
		public virtual void Awake(){}
		public virtual void OnEnemyKill(){}
		public virtual void AttackFriend(Character attackedCharacter){}
		public virtual void AttackEnemy(Character attackedCharacter){}
		public virtual void OnDamage(Character targetCharacter, int damageDealt){}
		public virtual void BeforeParentDamage(ref int damage){}
		/// <summary>
		/// Triggers before any character uses basic attack on parent.
		/// If returns false, parent cannot be attacked.
		/// </summary>
		/// <returns>Can attack</returns>
		public virtual bool BeforeParentBasicAttacked(Character attackingCharacter) { return true; }

		public virtual void Cancel() => OnFailedUseFinish();

		public virtual List<HexCell> GetBasicAttackCells()
		{
			throw new NotImplementedException();
		}
		public virtual List<HexCell> GetMoveCells()
		{
			throw new NotImplementedException();
		}
		public virtual void Move(HexCell cell)
		{
			throw new NotImplementedException();
		}
	}
	public enum AbilityType
	{
		Normal,
		Ultimatum,
		Passive
	}
}
