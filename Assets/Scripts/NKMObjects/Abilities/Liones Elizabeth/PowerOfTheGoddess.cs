using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Liones_Elizabeth
{
    public class PowerOfTheGoddess : Ability, IClickable
    {
        private const int Heal = 20;
        public PowerOfTheGoddess(Game game) : base(game, AbilityType.Ultimatum, "Power of the goddess", 6)
        {
        }

        public override List<HexCell> GetRangeCells() => HexMap.Cells;
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereFriendsOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} leczy wszystkich sojuszników (w tym siebie) na mapie za {Heal} HP.

Czas odnowienia: {Cooldown}";

        public void Click()
        {
            //Active.MakeAction();
			ParentCharacter.TryToTakeTurn();
            GetTargetsInRange().GetCharacters().ForEach(c => ParentCharacter.Heal(c, Heal));
            Finish();
        }
    }
}
