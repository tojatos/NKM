using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;
using UnityEngine;

namespace NKMObjects.Abilities.Ononoki_Yotsugi
{
    public class UrbCrunch : Ability, IClickable, IUseable
    {
        private const int Damage = 20;
        private const int Range = 5;
        private const int Radius = 3;
        public UrbCrunch(Game game) : base(game, AbilityType.Normal, "URB - Crunch", 3) { }

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} uderza w wybrany obszar o promieniu {Radius},
zadając {Damage} obrażeń fizycznych i ogłuszając trafionych wrogów na fazę.
Uderzenie trwale niszczy wszystkie przeszkody.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);

        public void Click()
        {
            List<HexCell> cellRange = GetRangeCells();
            Active.Prepare(this, cellRange, false, false);
            Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, Radius);
        }

        public void Use(List<HexCell> cells)
        {
            cells.ForEach(c =>
            {
                if (c.Type != HexTileType.Wall) return;
                c.Type = HexTileType.Normal;
                Active.SelectDrawnCell(c).Color = Color.white;
            });
            HexMapDrawer.Instance.TriangulateCells();
            cells.WhereEnemiesOf(Owner).GetCharacters().ForEach(e =>
            {
                ParentCharacter.Attack(this, e, new Damage(Damage, DamageType.Physical));
                e.Effects.Add(new Stun(Game, 1, e, Name));
            });
            Finish();
        }
    }
}