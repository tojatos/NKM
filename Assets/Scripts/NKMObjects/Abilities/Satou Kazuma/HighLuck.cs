using Extensions;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Satou_Kazuma
{
    public class HighLuck : Ability
    {
        private const int CriticalStrikePercentChance = 25;
        public HighLuck() : base(AbilityType.Passive, "High luck")
        {
            OnAwake += () => ParentCharacter.BeforeAttack += (character, damage) =>
            {
                if (UnityEngine.Random.Range(1, 101) <= 25) damage.Value *= 2;
            };
        }

        public override string GetDescription() => 
$@"{ParentCharacter.FirstName()} ma {CriticalStrikePercentChance}% szans na trafienie krytyczne przy zadawaniu obrażeń,
podwajając te obrażenia.";
    }
}