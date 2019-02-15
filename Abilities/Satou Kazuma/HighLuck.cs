using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Satou_Kazuma
{
    public class HighLuck : Ability
    {
        private const int CriticalStrikePercentChance = 25;
        public HighLuck(Game game) : base(game, AbilityType.Passive, "High luck")
        {
            OnAwake += () => ParentCharacter.BeforeAttack += (character, damage) =>
            {
                int r = Random.Get(Name, 1, 101);
                if (r <= 25) damage.Value *= 2;
            };
        }

        public override string GetDescription() => 
$@"{ParentCharacter.FirstName()} ma {CriticalStrikePercentChance}% szans na trafienie krytyczne przy zadawaniu obrażeń,
podwajając te obrażenia.";
    }
}