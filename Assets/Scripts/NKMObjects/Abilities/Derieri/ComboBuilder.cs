using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Derieri
{
    public class ComboBuilder : Ability, IClickable, IUseable
    {
        private const int ComboIncrease = 3;
        private const int Range = 2;
        public ComboBuilder() : base(AbilityType.Normal, "Combo builder", 2)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} wykonuje kilka uderzeń w wybranego wroga, które zwiększają Combo o {ComboIncrease}.
Dodatkowo otrzymuje możliwość podstawowego ataku.

Zasięg liniowy: {Range}    Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(List<HexCell> cells)
        {
            NKMCharacter target = cells[0].CharacterOnCell;
			ComboStar passiveAbility = ParentCharacter.Abilities.OfType<ComboStar>().SingleOrDefault();
            if (passiveAbility == null)
            {
                OnFailedUseFinish();
                return;
            }
            if (target != passiveAbility.ComboCharacter) passiveAbility.SetNewComboCharacter(target);
            passiveAbility.Combo += 3;
            ParentCharacter.HasFreeAttackUntilEndOfTheTurn = true;
            Finish();
        }
    }
}