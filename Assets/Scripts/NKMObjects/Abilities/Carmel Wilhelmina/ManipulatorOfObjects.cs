using System.Linq;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Carmel_Wilhelmina
{
    public class ManipulatorOfObjects : Ability
    {
        private const int EffectTimeout = 2;
        private const int Duration = 1;
        public ManipulatorOfObjects() : base(AbilityType.Passive, "Manipulator of Objects")
        {
            OnAwake += () => ParentCharacter.BeforeBasicAttack += (character, damage) =>
            {
                if (!character.IsEnemyFor(Owner) || character.Effects.Any(e => e.Name == Name)) return;
                character.Effects.Add(new MovementDisability(Duration, character, Name));
                character.Effects.Add(new Effects.Empty(EffectTimeout, character, Name,
                    $"{Name} nie może zostać nałożony na tą postać."));
            };
        }

        public override string GetDescription() =>
$@"Podstawowe ataki tej postaci unieruchamiają wroga na {Duration} fazę.
Efekt nie może zostać nałożony ponownie przez {EffectTimeout} fazy z rzędu na ten sam cel.";
    }
}