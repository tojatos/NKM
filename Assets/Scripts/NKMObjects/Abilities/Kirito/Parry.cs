using NKMObjects.Templates;

namespace NKMObjects.Abilities.Kirito
{
    public class Parry : Ability
    {
        private const int DodgeChancePercent = 25;

        public Parry() : base(AbilityType.Passive, "Parry")
        {
            OnAwake += () => ParentCharacter.BeforeBeingBasicAttacked += (character, damage) =>
            {
//                var r = UnityEngine.Random.Range(1, 101);
                int r = NKMRandom.Get(Name, 1, 101);
                if (r <= DodgeChancePercent) damage.Value = 0;
            };
        }

        public override string GetDescription() =>
            $"{ParentCharacter.Name} ma {DodgeChancePercent}% szans na uniknięcie podstawowego ataku wrogiej postaci.";
    }
}