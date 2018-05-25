using System;
using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.Effects;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Hanekawa_Tsubasa
{
	public class CurseOfTheBlackCat : Ability
	{
		private const int AbilityRange = 5;
		private const int DoTDamage = 6;
		private const int DoTTime = 5;
		private const int AdditionalDamagePercent = 25;
		public CurseOfTheBlackCat()
		{
			Name = "Curse of The Black Cat";
			Cooldown = 7;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} rzuca klątwę na wroga wysysając z niego {1} HP co fazę przez {2} fazy (zadaje obrażenia nieuchronne).
Podczas trwania efektu, {0} zadaje celowi klątwy dodatkowe {3}% obrażeń.
Zasięg: {4} Czas odnowienia: {5}",
				ParentCharacter.Name, DoTDamage, DoTTime, AdditionalDamagePercent, AbilityRange, Cooldown);
		}
		public override List<HexCell> GetRangeCells()
		{
			return ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
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
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			Active.Prepare(this, cellRange);
		}
		public override void Use(Character targetCharacter)
		{
			try
			{
				targetCharacter.Effects.Add(new HPDrain(ParentCharacter, DoTDamage, AttackType.True, DoTTime, targetCharacter, "Curse of The Black Cat"));
				OnUseFinish();
			}
			catch (Exception e)
			{
				MessageLogger.DebugLog(e.Message);
				OnFailedUseFinish();
			}
		}
	}
}
