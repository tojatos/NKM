using System.Collections.Generic;
using System.Linq;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Effects
{
    public class BlackBlood : Effect
    {
        private const int Damage = 10;
        private readonly Character _characterThatAttacks;

        public BlackBlood(Character characterThatAttacks, Character effectTarget, int cooldown = -1) : base(cooldown,
            effectTarget, "Black Blood")
        {
            _characterThatAttacks = characterThatAttacks;
            Type = effectTarget.Owner == characterThatAttacks.Owner ? EffectType.Positive : EffectType.Negative;
            ParentCharacter.BeforeParentDamage += () =>
            {
                List<Character> enemiesInRange =
                    ParentCharacter.ParentCell.GetNeighbors(1).Select(c => c.CharacterOnCell).Where(c => c != null).ToList();
//                if(effectTarget.Owner != characterThatAttacks.Owner) enemiesInRange.Add(effectTarget); TODO: infinite loop
                enemiesInRange.ForEach(enemy => characterThatAttacks.Attack(enemy, AttackType.Magical, Damage));
            };
        }

        public override string GetDescription()
        {
            return
                $"Zadaje {10} obrażeń magicznych przy otrzymaniu obrażeń wszystkim wrogom gracza {_characterThatAttacks.Owner.Name} w zasięgu 1.\nCzas do zakończenia efektu: {CurrentCooldown}";
        }
    }
}