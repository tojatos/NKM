using System;
using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.Effects;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Hanekawa_Tsubasa
{
	public class BloodKiss : Ability
	{
		private const int AbilityRange = 3;
		private const int DoTDamage = 8;
		private const int DoTTime = 4;

		public BloodKiss()
		{
			Name = "Blood Kiss";
			Cooldown = 4;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} liże wroga, wywołując silne krwawienie, które zadaje {1} nieuchronnych obrażeń przez {2} fazy.
Zasięg: {3} Czas odnowienia: {4}",
ParentCharacter.Name, DoTDamage, DoTTime, AbilityRange, Cooldown);
		}
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
		public override List<HexCell> GetRangeCells()
		{
			return ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		}
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			Active.Prepare(this, cellRange);
		}
		public override void Use(Character targetCharacter)
		{
			var damage = new Damage(DoTDamage, DamageType.True);
			targetCharacter.Effects.Add(new DamageOverTime(ParentCharacter, damage, DoTTime, targetCharacter, "Blood Kiss"));
			OnUseFinish();
		}
	}
}
