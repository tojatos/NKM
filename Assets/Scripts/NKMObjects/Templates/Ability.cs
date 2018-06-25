using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Abilities.Bezimienni;

namespace NKMObjects.Templates
{
	public abstract class Ability : NKMObject
	{
		protected Ability(AbilityType type, string name, int cooldown = 0)
		{
			Type = type;
			Name = name;
			Cooldown = cooldown;
			CurrentCooldown = 0;
			OnAwake += () => Validator = new AbilityUseValidator(this);
			OnAwake += () => Active.Phase.PhaseFinished += () =>
			{
				if (CurrentCooldown > 0) CurrentCooldown--;
			};
			OnAwake += () => Owner = ParentCharacter.Owner;
		}

		protected AbilityUseValidator Validator;
		public AbilityType Type { get; }
		public abstract string GetDescription();
		public virtual List<HexCell> GetRangeCells() => new List<HexCell>();
		public virtual List<HexCell> GetTargetsInRange() => new List<HexCell>();

		protected readonly int Cooldown;

		private int _currentCooldown;
		public int CurrentCooldown
		{
			get { return _currentCooldown; }
			protected set
			{
				_currentCooldown = value;
				UI.CharacterUI.Abilities.Instance.UpdateButtonData();
			}
		}

		public Character ParentCharacter { get; set; }

		public bool CanBeUsed => Validator.AbilityCanBeUsed;

		public virtual void Use(Character character)
		{
			throw new NotImplementedException();
		}
		public virtual void Use(HexCell cell)
		{
			throw new NotImplementedException();
		}
		public virtual void Use(List<HexCell> cells)
		{
			throw new NotImplementedException();
		}
		public virtual void OnUseFinish() => OnUseFinish(Cooldown);

		protected virtual void OnUseFinish(int cooldown)
		{
			if (ParentCharacter.Abilities.ContainsType(typeof(AceInTheHole)))
			{
				var ability = (ParentCharacter.Abilities.Single(a => a.GetType() == typeof(AceInTheHole)) as AceInTheHole);
				if (ability != null && ability.HasFreeAbility) ability.HasFreeAbility = false;
				else CurrentCooldown = cooldown;

			}
			else CurrentCooldown = cooldown;
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

			Active.CleanAndTrySelecting();
			AfterUseFinish?.Invoke();
		}
		protected void OnFailedUseFinish() => Active.CleanAndTrySelecting();
		
		protected event Character.VoidDelegate OnAwake;
		public event Character.VoidDelegate AfterUseFinish;
		public void Awake() => OnAwake?.Invoke();

		public virtual void Cancel() => OnFailedUseFinish();

	}
	public enum AbilityType
	{
		Passive,
		Normal,
		Ultimatum
	}
}
