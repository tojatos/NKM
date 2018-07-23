using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Levi
{
    public class VerticalManeuveringEquipment : Ability, IClickable, IUseable
    {
        private const int Range = 7;
        private const int MoveTargetRange = 7;
        public VerticalManeuveringEquipment() : base(AbilityType.Normal, "Vertical Maneuvering Equipment", 2)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c => c.Type == HexTileType.Wall);
        private List<HexCell> GetMoveTargets(HexCell cell) =>
            cell.GetNeighbors(MoveTargetRange, SearchFlags.StraightLine).FindAll(e => e.IsFreeToStand);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} zaczepia się ściany w zasięgu {Range} i przemieszcza się o max. {MoveTargetRange} pól.";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(List<HexCell> cells)
        {
            HexCell cell = cells[0];
            if (cell.Type == HexTileType.Wall) Active.Prepare(this, GetMoveTargets(cell));
            else
            {
                ParentCharacter.MoveTo(cell);
                Finish();
            }
        }
    }
}