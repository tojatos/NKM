using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Roronoa_Zoro
{
	public class OniGiri : Ability, IClickable, IUseableCharacter
	{
		private const int Range = 4;

		public event Delegates.CellList AfterOniGiriPrepare;
		public event Delegates.Void AfterOniGiri;

		public OniGiri(Game game) : base(game, AbilityType.Normal, "Oni Giri", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
			CanUseOnGround = false;
		}

		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StopAtWalls | SearchFlags.StraightLine);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner).FindAll(c =>
		{
			//check if target move cell is free
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(c);
            HexCell moveCell = c.GetCell(direction, 2);
            return moveCell != null && moveCell.IsFreeToStand;
		});

		public override string GetDescription() => $@"{ParentCharacter.Name} zadaje obrażenia podstawowe wybranej postaci, lądując 2 pola za nią.
Zasięg: {Range}	Czas odnowienia: {Cooldown}";


		public void Click()
		{
			List<HexCell> targetCells = GetTargetsInRange();
			Active.Prepare(this, targetCells);
			AfterOniGiriPrepare?.Invoke(targetCells);
		}

		public void Use(Character targetCharacter)
		{
			ParentCharacter.TryToTakeTurn();
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(targetCharacter.ParentCell);
			HexCell moveCell = targetCharacter.ParentCell.GetCell(direction, 2);
			ParentCharacter.MoveTo(moveCell);
			var damage = new Damage(ParentCharacter.AttackPoints.Value, DamageType.Physical);
			ParentCharacter.Attack(this, targetCharacter, damage);
			AfterOniGiri?.Invoke();
			Finish();
		}
	}
}
