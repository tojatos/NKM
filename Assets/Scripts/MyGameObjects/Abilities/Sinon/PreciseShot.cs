using System;
using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Sinon
{
	public class PreciseShot : Ability
	{
		private const int AbilityDamage = 40;
		private const int AbilityRange = 12;

		public PreciseShot()
		{
			Name = "Precise Shot";
			Cooldown = 6;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
		}
		public override string GetDescription() => $@"{ParentCharacter.Name} strzela w wybranego wroga, zadając {AbilityDamage} obrażeń fizycznych.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
			cellRange.RemoveNonEnemies();
			if (cellRange.Count == 0)
			{
				throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
			}
		}
		protected override void Use()
		{
			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
			cellRange.RemoveNonEnemies();
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
			OnFailedUseFinish();
		}
		public override void Use(Character character)
		{
			ParentCharacter.Attack(character, AttackType.Physical, AbilityDamage);
			OnUseFinish();
		}
	}
}
