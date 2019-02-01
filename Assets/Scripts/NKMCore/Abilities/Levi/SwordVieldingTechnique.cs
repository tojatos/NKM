using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity;
using Unity.Animations;
using Unity.Hex;

namespace NKMCore.Abilities.Levi
{
    public class SwordVieldingTechnique : Ability, IClickable, IUseableCell
    {
        private const int Range = 7;
        private const int MoveTargetRange = 7;
        public SwordVieldingTechnique(Game game) : base(game, AbilityType.Ultimatum, "Sword-Vielding Technique", 5)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c => c.Type == HexCell.TileType.Wall);
        private List<HexCell> GetMoveTargets(HexCell cell) =>
            cell.GetNeighbors(Owner.Owner, MoveTargetRange, SearchFlags.StraightLine).FindAll(e => e.IsFreeToStand);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} zaczepia się ściany w zasięgu {Range} i przemieszcza się o max. {MoveTargetRange} pól.
W trakcie przemieszczenia się zadaje podstawowe obrażenia osobom w zasięgu ataku.";

        public void Click() => Active.Prepare(this, GetTargetsInRange());
        private HexCell _theWall;

        public void Use(HexCell cell)
        {
            if (cell.Type == HexCell.TileType.Wall)
            {
                _theWall = cell;
                Active.Prepare(this, GetMoveTargets(cell));
                AnimationPlayer.Add(new MoveTo(HexMapDrawer.Instance.GetCharacterObject(ParentCharacter).transform, Active.SelectDrawnCell(cell).transform.position, 0.13f));
            }
            else
            {
                ParentCharacter.TryToTakeTurn();
                ParentCharacter.MoveTo(cell);
                List<HexCell> realMoveCells = _theWall.GetLine(_theWall.GetDirection(cell), _theWall.GetDistance(cell));
                
                List<Character> targets = realMoveCells.SelectMany(c => ParentCharacter.DefaultGetBasicAttackCells(c))
                    .ToList().WhereEnemiesOf(Owner).GetCharacters().Distinct().ToList();
                targets.ForEach(t => ParentCharacter.Attack(this, t, new Damage(ParentCharacter.AttackPoints.Value, DamageType.Physical)));
                Finish();
            }
        }
        public override void Cancel()
        {
            base.Cancel();
            AnimationPlayer.Add(new MoveTo(HexMapDrawer.Instance.GetCharacterObject(ParentCharacter).transform, Active.SelectDrawnCell(ParentCharacter.ParentCell).transform.position, 0.13f));
        }
    }
}