using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Rem
{
	public class Confession : Ability, IClickable, IUseableCharacter
	{
		private const int Range = 6;

		public Confession(Game game) : base(game, AbilityType.Ultimatum, "Confession", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereFriendsOf(Owner).FindAll(c => c.FirstCharacter.TookActionInPhaseBefore);

		public override string GetDescription() =>
$@"{ParentCharacter.Name} wyznaje miłość wybranej postaci, umożliwiając jej ponowną akcję w tej fazie.

Zasięg: {Range}	Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());

		public void Use(Character character)
		{
			ParentCharacter.TryToTakeTurn();
			character.TookActionInPhaseBefore = false;
			character.HasUsedBasicAttackInPhaseBefore = false;
			character.HasUsedBasicMoveInPhaseBefore = false;
			character.HasUsedNormalAbilityInPhaseBefore = false;
			character.HasUsedUltimatumAbilityInPhaseBefore = false;
			Finish();
		}
	}
}
