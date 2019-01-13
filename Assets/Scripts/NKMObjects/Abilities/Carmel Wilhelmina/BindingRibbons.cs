using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Carmel_Wilhelmina
{
    public class BindingRibbons : Ability, IClickable, IUseable
    {
        private const int Range = 4;
        private const int Radius = 3;
        private const int EnemiesToHitToActivateSnare = 3;
        private const int RootDuration = 1;
        private const int SilentDuration = 1;
        public BindingRibbons(Game game) : base(game, AbilityType.Normal, "Binding Ribbons", 4){}

        public override string GetDescription() =>
$@"{ParentCharacter.Name} rzuca zaklęcie w obszar o promieniu {Radius},
uciszając wszystkich wrogów na {SilentDuration} fazę.
Gdy trafi co najmniej {EnemiesToHitToActivateSnare} wrogów unieruchamia ich dodatkowo na {RootDuration} fazę.";

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);

        public void Click()
        {
            Active.Prepare(this, GetRangeCells(), false, false);
            Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, Radius);
        }

        public void Use(List<HexCell> cells)
        {
           // Active.MakeAction();
			ParentCharacter.TryToTakeTurn();
            List<Character> enemiesInRange = cells.WhereEnemiesOf(Owner).GetCharacters();
            enemiesInRange.ForEach(c =>
            {
                c.Effects.Add(new Silent(Game, SilentDuration, c, Name));
                if(enemiesInRange.Count >= EnemiesToHitToActivateSnare) 
                    c.Effects.Add(new Snare(Game, RootDuration, c, Name));
                
            });
            Finish();
        }
    }
}