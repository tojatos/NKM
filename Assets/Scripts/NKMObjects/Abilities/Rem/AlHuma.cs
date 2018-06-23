using System;
using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Rem
{
	public class AlHuma : Ability, IClickable
	{
		private const int AbilityDamage = 10;
		private const int AbilityRange = 7;
		public AlHuma() : base(AbilityType.Normal, "Al Huma", 4)
		{
//			Name = "Al Huma";
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
$@"{ParentCharacter.Name} zamraża jednego wroga w zasięgu {AbilityRange} na jedną turę,
zadając {AbilityDamage} obrażeń magicznych.
Czas odnowiania: {Cooldown}";

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
			var damage = new Damage(AbilityDamage, DamageType.Magical);
			ParentCharacter.Attack(targetCharacter, damage);
			targetCharacter.Effects.Add(new Stun(1, targetCharacter, Name));
			OnUseFinish();
		}
	}
}
