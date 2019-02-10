using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Nibutani_Shinka
{
    public class SummerBreeze : Ability, IClickable, IUseableCharacter
    {
        private const int Range = 6;
        private const int Damage = 15;
        private const int KnockbackAmount = 4;
        private const int StunDuration = 1;
        public SummerBreeze(Game game) : base(game, AbilityType.Normal, "Summer Breeze", 3)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StraightLine | SearchFlags.StopAtWalls);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        public override string GetDescription() =>
$@"Przywołuje Letnią Bryzę, która odrzuca wybranego wroga o {KnockbackAmount} pól w tył.
Jeśli wróg wpadnie na ścianę lub inną postać, zostanie ogłuszony na {StunDuration} turę i otrzyma {Damage} obrażeń magicznych.

Czas odnowienia: {Cooldown}";


        public void Click() => Active.Prepare(this, GetTargetsInRange());
        public void Use(Character target)
        {
			ParentCharacter.TryToTakeTurn();
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
                if (!c.IsFreeToStand)
                {
                    character.Effects.Add(new Stun(Game, StunDuration, character, Name));
                    ParentCharacter.Attack(this, character, new Damage(Damage, DamageType.Magical));
                    break;
                }

                lastCell = c;
            }
            character.MoveTo(lastCell);
        }
    }
}