﻿using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Monkey_D._Luffy
{
    public class GearSecond : Ability, IClickable, IEnableable
    {
        private const int SpeedIncrease = 3;
        private const int Duration = 2;
        
        private int _currentDuration;
        public GearSecond() : base(AbilityType.Ultimatum, "Gear Second", 5)
        {
            OnAwake += () =>
            {
                Active.Phase.PhaseFinished += () =>
                {
                    if (!IsEnabled) return;
                    _currentDuration++;
                    if (_currentDuration <= Duration) return;
                    
                    IsEnabled = false;
                    Ability normalAbility = ParentCharacter.Abilities.Find(a => a.Type == AbilityType.Normal);
                    if (normalAbility is IEnchantable) ((IEnchantable) normalAbility).IsEnchanted = false;
                };
            };
        }

        public override string GetDescription() =>
$@"{ParentCharacter.Name} przyśpiesza przepływ krwi w swoim ciele,
zwiększając swoją szybkość o {SpeedIncrease} i ulepszając swoją zwykłą umiejętność na {Duration} następne fazy.";

        public void Click()
        {
            ParentCharacter.Effects.Add(new StatModifier(Duration+1, SpeedIncrease, ParentCharacter, StatType.Speed, Name));
            Ability normalAbility = ParentCharacter.Abilities.Find(a => a.Type == AbilityType.Normal);
            if (normalAbility is IEnchantable) ((IEnchantable) normalAbility).IsEnchanted = true;
            IsEnabled = true;
            Finish();
        }

        public bool IsEnabled { get; private set; }
    }
}