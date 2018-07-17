using Extensions;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Liones_Elizabeth
{
    public class ImmenseHealingPowers : Ability
    {
        public ImmenseHealingPowers() : base(AbilityType.Passive, "Immense Healing Powers")
        {

            OnAwake += () => ParentCharacter.BeforeHeal += (Character character, ref int value) =>
            {
                float missingHP = 100 - character.HealthPoints.Value / (float) character.HealthPoints.BaseValue * 100;
                if (missingHP > 50)
                {
                    if (missingHP > 75) value *= 2;
                    else value = (int) (value * 1.5);
                }
            };
        }

        public override string GetDescription() =>
$@"Leczenie {ParentCharacter.FirstName()} jest silniejsze im cel jest bardziej ranny:
0-50% brakującego zdrowia - 0% wzmocnionego leczenia
51-75% brakującego zdrowia - 50% wzmocnionego leczenia
76-100% brakującego zdrowia - 100% wzmocnionego leczenia";
    }
}
