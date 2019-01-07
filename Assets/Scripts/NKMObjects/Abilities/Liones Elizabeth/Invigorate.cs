﻿using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Liones_Elizabeth
{
    public class Invigorate : Ability, IClickable, IUseable
    {
        private const int Range = 5;
        private const int Heal = 6;
        private const int Duration = 3;
        public Invigorate(Game game) : base(game, AbilityType.Normal, "Invigorate", 3)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereFriendsOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} nakłada na sojusznika zaklęcie, które leczy go o {Heal} przez {Duration} fazy.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(List<HexCell> cells)
        {
            cells[0].CharactersOnCell[0].Effects
                .Add(new HealOverTime(Game, ParentCharacter, Heal, Duration, cells[0].CharactersOnCell[0], Name));
            Finish();
        }
    }
}
