using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Levi
{
    public class AwakenedPower : Ability, IEnableable
    {
        private const int AttackBonus = 7;
        private const int SpeedBonus = 2;
        private const int HealthTresholdPercent = 25;
        public AwakenedPower(Game game) : base(game, AbilityType.Passive, "Awakened Power")
        {
            OnAwake += () =>
            {
                ParentCharacter.OnKill += TryEnabling;
                ParentCharacter.HealthPoints.StatChanged += () =>
                {
                    if (ParentCharacter.HealthPoints.Value <
                        ParentCharacter.HealthPoints.BaseValue * HealthTresholdPercent / 100f)
                        TryEnabling();
                };
            };
        }

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} otrzymuje na stałe {AttackBonus} ataku i {SpeedBonus} szybkości,
kiedy zabije wroga albo jego zdrowie spadnie poniżej {HealthTresholdPercent}%.
Efekt ten aktywuje się tylko raz.";

        public bool IsEnabled { get; private set; }

        private void TryEnabling()
        {
            if(IsEnabled) return;
            IsEnabled = true;
            ParentCharacter.AttackPoints.Value = ParentCharacter.AttackPoints.RealValue + AttackBonus;
            ParentCharacter.Speed.Value = ParentCharacter.Speed.RealValue + SpeedBonus;
        }
    }
}