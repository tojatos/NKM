using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Nibutani_Shinka
{
    public class SummerBreeze : Ability, IClickable, IUseable
    {
        private const int Range = 6;
        private const int Damage = 15;
        private const int KnockbackAmount = 6;
        private const int StunDuration = 1;
        public SummerBreeze() : base(AbilityType.Normal, "Summer Breeze", 2)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

        public override string GetDescription() =>
$@"Przywołuje Letnią Bryzę, która odrzuca wybranego wroga o {KnockbackAmount} pól w tył.
Jeśli wróg wpadnie na ścianę lub inną postać, zostanie ogłuszony na {StunDuration} turę i otrzyma {Damage} obrażeń magicznych.";


        public void Click() => Active.Prepare(this, GetTargetsInRange());
        public void Use(List<HexCell> cells)
        {
            Character target = cells[0].CharacterOnCell;
            HexDirection direction = ParentCharacter.ParentCell.GetDirection(target.ParentCell);
            Knockback(target, direction);
            Finish();
        }

        private void Knockback(Character character, HexDirection direction)
        {
            List<HexCell> line = character.ParentCell.GetLine(direction, KnockbackAmount);
            HexCell lastCell = character.ParentCell;
            foreach (HexCell c in line)
            {
                if (c.Type == HexTileType.Wall || c.CharacterOnCell != null)
                {
                    character.Effects.Add(new Stun(StunDuration, character, Name));
                    ParentCharacter.Attack(this, character, new Damage(Damage, DamageType.Magical));
                    break;
                }

                lastCell = c;
            }
            character.MoveTo(lastCell);
        }
    }
}