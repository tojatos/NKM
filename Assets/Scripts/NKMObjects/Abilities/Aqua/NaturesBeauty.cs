using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Aqua
{
	public class NaturesBeauty : Ability
	{
		public NaturesBeauty() : base(AbilityType.Passive, "Nature's Beauty")
		{
			OnAwake += () =>
			{
				ParentCharacter.CanAttackAllies = true;
				ParentCharacter.BasicAttack = character =>
				{
					if (character.Owner == ParentCharacter.Owner)
					{
						ParentCharacter.Heal(character, ParentCharacter.AttackPoints.Value);
						ParentCharacter.HasUsedBasicAttackInPhaseBefore = true;
						if (ParentCharacter.HasFreeAttackUntilEndOfTheTurn) ParentCharacter.HasFreeAttackUntilEndOfTheTurn = false;
					}
					else ParentCharacter.DefaultBasicAttack(character);
				};
			};
		}
		public override string GetDescription() => $"{ParentCharacter.Name} może używać podstawowych ataków na sojuszników, lecząc ich za ilość HP równą jej obecnemu atakowi.";
		public override List<HexCell> GetRangeCells() => ParentCharacter.GetBasicAttackCells();
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyFriendsOf(Owner);
	}
}
