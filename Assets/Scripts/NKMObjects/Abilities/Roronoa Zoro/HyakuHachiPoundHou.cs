using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Roronoa_Zoro
{
	public class HyakuHachiPoundHou : Ability
	{
		private const int AbilityDamage = 18;
		private const int AbilityRange = 6;

		public HyakuHachiPoundHou()
		{
			Name = "Hyaku Hachi Pound Hou";
			Cooldown = 6;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} wysyła 3 fale uderzeniowe w wybranego wroga, z czego każda zadaje {1} obrażeń fizycznych.
Zasięg: {2}	Czas odnowienia: {3}",
				ParentCharacter.Name, AbilityDamage, AbilityRange, Cooldown);
		}
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
		public override void Use(Character targetCharacter)
		{
			Active.PlayAudio(Name);
			HexCell targetCell = targetCharacter.ParentCell;
			SendShockwave(targetCell);
			SendShockwave(targetCell);
			SendShockwave(targetCell);
			OnUseFinish();
		}

		private void SendShockwave(HexCell targetCell)
		{
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(targetCell);
			List<HexCell> shockwaveCells = ParentCharacter.ParentCell.GetLine(direction, AbilityRange).ToList();
			foreach (HexCell c in shockwaveCells)
			{
				if (c.CharacterOnCell == null || c.CharacterOnCell.Owner == ParentCharacter.Owner) continue;

				var damage = new Damage(AbilityDamage, DamageType.Physical);
				ParentCharacter.Attack(c.CharacterOnCell, damage);
				break;
			}
		}
	}
}
