using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Derieri
{
    public class ComboBuilder : Ability, IClickable, IUseableCharacter
    {
        private const int ComboIncrease = 3;
        private const int Range = 2;
        public ComboBuilder(Game game) : base(game, AbilityType.Normal, "Combo builder", 2)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} wykonuje kilka uderzeń w wybranego wroga, które zwiększają Combo o {ComboIncrease}.
Dodatkowo otrzymuje możliwość podstawowego ataku.

Zasięg liniowy: {Range}    Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(Character target)
        {
            ParentCharacter.TryToTakeTurn();
			ComboStar passiveAbility = ParentCharacter.Abilities.OfType<ComboStar>().SingleOrDefault();
            if (passiveAbility != null)
            {
                if (target != passiveAbility.ComboCharacter) passiveAbility.SetNewComboCharacter(target);
                passiveAbility.Combo += 3;
            }
            ParentCharacter.HasFreeAttackUntilEndOfTheTurn = true;
            Finish();
        }
    }
}