using NKMObjects.Templates;

namespace NKMObjects.Abilities.Nibutani_Shinka
{
    public class FairyOfLove : Ability, IClickable, IEnableable
    {
        private const int Duration = 3;
        private int _currentDuration;
        public FairyOfLove() : base(AbilityType.Ultimatum, "Fairy of Love", 6)
        {
            OnAwake += () => Active.Phase.PhaseFinished += () =>
            {
                if (!IsEnabled) return;
                _currentDuration++;
                if (_currentDuration <= Duration) return;

                IsEnabled = false;
                Ability normalAbility = ParentCharacter.Abilities.Find(a => a.Type == AbilityType.Passive);
                if (normalAbility is IEnchantable) ((IEnchantable) normalAbility).IsEnchanted = false;
            };
        }

        public override string GetDescription() =>
$@"{ParentCharacter.Name} wzmacnia swoją umiejętność bierną.
Czas trwania: {Duration}    Czas odnowienia: {Cooldown}";

        public void Click()
        {
            Ability normalAbility = ParentCharacter.Abilities.Find(a => a.Type == AbilityType.Passive);
            if (normalAbility is IEnchantable) ((IEnchantable) normalAbility).IsEnchanted = true;
            IsEnabled = true;
        }

        public bool IsEnabled { get; set; }
    }
}