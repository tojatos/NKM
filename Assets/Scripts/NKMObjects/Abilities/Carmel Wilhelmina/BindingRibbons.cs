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
        private const int Damage = 12;
        private const int EnemiesToHitToActivateStun = 3;
        private const int RootDuration = 1;
        private const int SilentDuration = 2;
        public BindingRibbons() : base(AbilityType.Normal, "Binding Ribbons", 4){}

        public override string GetDescription() =>
$@"{ParentCharacter.Name} rzuca zaklęcie w obszar o promieniu {Radius},
uciszając wszystkich wrogów na {SilentDuration} fazy i zadając im {Damage} obrażeń magicznych.
Gdy trafi co najmniej {EnemiesToHitToActivateStun} wrogów unieruchamia ich dodatkowo na {RootDuration} fazę.";

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);

        public void Click()
        {
            Active.Prepare(this, GetRangeCells(), false, false);
            Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, Radius);
        }

        public void Use(List<HexCell> cells)
        {
            Active.MakeAction();
            List<Character> enemiesInRange = cells.WhereOnlyEnemiesOf(Owner).GetCharacters();
            enemiesInRange.ForEach(c =>
            {
                c.Effects.Add(new Silent(SilentDuration, c, Name));
                ParentCharacter.Attack(this, c, new Damage(Damage, DamageType.Magical));
                if(enemiesInRange.Count >= EnemiesToHitToActivateStun) 
                    c.Effects.Add(new MovementDisability(RootDuration, c, Name));
                
            });
            Finish();
        }
    }
}