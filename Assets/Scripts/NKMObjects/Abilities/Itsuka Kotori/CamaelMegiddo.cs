using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Itsuka_Kotori
{
	public class CamaelMegiddo : Ability
	{
		private const int Damage = 35;

		public CamaelMegiddo()
		{
			Name = "Camael - Megiddo";
			Cooldown = 6;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} wystrzeliwuje falę płomieni w wybranym kierunku zadając {1} obrażeń wszystkim trafionym wrogom.
Jeżeli ta umiejętność uderzy w obszar Conflagration, zada ona obrażenia na całym tym obszarze, ale nie poleci dalej.
Czas odnowienia: {2}",
				ParentCharacter.Name, Damage, Cooldown);
		}
		public override List<HexCell> GetRangeCells()
		{
			List<HexCell> cells = new List<HexCell>();
			bool hitConflargation = false;
			foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
			{
				HexCell lastCell = ParentCharacter.ParentCell;
				do
				{
					HexCell neighbor = lastCell.GetCell(direction, 1);
					if(neighbor==null) break;
					if (neighbor.Effects.ContainsType(typeof(HexCellEffects.Conflagration)))
					{
						hitConflargation = true;
						break;
					}
					cells.Add(neighbor);
					lastCell = neighbor;
				} while (true);
			}
				if(hitConflargation) cells.AddRange(HexMapDrawer.Instance.Cells.Where(c => c.Effects.ContainsType(typeof(HexCellEffects.Conflagration))));
				return cells.Distinct().ToList();
		}
		protected override void Use()
		{
			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(1);
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			MessageLogger.DebugLog("Nie ma komórek dookoła postaci!");
			OnFailedUseFinish();
		}

		public override void Use(HexCell cell)
		{
			SendFlamewave(cell);
			OnUseFinish();
		}

		private void SendFlamewave(HexCell targetCell)
		{
			List<HexCell> cells = new List<HexCell>();
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(targetCell);
			bool hitConflargation = false;
			HexCell lastCell = ParentCharacter.ParentCell;
				do
				{
					HexCell neighbor = lastCell.GetCell(direction, 1);
					if(neighbor==null) break;
					if (neighbor.Effects.ContainsType(typeof(HexCellEffects.Conflagration)))
					{
						hitConflargation = true;
						break;
					}
					cells.Add(neighbor);
					lastCell = neighbor;
				} while (true);
			
				if(hitConflargation) cells.AddRange(HexMapDrawer.Instance.Cells.Where(c => c.Effects.ContainsType(typeof(HexCellEffects.Conflagration))));
			foreach (HexCell c in cells)
			{
				if (c.CharacterOnCell == null || c.CharacterOnCell.Owner == ParentCharacter.Owner) continue;

				var damage = new Damage(Damage, DamageType.Magical);
				ParentCharacter.Attack(c.CharacterOnCell, damage);
			}
		}
	}
}
