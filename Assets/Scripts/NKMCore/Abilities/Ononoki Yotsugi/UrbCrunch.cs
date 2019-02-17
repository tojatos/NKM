using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Ononoki_Yotsugi
{
    public class UrbCrunch : Ability, IClickable, IUseableCellList
    {
        private const int Damage = 20;
        private const int Range = 5;
        private const int Radius = 3;

        public event Delegates.CellList AfterCrunch;
        
        public UrbCrunch(Game game) : base(game, AbilityType.Normal, "URB - Crunch", 3) { }

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} uderza w wybrany obszar o promieniu {Radius},
zadając {Damage} obrażeń fizycznych i ogłuszając trafionych wrogów na fazę.
Uderzenie trwale niszczy wszystkie przeszkody.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);

		public void Click() => Active.PrepareAirSelection(this, GetRangeCells(), AirSelection.SelectionShape.Circle, Radius);

        public void Use(List<HexCell> cells)
        {
			ParentCharacter.TryToTakeTurn();
            cells.FindAll(c => c.Type == HexCell.TileType.Wall).ForEach(c => c.Type = HexCell.TileType.Normal);
            AfterCrunch?.Invoke(cells);
            cells.WhereEnemiesOf(Owner).GetCharacters().ForEach(e =>
            {
                ParentCharacter.Attack(this, e, new Damage(Damage, DamageType.Physical));
                e.Effects.Add(new Stun(Game, 1, e, Name));
            });
            Finish();
        }
    }
}