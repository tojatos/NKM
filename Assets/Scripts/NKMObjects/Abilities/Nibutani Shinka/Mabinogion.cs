﻿using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Nibutani_Shinka
{
    public class Mabinogion : Ability, IEnchantable, IRunnable
    {
        private int HealAmount => IsEnchanted ? 12 : 4;
        private int ShieldAmount => IsEnchanted ? 3 : 1;
        private const int EnchantedSpeedAmount = 3;
        private const int Radius = 4;
        public Mabinogion() : base(AbilityType.Passive, "Mabinogion")
        {
            OnAwake += () => Active.Turn.TurnFinished += character =>
            {
                if (character == ParentCharacter) Run();
            };
        }

        private void TryAddingShield(NKMCharacter character)
        {
            if(character.Shield.Value >= ShieldAmount) return;
            character.Shield.Value = ShieldAmount;
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Radius).AddOne(ParentCharacter.ParentCell);
        public override List<HexCell> GetTargetsInRange() => IsEnchanted ? GetRangeCells().WhereOnlyFriendsOf(Owner) : GetRangeCells().WhereOnlyCharacters();

        public override string GetDescription() => 
$@"{ParentCharacter.Name} leczy wszystkie postacie w promieniu {Radius} za {HealAmount} HP na końcu swojego ruchu.
Dodatkowo, sojusznicy otrzymuję {ShieldAmount} tarczy, która odnawia się co użycie.

Umiejętność <b>{Name}</b> może zostać ulepszona:

Leczenie: Działa tylko na sojuszników i jest potrojone
Tarcza: Jest potrojona
Dodatkowo, daje wszystkim sojusznikom w zasięgu {EnchantedSpeedAmount} szybkości na fazę.";
        public bool IsEnchanted { get; set; }

        public void Run()
        {
            GetTargetsInRange().GetCharacters().ForEach(c => ParentCharacter.Heal(c, HealAmount));
            List<NKMCharacter> friendsInRange = GetTargetsInRange().WhereOnlyFriendsOf(Owner).GetCharacters();
            friendsInRange.ForEach(TryAddingShield);
            if(IsEnchanted) friendsInRange.ForEach(f =>
            {
               if(f.Effects.Any(e => e.Name == Name))  return; // prevent speed stacking
                f.Effects.Add(new StatModifier(1, EnchantedSpeedAmount, f, StatType.Speed, Name));
            });
        }
    }
}
