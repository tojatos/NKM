using System;
using System.Collections.Generic;
using System.Linq;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Roronoa_Zoro
{
	public class OniGiri : Ability
	{
		private const int AbilityRange = 7;

		public OniGiri()
		{
			Name = "Oni Giri";
			Cooldown = 3;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
		}
		public override string GetDescription() => $@"{ParentCharacter.Name} zadaje obrażenia podstawowe wybranej postaci, lądując 2 pola za nią.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange, true, false, true);
		protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			var cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			if (cellRange.Count == 0)
			{
				throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
			}
			if(!cellRange.Any(c =>
			{
				var direction = ParentCharacter.ParentCell.GetDirection(c);
				var moveCell = c.GetCell(direction, 2);
				return moveCell != null && moveCell.Type != HexTileType.Wall;
			}))
			{
				throw new Exception("Nie ma gdzie się ruszyć!");
			}
		}
		protected override void Use()
		{
			var cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			var canUseAbility = Active.Prepare(Action.UseAbility, cellRange);
			try
			{
				if (!canUseAbility)
				{
					throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
				}
				cellRange.ForEach(c =>
				{
					var direction = ParentCharacter.ParentCell.GetDirection(c);
					var moveCell = c.GetCell(direction, 2);
					if (moveCell == null || moveCell.Type == HexTileType.Wall)
					{
						throw new Exception("Nie ma gdzie się ruszyć!");
					}
					moveCell.ToggleHighlight(HiglightColor.WhiteOrange);
				});
				cellRange.ForEach(c => c.ToggleHighlight(HiglightColor.Red));
				Active.Ability = this;
				Active.PlayAudio("oni");
			}
			catch (Exception e)
			{
				MessageLogger.DebugLog(e.Message);
				OnFailedUseFinish();
			}
		}
		public override void Use(Character targetCharacter)
		{
			var direction = ParentCharacter.ParentCell.GetDirection(targetCharacter.ParentCell);
			var moveCell = targetCharacter.ParentCell.GetCell(direction, 2);
			ParentCharacter.Attack(targetCharacter, AttackType.Physical, ParentCharacter.AttackPoints.Value);
			ParentCharacter.Move(moveCell);
			Active.PlayAudio("giri");
			OnUseFinish();
		}
	}
}
