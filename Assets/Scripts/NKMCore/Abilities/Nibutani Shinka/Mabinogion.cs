using System.Collections.Generic;
using System.Linq;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Nibutani_Shinka
{
    public class Mabinogion : Ability, IEnchantable, IRunnable
    {
        private int HealAmount => IsEnchanted ? 12 : 4;
        private int ShieldAmount => IsEnchanted ? 3 : 1;
        private const int EnchantedSpeedAmount = 3;
        private const int Radius = 4;
        public Mabinogion(Game game) : base(game, AbilityType.Passive, "Mabinogion")
        {
            OnAwake += () => Active.Turn.TurnFinished += character =>
            {
                if (character == ParentCharacter) Run();
            };
        }

        private void TryAddingShield(Character character)
        {
            if(character.Shield.Value >= ShieldAmount) return;
            character.Shield.Value = ShieldAmount;
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Radius).AddOne(ParentCharacter.ParentCell);
        public override List<HexCell> GetTargetsInRange() => IsEnchanted ? GetRangeCells().WhereFriendsOf(Owner) : GetRangeCells().WhereCharacters();

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
            List<Character> friendsInRange = GetTargetsInRange().WhereFriendsOf(Owner).GetCharacters();
            friendsInRange.ForEach(TryAddingShield);
            if(IsEnchanted) friendsInRange.ForEach(f =>
            {
               if(f.Effects.Any(e => e.Name == Name))  return; // prevent speed stacking
                f.Effects.Add(new StatModifier(Game, 1, EnchantedSpeedAmount, f, StatType.Speed, Name));
            });
        }
    }
}
