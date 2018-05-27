using System.Collections.Generic;
using System.Linq;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Itsuka_Kotori
{
	public class CamaelMegiddo : Ability
	{
		private const int Damage = 35;
		private const int Width = 1;

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
Szerokość: {2}	Czas odnowienia: {3}",
				ParentCharacter.Name, Damage, Width, Cooldown);
		}
		public override List<HexCell> GetRangeCells()
		{
			return (List<HexCell>) Enumerable.Empty<HexCell>(); //TODO
		}
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
			OnFailedUseFinish();
		}
//		public override void Use(Character targetCharacter)
//		{
//			Active.PlayAudio(Name);
//			HexCell targetCell = targetCharacter.ParentCell;
//			SendShockwave(targetCell);
//			SendShockwave(targetCell);
//			SendShockwave(targetCell);
//			OnUseFinish();
//		}

//		private void SendShockwave(HexCell targetCell)
//		{
//			HexDirection direction = ParentCharacter.ParentCell.GetDirection(targetCell);
//			List<HexCell> shockwaveCells = ParentCharacter.ParentCell.GetLine(direction, AbilityRange).ToList();
//			foreach (HexCell c in shockwaveCells)
//			{
//				if (c.CharacterOnCell == null || c.CharacterOnCell.Owner == ParentCharacter.Owner) continue;
//
//				ParentCharacter.Attack(c.CharacterOnCell, AttackType.Physical, AbilityDamage);
//				break;
//			}
//		}
	}
}
