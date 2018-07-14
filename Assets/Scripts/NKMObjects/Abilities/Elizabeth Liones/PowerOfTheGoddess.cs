﻿using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Elizabeth_Liones
{
    public class PowerOfTheGoddess : Ability, IClickable
    {
        private const int Heal = 25;
        public PowerOfTheGoddess() : base(AbilityType.Ultimatum, "Power of the goddess", 6)
        {
        }

        public override List<HexCell> GetRangeCells() => HexMapDrawer.Instance.Cells;
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyFriendsOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} leczy wszystkich sojuszników (w tym siebie) na mapie za {Heal} HP.

Czas odnowienia: {Cooldown}";

        public void Click()
        {
            Active.MakeAction();
            GetTargetsInRange().GetCharacters().ForEach(c => ParentCharacter.Heal(c, Heal));
            Finish();
        }
    }
}