using System;
using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Sinon
{
	public class PreciseShot : Ability, IClickable
	{
		private const int AbilityDamage = 40;
		private const int AbilityRange = 11;

		public PreciseShot() : base(AbilityType.Ultimatum, "Precise Shot", 6)
		{
//			Name = "Precise Shot";
//			Cooldown = 6;
//			CurrentCooldown = 0;
//			Type = AbilityType.Ultimatum;
		}
		public override string GetDescription() => $@"{ParentCharacter.Name} strzela w wybranego wroga, zadając {AbilityDamage} obrażeń fizycznych.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);

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

		public void ImageClick()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
			OnFailedUseFinish();
		}
		public override void Use(Character character)
		{
			var damage = new Damage(AbilityDamage, DamageType.Physical);
			ParentCharacter.Attack(character, damage);
			OnUseFinish();
		}
	}
}
