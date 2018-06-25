using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Llenn
{
    public class GrenadeThrow : Ability, IClickable
    {
        private const int Damage = 13;
        private const int Range = 7;
        private const int Radius = 2;
        public GrenadeThrow() : base(AbilityType.Normal, "Grenade Throw", 3){}
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);

        public override string GetDescription() => 
$@"{ParentCharacter.Name} rzuca granatem,
zadając {Damage} obrażeń fizycznych wszystkim postaciom w promieniu {Radius}.
Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click()
        {
            Active.Prepare(this, GetRangeCells(), false, false);
			Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, Radius);
        }

        public override void Use(List<HexCell> cells)
        {
            cells.GetCharacters()
                .ForEach(c => ParentCharacter.Attack(this, c, new Damage(Damage, DamageType.Physical)));
            OnUseFinish();
        }
    }
}