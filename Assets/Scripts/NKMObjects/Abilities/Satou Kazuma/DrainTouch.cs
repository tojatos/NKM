using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Satou_Kazuma
{
    public class DrainTouch : Ability, IClickable, IUseable
    {
        private const int Damage = 18;
        private const int Range = 6;
        public DrainTouch() : base(AbilityType.Normal, "Drain Touch", 3)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} wysysa z przeciwnika {Damage} HP,
zadając mu tyle obrażeń magicznych
i przywracając sobie HP równe wartości zadanej przeciwnikowi.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());
        public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

        private void Use(NKMCharacter character)
        {
            var dmg = new Damage(Damage, DamageType.Magical);
            ParentCharacter.Attack(this, character, dmg);
            ParentCharacter.Heal(ParentCharacter, dmg.Value);
            Finish();
        }
    }
}