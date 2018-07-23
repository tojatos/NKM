using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Levi
{
    public class SwordVieldingTechnique : Ability, IClickable, IUseable
    {
        private const int Range = 7;
        private const int MoveTargetRange = 7;
        private int Damage => 30 + ParentCharacter.AttackPoints.Bonus;
        public SwordVieldingTechnique() : base(AbilityType.Ultimatum, "Sword-Vielding Technique", 5)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c => c.Type == HexTileType.Wall);
        private List<HexCell> GetMoveTargets(HexCell cell) =>
            cell.GetNeighbors(MoveTargetRange, SearchFlags.StraightLine).FindAll(e => e.IsFreeToStand);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} zaczepia się ściany w zasięgu {Range} i przemieszcza się o max. {MoveTargetRange} pól.
W trakcie przemieszczenia się zadaje {Damage} obrażeń fizycznych osobom w zasięgu ataku.";

        public void Click() => Active.Prepare(this, GetTargetsInRange());
//        private List<HexCell> _moveCells;
        private HexCell _theWall;

        public void Use(List<HexCell> cells)
        {
            HexCell cell = cells[0];
            if (cell.Type == HexTileType.Wall)
            {
//                _moveCells = GetMoveTargets(cell);
//                Active.Prepare(this, _moveCells);
                _theWall = cell;
                Active.Prepare(this, GetMoveTargets(cell));
            }
            else
            {
//                int removeAfterIndex = _moveCells.FindIndex(c => c == cell);
//                List<HexCell> realMoveCells = _moveCells.GetRange(0, removeAfterIndex);
                List<HexCell> realMoveCells = _theWall.GetLine(_theWall.GetDirection(cell), _theWall.GetDistance(cell));
                
                List<Character> targets = realMoveCells.SelectMany(c => ParentCharacter.DefaultGetBasicAttackCells(c))
                    .ToList().WhereOnlyEnemiesOf(Owner).GetCharacters().Distinct().ToList();
                targets.ForEach(t => ParentCharacter.Attack(this, t, new Damage(Damage, DamageType.Physical)));
                ParentCharacter.MoveTo(cell);
                Finish();
            }
        }
    }
}