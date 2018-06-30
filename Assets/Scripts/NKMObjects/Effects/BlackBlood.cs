using System.Collections.Generic;
using System.Linq;
using NKMObjects.Templates;

namespace NKMObjects.Effects
{
    public class BlackBlood : Effect
    {
        private readonly Character _characterThatAttacks;
        private bool _wasActivatedOnce;

        public BlackBlood(Character characterThatAttacks, Character effectTarget, int cooldown, int damage, int range) : base(cooldown,
            effectTarget, "Black Blood")
        {
            _characterThatAttacks = characterThatAttacks;
            Type = effectTarget.Owner == characterThatAttacks.Owner ? EffectType.Positive : EffectType.Negative;
            Character.DamageDelegate tryToActivateEffect = d => 
            {
                if(d.Value==0) return;
                if(_wasActivatedOnce) return;//prevent infinite loop
                _wasActivatedOnce = true; 
                List<Character> enemiesInRange =
                    ParentCharacter.ParentCell.GetNeighbors(range).Select(c => c.CharacterOnCell).Where(c => c != null && c.Owner != characterThatAttacks.Owner).ToList();
                if(effectTarget.Owner != characterThatAttacks.Owner) enemiesInRange.Add(effectTarget);
                enemiesInRange.ForEach(enemy =>
                {
                    var dmg = new Damage(damage, DamageType.Magical);
                    characterThatAttacks.Attack(this, enemy, dmg);
                });
                _wasActivatedOnce = false;
            };
            ParentCharacter.BeforeBeingDamaged += tryToActivateEffect;
            OnRemove += () => ParentCharacter.BeforeBeingDamaged -= tryToActivateEffect;
        }

        public override string GetDescription()
        {
            return
                $"Zadaje {10} obrażeń magicznych przy otrzymaniu obrażeń wszystkim wrogom gracza {_characterThatAttacks.Owner.Name} w zasięgu 1.\n" +
                ((CurrentCooldown==int.MaxValue) ? "Efekt ten jest trwa wiecznie." : $"Czas do zakończenia efektu: {CurrentCooldown}");
        }
    }
}