using System.Linq;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Yoshino
{
    public class Zadkiel : EnableableAbility
    {
        private const int StunDuration = 1;
        private const int HitDurability = 3;
        private const int TimeDurability = 5;

        private bool _isEnabled;
        private int _currentHitRemains;
        private int _currentTimeDurability;
        
        public Zadkiel()
        {
            Name = "Zadkiel";
            Cooldown = 3;
            CurrentCooldown = 0;
            Type = AbilityType.Normal;
        }

        public override string GetDescription() => 
$@"{ParentCharacter.Name} osłania się swoim aniołem tworząc zbroję pochłaniającą podstawowe ataki przeciwników.
Każdy wróg, który zaatakuje {ParentCharacter.Name} zostanie ogłuszony na {StunDuration} turę.
Zbroja niszczy się po otrzymaniu {HitDurability} ciosów bądź po {TimeDurability} fazach.
Wraz ze zniszczeniem zbroi, uaktywnia się umiejętność bierna {ParentCharacter.Name}.";

        protected override void Use()
        {
            Active.MakeAction();

            _isEnabled = true;
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
            _isEnabled = false;
            //TODO: unsubscribe
            
            var runablePassive = ParentCharacter.Abilities.SingleOrDefault(a => a.Type == AbilityType.Passive && a is IRunable) as IRunable;
            runablePassive?.Run();
        }

        public override bool IsEnabled => _isEnabled;
    }
}