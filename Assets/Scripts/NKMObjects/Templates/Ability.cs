using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Abilities.Bezimienni;

namespace NKMObjects.Templates
{
	public abstract class Ability
	{
		
		protected readonly Game Game;
		protected Active Active => Game.Active;
		protected Action Action => Game.Action;
		protected HexMap HexMap => Game.HexMap;
		protected Console Console => Game.Console;
		public Character Owner => Game.Characters.First(c => c.Abilities.Contains(this));
		protected Ability(Game game, AbilityType type, string name, int cooldown = 0) : this(game, type, name, cooldown, NKMID.GetNext("Ability")){}
		protected Ability(Game game, AbilityType type, string name, int cooldown, uint id)
		{
			Game = game;
			ID = id;
			Type = type;
			Name = name;
			Cooldown = cooldown;
			CurrentCooldown = 0;
			OnAwake += () =>
			{
				Validator = new AbilityUseValidator(this);
                Active.Phase.PhaseFinished += () =>
                {
                    if (CurrentCooldown > 0) CurrentCooldown--;
                };
                //Owner = ParentCharacter.Owner;
				AfterUseFinish += () => ParentCharacter.InvokeAfterAbilityUse(this);
			};
		}

		protected List<HexCell> GetNeighboursOfOwner(int depth, SearchFlags searchFlags = SearchFlags.None, Predicate<HexCell> stopAt = null) =>
			ParentCharacter.ParentCell.GetNeighbors(Owner.Owner, depth, searchFlags, stopAt);
		public readonly string Name;
		public readonly uint ID;
		public bool CanUseOnGround { get; protected set; } = true;
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
			set
			{
				_currentCooldown = value;
				UI.CharacterUI.Abilities.Instance.UpdateButtonData();
			}
		}

		public Character ParentCharacter => Game.Characters.FirstOrDefault(c => c.Abilities.Contains(this));

		public bool CanBeUsed => Validator.AbilityCanBeUsed;

		protected void Finish() => Finish(Cooldown);

		protected void Finish(int cooldown)
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
		
		protected event Delegates.Void OnAwake;
		public event Delegates.Void AfterUseFinish;
		public void Awake() => OnAwake?.Invoke();

		public virtual void Cancel()
		{
			OnFailedUseFinish();
		}
        public enum AbilityType
        {
            Passive,
            Normal,
            Ultimatum
        }
	}
}
