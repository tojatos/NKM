using System.Collections.Generic;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Sakai_Yuuji
{
    public class Grammatica : Ability, IClickable, IUseable
    {
        private const int HealthPercentDamage = 25;
        private const int Range = 7;
        public Grammatica() : base(AbilityType.Ultimatum, "Grammatica", 5)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
            CanUseOnGround = false;
        }

        public override string GetDescription() =>
            $@"{ParentCharacter.Name} teleportuje się do wybranej postaci.
Jeśli jest ona przeciwnikiem, to zadaje jej {HealthPercentDamage}% max. HP jako obrażenia nieuchronne.
Następnie wraca z wybraną postacią w miejsce, na którym stał przed użyciem tej umiejętności, a wybrana postać pojawia się o jedno pole przed nim.
Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range, SearchFlags.StraightLine);

        public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c =>
            c.CharacterOnCell != null && ParentCharacter.ParentCell.GetCell(ParentCharacter.ParentCell.GetDirection(c), 1).IsFreeToStand);

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

        private void Use(Character character)
        {
            if (character.IsEnemyFor(Owner))
            {
                int dmg = (int)(character.HealthPoints.BaseValue * HealthPercentDamage / 100f);
                ParentCharacter.Attack(this, character, new Damage(dmg, DamageType.True));
            }
            character.MoveTo(ParentCharacter.ParentCell.GetCell(ParentCharacter.ParentCell.GetDirection(character.ParentCell), 1));
            Finish();
        }
    }
}