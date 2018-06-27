using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Rem
{
	public class Confession : Ability, IClickable, IUseable
	{
		private const int Range = 6;

		public Confession() : base(AbilityType.Ultimatum, "Confession", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyFriendsOf(Owner).FindAll(c => !c.CharacterOnCell.TookActionInPhaseBefore);

		public override string GetDescription() =>
$@"{ParentCharacter.Name} wyznaje miłość wybranej postaci, umożliwiając jej ponowną akcję w tej fazie.

Zasięg: {Range}	Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());
	    public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

		private void Use(Character character)
		{
			character.TookActionInPhaseBefore = false;
			character.HasUsedBasicAttackInPhaseBefore = false;
			character.HasUsedBasicMoveInPhaseBefore = false;
			character.HasUsedNormalAbilityInPhaseBefore = false;
			character.HasUsedUltimatumAbilityInPhaseBefore = false;
			Finish();
		}
	}
}
