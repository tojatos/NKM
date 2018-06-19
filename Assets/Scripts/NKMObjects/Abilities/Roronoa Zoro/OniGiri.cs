using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Roronoa_Zoro
{
	public class OniGiri : Ability
	{
		private const int AbilityRange = 4;

		public OniGiri()
		{
			Name = "Oni Giri";
			Cooldown = 3;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
		}
		public override string GetDescription() => $@"{ParentCharacter.Name} zadaje obrażenia podstawowe wybranej postaci, lądując 2 pola za nią.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange, SearchFlags.StopAtWalls | SearchFlags.StraightLine);
		protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			if (cellRange.Count == 0)
			{
				throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
			}
			if(!cellRange.Any(c =>
			{
				HexDirection direction = ParentCharacter.ParentCell.GetDirection(c);
				HexCell moveCell = c.GetCell(direction, 2);
				return moveCell != null && moveCell.Type != HexTileType.Wall && moveCell.CharacterOnCell == null;
			}))
			{
				throw new Exception("Nie ma gdzie się ruszyć!");
			}
		}
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			List<HexCell> validatedCellRange = new List<HexCell>();
				cellRange.ForEach(c =>
				{
					HexDirection direction = ParentCharacter.ParentCell.GetDirection(c);
					HexCell moveCell = c.GetCell(direction, 2);
					if (moveCell == null || moveCell.Type == HexTileType.Wall || moveCell.CharacterOnCell != null)
					{
//						throw new Exception("Nie ma gdzie się ruszyć!");
//						cellRange.Remove(c);
						return;
					}
					validatedCellRange.Add(c);

				});
			var canUseAbility = Active.Prepare(Action.UseAbility, validatedCellRange);
			try
			{
				if (!canUseAbility)
				{
					throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
				}
				
				validatedCellRange.ForEach(c =>
				{
					c.AddHighlight(Highlights.RedTransparent);
					HexDirection direction = ParentCharacter.ParentCell.GetDirection(c);
					HexCell moveCell = c.GetCell(direction, 2);
					moveCell.AddHighlight(Highlights.BlueTransparent);
				});
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
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(targetCharacter.ParentCell);
			HexCell moveCell = targetCharacter.ParentCell.GetCell(direction, 2);
			ParentCharacter.MoveTo(moveCell);
			var damage = new Damage(ParentCharacter.AttackPoints.Value, DamageType.Physical);
			ParentCharacter.Attack(targetCharacter, damage);
			Active.PlayAudio("giri");
			OnUseFinish();
		}
	}
}
