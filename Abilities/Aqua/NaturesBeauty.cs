using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Aqua
{
	public class NaturesBeauty : Ability
	{
		public NaturesBeauty(Game game) : base(game, AbilityType.Passive, "Nature's Beauty")
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
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereFriendsOf(Owner.Owner);
	}
}
