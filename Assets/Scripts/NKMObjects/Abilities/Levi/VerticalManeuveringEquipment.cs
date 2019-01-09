using System.Collections.Generic;
using Animations;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Levi
{
    public class VerticalManeuveringEquipment : Ability, IClickable, IUseable
    {
        private const int Range = 7;
        private const int MoveTargetRange = 7;
        public VerticalManeuveringEquipment(Game game) : base(game, AbilityType.Normal, "Vertical Maneuvering Equipment", 2)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c => c.Type == HexCell.TileType.Wall);
        private List<HexCell> GetMoveTargets(HexCell cell) =>
            cell.GetNeighbors(Owner.Owner, MoveTargetRange, SearchFlags.StraightLine).FindAll(e => e.IsFreeToStand);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} zaczepia się ściany w zasięgu {Range} i przemieszcza się o max. {MoveTargetRange} pól.";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(List<HexCell> cells)
        {
            HexCell cell = cells[0];
            if (cell.Type == HexCell.TileType.Wall)
            {
                Active.Prepare(this, GetMoveTargets(cell));
                AnimationPlayer.Add(new MoveTo(ParentCharacter.CharacterObject.transform, Active.SelectDrawnCell(cell).transform.position, 0.13f));
            }
            else
            {
                ParentCharacter.MoveTo(cell);
                Finish();
            }
        }

        public override void Cancel()
        {
            base.Cancel();
            AnimationPlayer.Add(new MoveTo(ParentCharacter.CharacterObject.transform, Active.SelectDrawnCell(ParentCharacter.ParentCell).transform.position, 0.13f));
        }
    }
}