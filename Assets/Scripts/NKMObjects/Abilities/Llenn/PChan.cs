using NKMObjects.Templates;

namespace NKMObjects.Abilities.Llenn
{
    public class PChan : Ability
    {
        private const int SpeedIncrease = 2;
        public PChan() : base(AbilityType.Passive, "P-Chan")
        {
            OnAwake += () =>
                Owner.Characters.ForEach(c => c.OnDeath += () => ParentCharacter.Speed.Value = ParentCharacter.Speed.RealValue + SpeedIncrease);
        }

        public override string GetDescription() =>
$@"Każda śmierć przyjaznej postaci zwiększa wolę do działania {ParentCharacter.Name},
co daje jej {SpeedIncrease} szybkości na stałe.";
    }
}