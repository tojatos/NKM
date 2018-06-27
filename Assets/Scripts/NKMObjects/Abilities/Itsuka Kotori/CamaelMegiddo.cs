using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Itsuka_Kotori
{
	public class CamaelMegiddo : Ability, IClickable, IUseable
	{
		private const int Damage = 35;

		public CamaelMegiddo() : base(AbilityType.Ultimatum, "Camael - Megiddo", 6){}
		public override string GetDescription() =>
$@"{ParentCharacter.Name} wystrzeliwuje falę płomieni w wybranym kierunku zadając {Damage} obrażeń wszystkim trafionym wrogom.
Jeżeli ta umiejętność uderzy w obszar Conflagration, zada ona obrażenia na całym tym obszarze, ale nie poleci dalej.
Czas odnowienia: {Cooldown}";

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
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

		public void Click()
		{
			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(1);
			Active.Prepare(this, cellRange); //TODO: Air selection magic? Or maybe new mechanism?
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
				ParentCharacter.Attack(this, c.CharacterOnCell, damage);
			}
		}

		public void Use(List<HexCell> cells)
		{
			SendFlamewave(cells[0]);
			Finish();
		}
	}
}
