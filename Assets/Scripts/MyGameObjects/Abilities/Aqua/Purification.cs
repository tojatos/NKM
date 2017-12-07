using System;
using System.Collections.Generic;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Aqua
{
	public class Purification : Ability
	{
		private const int AbilityRange = 8;

		public Purification()
		{
			Name = "Purification";
			Cooldown = 4;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
		}
		public override string GetDescription() => $@"{ParentCharacter.Name} rzuca oczyszczający czar na sojusznika, zdejmując z niego wszelkie negatywne efekty.
Zasięg: {AbilityRange} Czas odnowienia: {Cooldown}";

		protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			var cellRange = GetRangeCells();
			cellRange.RemoveNonFriends();
			if (cellRange.Count == 0)
			{
				throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
			}
		}

		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);

		protected override void Use()
		{
			var cellRange = GetRangeCells();
			cellRange.RemoveNonFriends();
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;
			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
			OnFailedUseFinish();
		}
		public override void Use(Character character)
		{
			character.Effects.RemoveAll(e => e.Type == EffectType.Negative);
			OnUseFinish();
		}
	}
}
