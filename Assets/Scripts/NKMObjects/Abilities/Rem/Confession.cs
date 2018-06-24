using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Rem
{
	public class Confession : Ability, IClickable
	{
		private const int AbilityRange = 6;

		public Confession() : base(AbilityType.Ultimatum, "Confession", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyFriendsOf(Owner).FindAll(c => !c.CharacterOnCell.TookActionInPhaseBefore);

		public override string GetDescription() =>
$@"{ParentCharacter.Name} wyznaje miłość wybranej postaci, umożliwiając jej ponowną akcję w tej fazie.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());
		public override void Use(Character character)
		{
			character.TookActionInPhaseBefore = false;
			character.HasUsedBasicAttackInPhaseBefore = false;
			character.HasUsedBasicMoveInPhaseBefore = false;
			character.HasUsedNormalAbilityInPhaseBefore = false;
			character.HasUsedUltimatumAbilityInPhaseBefore = false;
			OnUseFinish();
		}
	}
}
