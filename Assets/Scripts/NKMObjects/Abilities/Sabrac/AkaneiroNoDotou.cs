using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Sabrac
{
    public class AkaneiroNoDotou : Ability, IClickable, IUseable
    {
        private const int Range = 9;
        private const int Damage = 30;
        private const int SilentDuration = 2;
        private const int Width = 3;
        public AkaneiroNoDotou() : base(AbilityType.Ultimatum, "Akaneiro no Dotou", 5) { }

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} tworzy falę płomieni i mieczy, na której przemieszcza się w wybrany punkt.
Fala zadaje {Damage} obrażeń magicznych i ucisza na {SilentDuration} tury wszystkich wrogów którzy zostali nią trafieni.

Zasięg: {Range}
Szerokość: {Width}
Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c => c.IsFreeToStand);

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(List<HexCell> cells) => Use(cells[0]);
        
        private void Use(HexCell cell)
        {
            List<HexCell> targetCells = ParentCharacter.ParentCell.GetArea(cell, Width);
            ParentCharacter.MoveTo(cell);
            targetCells.WhereOnlyEnemiesOf(Owner).GetCharacters().ForEach(c =>
            {
                ParentCharacter.Attack(this, c, new Damage(Damage, DamageType.Magical));
                c.Effects.Add(new Silent(SilentDuration, c, Name));
            });
            Finish();
        }
    }
}