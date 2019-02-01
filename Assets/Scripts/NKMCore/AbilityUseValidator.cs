using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Abilities.Bezimienni;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Templates;
using Unity.Managers;

// ReSharper disable MemberCanBePrivate.Global

namespace NKMCore
{
	public class AbilityUseValidator
	{
		public readonly List<Func<bool>> ToCheck = new List<Func<bool>>();
		private readonly Ability _abilityToValidate;
		private static Active Active => GameStarter.Instance.Game.Active;
	
		public AbilityUseValidator(Ability abilityToValidate)
		{
			_abilityToValidate = abilityToValidate;
			ToCheck.Add(CanBeClicked);
			ToCheck.Add(IsOwnerActivePlayer);
			ToCheck.Add(CanCharacterUseAbility);
			ToCheck.Add(IsNotOnCooldown);
			ToCheck.Add(IsCharacterNotSilenced);
			ToCheck.Add(CharacterNotGroundedOrCanUseOnGround);
			if(_abilityToValidate.Type == Ability.AbilityType.Ultimatum) ToCheck.Add(IsActivePhaseGreaterThanThree);
		}

		public Func<bool> CanBeClicked => () => _abilityToValidate is IClickable;
		public Func<bool> IsCharacterNotSilenced => () => !_abilityToValidate.ParentCharacter.Effects.ContainsType<Silent>();
		public Func<bool> IsOwnerActivePlayer => () => _abilityToValidate.ParentCharacter.Owner == Active.GamePlayer;
		public Func<bool> CanCharacterUseAbility => () => _abilityToValidate.ParentCharacter.CanTakeAction
		                                                  && (_abilityToValidate.Type == Ability.AbilityType.Normal && _abilityToValidate.ParentCharacter.CanUseNormalAbility
		                                                      || _abilityToValidate.Type == Ability.AbilityType.Ultimatum && _abilityToValidate.ParentCharacter.CanUseUltimatumAbility);

		public Func<bool> IsActivePhaseGreaterThanThree => () => Active.Phase.Number > 3;
		public Func<bool> AreAnyTargetsInRange => () => _abilityToValidate.GetTargetsInRange().Count > 0;
		public Func<bool> IsNotOnCooldown => () => _abilityToValidate.CurrentCooldown <= 0 || IsAbilityFree;

		public Func<bool> CharacterNotGroundedOrCanUseOnGround => () =>
			_abilityToValidate.CanUseOnGround || !_abilityToValidate.ParentCharacter.IsGrounded;

		private bool IsAbilityFree => _abilityToValidate.ParentCharacter.Abilities.ContainsType(typeof(AceInTheHole)) &&
		                              _abilityToValidate.ParentCharacter.Abilities.OfType<AceInTheHole>().First().HasFreeAbility;

		public bool AbilityCanBeUsed
		{
			get
			{
				foreach (Func<bool> isTrue in ToCheck) if (!isTrue()) return false;
				return true;
			}
		}

	}
}