﻿using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Sakai_Yuuji
{
    public class DragonTail : Ability, IClickable
    {
        private const int HealRange = 6;
        private const int HealPercent = 25;
        private const int BasicAttackRangeBonus = 3;
        private const int BonusDuration = 3;

        public DragonTail() : base(AbilityType.Normal, "Dragon Tail", 5){}

        public override string GetDescription() =>
$@"{ParentCharacter.Name} poświęca połowę swojego max. HP, aby uleczyć pozostałych pobliskich sojuszników o {HealPercent}% ich max HP.
Ponadto zwiększa zasięg swoich podstawowych ataków o {BasicAttackRangeBonus} na {BonusDuration} fazy.

Promień działania: {HealRange}    Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(HealRange);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyFriendsOf(Owner);

        public void Click()
        {
            Active.MakeAction();
            GetTargetsInRange().GetCharacters().ForEach(c => ParentCharacter.Heal(c, (int)(c.HealthPoints.BaseValue * HealPercent / 100f)));
            ParentCharacter.Effects.Add(new StatModifier(BonusDuration, BasicAttackRangeBonus, ParentCharacter, StatType.BasicAttackRange, Name));
            ParentCharacter.Attack(this, ParentCharacter, new Damage(ParentCharacter.HealthPoints.BaseValue / 2, DamageType.True));
            Finish();
        }
    }
}