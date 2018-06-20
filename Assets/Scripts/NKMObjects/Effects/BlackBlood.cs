using System.Collections.Generic;
using System.Linq;
using NKMObjects.Templates;

namespace NKMObjects.Effects
{
    public class BlackBlood : Effect
    {
        private const int Damage = 10;
        private const int Range = 2;
        private readonly Character _characterThatAttacks;
        private bool _wasActivatedOnce;

        public BlackBlood(Character characterThatAttacks, Character effectTarget, int cooldown = -1) : base(cooldown,
            effectTarget, "Black Blood")
        {
            _characterThatAttacks = characterThatAttacks;
            Type = effectTarget.Owner == characterThatAttacks.Owner ? EffectType.Positive : EffectType.Negative;
            Character.DamageDelegate tryToActivateEffect = d => 
            {
                if(_wasActivatedOnce) return;//prevent infinite loop
                _wasActivatedOnce = true; 
                List<Character> enemiesInRange =
                    ParentCharacter.ParentCell.GetNeighbors(Range).Select(c => c.CharacterOnCell).Where(c => c != null && c.Owner != characterThatAttacks.Owner).ToList();
                if(effectTarget.Owner != characterThatAttacks.Owner) enemiesInRange.Add(effectTarget);
                enemiesInRange.ForEach(enemy =>
                {
                    var damage = new Damage(Damage, DamageType.Magical);
                    characterThatAttacks.Attack(enemy, damage);
                });
                _wasActivatedOnce = false;
            };
            ParentCharacter.BeforeBeingDamaged += tryToActivateEffect;
            OnRemove += () => ParentCharacter.BeforeBeingDamaged -= tryToActivateEffect;
        }

        public override string GetDescription()
        {
            return
                $"Zadaje {10} obrażeń magicznych przy otrzymaniu obrażeń wszystkim wrogom gracza {_characterThatAttacks.Owner.Name} w zasięgu 1.\nCzas do zakończenia efektu: {CurrentCooldown}";
        }
    }
}