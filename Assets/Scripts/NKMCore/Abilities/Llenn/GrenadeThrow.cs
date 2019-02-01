using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Llenn
{
    public class GrenadeThrow : Ability, IClickable, IUseableCellList
    {
        private const int Damage = 16;
        private const int Range = 7;
        private const int Radius = 2;
        public GrenadeThrow(Game game) : base(game, AbilityType.Normal, "Grenade Throw", 3){}
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);

        public override string GetDescription() => 
$@"{ParentCharacter.Name} rzuca granatem,
zadając {Damage} obrażeń fizycznych wszystkim postaciom w promieniu {Radius}.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click()
        {
            Active.Prepare(this, GetRangeCells(), false, false);
			Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, Radius);
        }

        public void Use(List<HexCell> cells)
        {
			ParentCharacter.TryToTakeTurn();
            cells.GetCharacters().ForEach(c => ParentCharacter.Attack(this, c, new Damage(Damage, DamageType.Physical)));
            Finish();
        }
    }
}