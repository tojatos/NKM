using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Monkey_D._Luffy
{
    public class GomuGomuNoMi : Ability, IClickable, IUseable, IEnchantable
    {
        private const int Range = 7;
        private const int BazookaRange = 3;
        private const int BazookaDamage = 17;
        private const int BazookaKnockback = 8;
        private const int BazookaCooldown = 3;
        private const int PistolDamage = 15;
        private const int JetBazookaKnockback = 14;
        private const int JetBazookaDamage = 23;
        private const int JetPistolDamage = 19;
        public GomuGomuNoMi(Game game) : base(game, AbilityType.Normal, "Gomu Gomu no Mi", 2)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StraightLine);

        public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c =>
            c.CharactersOnCell.Any(ch => ch.IsEnemyFor(Owner))|| !ParentCharacter.IsGrounded && c.Type == HexTileType.Wall);

        public override string GetDescription() =>
$@"{ParentCharacter.Name} używa umiejętności Gumowego Owocu w zależności od celu:

<i>Wróg w zasięgu {BazookaRange}</i>
<b>Bazooka</b>
{ParentCharacter.Name} wyciąga obie ręce do tyłu, a następnie ciska je do przodu,
zadając przeciwnikowi {BazookaDamage} obrażeń fizycznych i odrzucając go {BazookaKnockback} pól dalej.

<i>Wróg w dalszym zasięgu</i>
<b>Pistol</b>
{ParentCharacter.Name} wyciąga rękę do tyłu, a następnie ciska ją do przodu,
zadając przeciwnikowi {PistolDamage} obrażeń fizycznych.

<i>Ściana</i>
<b>Rocket</b>
{ParentCharacter.Name} łapie się ściany, wybijając się za nią o tyle pól, ile ma do ściany.

Umiejętność <b>{Name}</b> może zostać ulepszona:

<b>Bazooka</b>
Obrażenia: {JetBazookaDamage}
Odrzut: {JetBazookaKnockback}

<b>Pistol</b>
Obrażenia: {JetPistolDamage}

Zasięg: {Range}    Czas odnowienia: {Cooldown} ({BazookaCooldown}, jeżeli użyje Bazooki)";

        public void Click()
        {
            Active.Prepare(this, GetTargetsInRange());
            Active.PlayAudio("gomu gomu no");
        }

        public void Use(List<HexCell> cells)
        {
            HexCell cell = cells[0];
            if (cell.Type == HexTileType.Wall) Rocket(cell);
            else
            {
                Character enemy = cell.CharactersOnCell[0];
                if (GetNeighboursOfOwner(BazookaRange).Contains(cell)) Bazooka(enemy);
                else Pistol(enemy);
            }
        }

        private void Pistol(Character enemy)
        {
            ParentCharacter.Attack(this, enemy,new Damage(IsEnchanted ? JetPistolDamage : PistolDamage, DamageType.Physical));
            Active.PlayAudio("pistol");
            Finish();
        }

        private void Bazooka(Character enemy)
        {
            ParentCharacter.Attack(this, enemy, new Damage(IsEnchanted?JetBazookaDamage:BazookaDamage, DamageType.Physical));
            if(!enemy.IsAlive) return;
            HexDirection direction = ParentCharacter.ParentCell.GetDirection(enemy.ParentCell);
            ThrowCharacter(enemy, direction, IsEnchanted?JetBazookaKnockback:BazookaKnockback);
            Finish(BazookaCooldown);
        }
        
        private void Rocket(HexCell cell)
        {
            HexDirection direction = ParentCharacter.ParentCell.GetDirection(cell);
            int distance = ParentCharacter.ParentCell.GetDistance(cell);
            if(distance<=0) return;
            Active.PlayAudio("gomu gomu no rocket effect");
            ThrowCharacter(ParentCharacter, direction, distance*2);
            Finish();
        }

        private static void ThrowCharacter(Character character, HexDirection direction, int distance)
        {
            List<HexCell> line = character.ParentCell.GetLine(direction, distance);
            line.Reverse();
            HexCell targetCell = line.FirstOrDefault(c => c.IsFreeToStand);
            if(targetCell==null) return;
            character.MoveTo(targetCell);
        }


        public bool IsEnchanted { get; set; }
    }
}
