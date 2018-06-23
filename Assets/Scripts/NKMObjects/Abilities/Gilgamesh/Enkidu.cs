using System;
using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Gilgamesh
{
	public class Enkidu : Ability, IClickable
	{
		private const int AbilityRange = 8;
		private const int SnarDuration = 2;
		public Enkidu() : base(AbilityType.Normal, "Enkidu", 4)
		{
//			Name = "Enkidu";
//			Cooldown = 4;
//			CurrentCooldown = 0;
//			Type = AbilityType.Normal;
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

		public override string GetDescription() =>
$@"{ParentCharacter.Name} wypuszcza niebiańskie łańcuchy, unieruchamiając przeciwnika na {SnarDuration} fazy
i podwajając bonusy zdolności biernej na ten okres.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public void ImageClick()
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
				ParentCharacter.Effects.Add(new PassiveBuff(SnarDuration, ParentCharacter, Name));
				targetCharacter.Effects.Add(new MovementDisability(SnarDuration, targetCharacter, Name));
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
