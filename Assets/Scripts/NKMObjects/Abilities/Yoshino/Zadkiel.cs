﻿using System.Linq;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Yoshino
{
    public class Zadkiel : Ability, IEnableable, IClickable
    {
        private const int StunDuration = 1;
        private const int HitDurability = 3;
        private const int TimeDurability = 5;

        private int _currentHitRemains;
        private int _currentTimeDurability;
        
        public Zadkiel() : base(AbilityType.Normal, "Zadkiel", 3)
        {
//            Name = "Zadkiel";
//            Cooldown = 3;
//            CurrentCooldown = 0;
//            Type = AbilityType.Normal;
        }

        public override string GetDescription() => 
$@"{ParentCharacter.Name} osłania się swoim aniołem tworząc zbroję pochłaniającą podstawowe ataki przeciwników.
Każdy wróg, który zaatakuje {ParentCharacter.Name} zostanie ogłuszony na {StunDuration} turę.
Zbroja niszczy się po otrzymaniu {HitDurability} ciosów bądź po {TimeDurability} fazach.
Wraz ze zniszczeniem zbroi, uaktywnia się umiejętność bierna {ParentCharacter.Name}.";

        public void ImageClick()
        {
            Active.MakeAction();

            IsEnabled = true;
            _currentHitRemains = HitDurability;
            _currentTimeDurability = TimeDurability;

            ParentCharacter.BeforeBeingBasicAttacked += (character, damage) =>
            {
                if (!character.IsEnemy) return;
                damage.Value = 0;
                character.Effects.Add(new Stun(StunDuration, character, Name));
                _currentHitRemains--;
                if (_currentHitRemains <= 0) Disable();

            };

            Active.Phase.PhaseFinished += () =>
            {
                _currentTimeDurability--;
                if (_currentTimeDurability <= 0) Disable();
            };
            
            //TODO: unsubscribe from events after disabling the ability, maybe Disable()
            
            OnUseFinish();
        }

        private void Disable()
        {
            IsEnabled = false;
            //TODO: unsubscribe
            
            var runablePassive = ParentCharacter.Abilities.SingleOrDefault(a => a.Type == AbilityType.Passive && a is IRunable) as IRunable;
            runablePassive?.Run();
        }

        public bool IsEnabled { get; private set; }
    }
}