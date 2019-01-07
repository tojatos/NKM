﻿using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Roronoa_Zoro
{
	public class OniGiri : Ability, IClickable, IUseable
	{
		private const int Range = 4;

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
			Active.Prepare(this, GetTargetsInRange());
			//Show highlights on move cells
			GetTargetsInRange().ForEach(c =>
			{
				HexDirection direction = ParentCharacter.ParentCell.GetDirection(c);
                HexCell moveCell = c.GetCell(direction, 2);
                Active.SelectDrawnCell(moveCell).AddHighlight(Highlights.BlueTransparent);
			});
			Active.PlayAudio("oni");
		}
	    public void Use(List<HexCell> cells) => Use(cells[0].CharactersOnCell[0]);

		private void Use(Character targetCharacter)
		{
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(targetCharacter.ParentCell);
			HexCell moveCell = targetCharacter.ParentCell.GetCell(direction, 2);
			ParentCharacter.MoveTo(moveCell);
			var damage = new Damage(ParentCharacter.AttackPoints.Value, DamageType.Physical);
			ParentCharacter.Attack(this, targetCharacter, damage);
			Active.PlayAudio("giri");
			Finish();
		}
	}
}
