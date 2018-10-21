using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Ryuko_Matoi
{
    public class FiberDecapitation : Ability, IClickable, IUseable
    {
        private const int PhysicalDefenseDecrease = 15;
        private const int TargetCellOffset = 3;
        private const int Range = 6;
        private const int Damage = 20;
        public FiberDecapitation() : base(AbilityType.Normal, "Fiber Decapitation", 3)
        {
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
			CanUseOnGround = false;
        }
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range, SearchFlags.StraightLine);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner).FindAll(c =>
		{
			//check if target move cell is free
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(c);
            HexCell moveCell = c.GetCell(direction, TargetCellOffset);
            return moveCell != null && moveCell.Type != HexTileType.Wall && moveCell.CharacterOnCell == null;
		});
        public override string GetDescription() =>
$@"{ParentCharacter.Name} przecina się przez przeciwnika, zmniejszając mu obronę fizyczną na stałe o {PhysicalDefenseDecrease},
a następnie zadjąc mu {Damage} obrażeń fizycznych i lądując {TargetCellOffset} pola za nim.
Zasięg: {Range}    Czas odnowienia: {Cooldown}";
	    
		public void Click()
		{
			Active.Prepare(this, GetTargetsInRange());
			//Show highlights on move cells
			GetTargetsInRange().ForEach(c =>
			{
				HexDirection direction = ParentCharacter.ParentCell.GetDirection(c);
                HexCell moveCell = c.GetCell(direction, TargetCellOffset);
                moveCell.AddHighlight(Highlights.BlueTransparent);
			});
		}
	    public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

		private void Use(NKMCharacter targetCharacter)
		{
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(targetCharacter.ParentCell);
			HexCell moveCell = targetCharacter.ParentCell.GetCell(direction, TargetCellOffset);
			targetCharacter.PhysicalDefense.Value = targetCharacter.PhysicalDefense.RealValue - 15;
			ParentCharacter.MoveTo(moveCell);
			var damage = new Damage(Damage, DamageType.Physical);
			ParentCharacter.Attack(this, targetCharacter, damage);
			Finish();
		}
    }
}