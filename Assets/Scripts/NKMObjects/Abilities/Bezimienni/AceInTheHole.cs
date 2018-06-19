using NKMObjects.Templates;

namespace NKMObjects.Abilities.Bezimienni
{
    public class AceInTheHole : Ability
    {
        private const int HpPercentActivate = 50;
        private int _damageThisTurn;
        public bool HasFreeAbility { get; set; }
        public AceInTheHole()
        {
            Name = "Ace in the hole";
            Type = AbilityType.Passive;
        }
        public override string GetDescription()
        {
            return
                "Jeśli Bezimienni otrzymają na raz (podczas ruchu jednej postaci) obrażenia wynoszące więcej niż 40% ich maksymalnego HP,\n" +
                "będą oni mogli w swoim następnym ruchu użyć jednej ze swoich umiejętności, niezależnie od jej CD.\n" +
                $"{Name} nie wpływa na liczenie faktycznego CD umiejętności.";
        }

        public override void Awake()
        {
            ParentCharacter.AfterBeingDamaged += damage =>
            {
                if (HasFreeAbility) return;
                _damageThisTurn += damage.Value;
                if (_damageThisTurn > ParentCharacter.HealthPoints.Value * (HpPercentActivate / 100f)) HasFreeAbility = true;
            };
            Active.Turn.TurnFinished += () => _damageThisTurn = 0;

        }
    }
}