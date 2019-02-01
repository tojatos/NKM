using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity;
using Unity.Hex;

namespace NKMCore.Abilities.Ryuko_Matoi
{
    public class FiberDecapitation : Ability, IClickable, IUseableCharacter
    {
        private const int PhysicalDefenseDecrease = 15;
        private const int TargetCellOffset = 3;
        private const int Range = 6;
        private const int Damage = 20;
        public FiberDecapitation(Game game) : base(game, AbilityType.Normal, "Fiber Decapitation", 3)
        {
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
			CanUseOnGround = false;
        }
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StraightLine);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner).FindAll(c =>
		{
			//check if target move cell is free
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(c);
            HexCell moveCell = c.GetCell(direction, TargetCellOffset);
            return moveCell != null && moveCell.IsFreeToStand;
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
				Active.SelectDrawnCell(moveCell).AddHighlight(Highlights.BlueTransparent);
			});
		}

		public void Use(Character targetCharacter)
		{
			ParentCharacter.TryToTakeTurn();
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