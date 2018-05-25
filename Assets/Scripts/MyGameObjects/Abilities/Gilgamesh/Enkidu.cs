using System;
using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.Effects;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Gilgamesh
{
	public class Enkidu : Ability
	{
		private const int AbilityRange = 8;
		private const int StunDuration = 2;
		public Enkidu()
		{
			Name = "Enkidu";
			Cooldown = 3;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
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

		public override string GetDescription()
		{
			return string.Format(
@"{0} wypuszcza niebiańskie łańcuchy, unieruchamiając przeciwnika na {1} fazy
i podwajając bonusy zdolności biernej na ten okres.
Zasięg: {2}	Czas odnowienia: {3}",
				ParentCharacter.Name, StunDuration, AbilityRange, Cooldown);
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
			try
			{
				ParentCharacter.Effects.Add(new PassiveBuff(StunDuration, ParentCharacter, Name));
				targetCharacter.Effects.Add(new MovementDisability(StunDuration, targetCharacter, Name));
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
