﻿using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Derieri
{
    public class GapCloser : Ability, IClickable, IUseable
    {
        private const int MaxDistanceFromTarget = 2;
        private const int Range = 9;
        public GapCloser() : base(AbilityType.Ultimatum, "Gap closer", 4)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} przemieszcza się w pole oddalone max. {MaxDistanceFromTarget} jednostki od wcześniej zaatakowanego wroga.
Dodatkowo otrzymuje możliwość podstawowego ataku.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
        public override List<HexCell> GetTargetsInRange()
        {
			ComboStar passiveAbility = ParentCharacter.Abilities.OfType<ComboStar>().SingleOrDefault();
            Character targetCharacter = passiveAbility?.ComboCharacter;
            return targetCharacter==null ? new List<HexCell>() : GetRangeCells().FindAll(c => c.CharacterOnCell == targetCharacter).FindAll(c => c.GetNeighbors(MaxDistanceFromTarget).Any(cc => cc.IsFreeToStand));
        }

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(List<HexCell> cells)
        {
            if (cells[0].CharacterOnCell != null)
            {
                Active.Prepare(this, cells[0].GetNeighbors(MaxDistanceFromTarget).FindAll(c => c.IsFreeToStand));
            }
            else
            {
                ParentCharacter.MoveTo(cells[0]);
                ParentCharacter.HasFreeAttackUntilEndOfTheTurn = true;
                Finish();
            }
        }
    }
}