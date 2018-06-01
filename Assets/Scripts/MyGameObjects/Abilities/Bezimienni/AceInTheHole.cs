using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Bezimienni
{
    public class AceInTheHole : Ability
    {
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
            ParentCharacter.OnParentDamage += value =>
            {
                if (HasFreeAbility) return;
                _damageThisTurn += value;
                if (_damageThisTurn > ParentCharacter.HealthPoints.BaseValue * (4 / 10f)) HasFreeAbility = true;
            };
            Active.Turn.TurnFinished += () => _damageThisTurn = 0;

        }
    }
}