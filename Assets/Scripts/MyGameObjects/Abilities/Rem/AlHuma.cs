using System;
using System.Collections.Generic;
using Hex;
using MyGameObjects.Effects;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Rem
{
	public class AlHuma : Ability
	{
		private const int AbilityDamage = 10;
		private const int AbilityRange = 7;
		public AlHuma()
		{
			Name = "Al Huma";
			Cooldown = 4;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
		}
		protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			var cellRange = GetRangeCells();
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
@"{0} zamraża jednego wroga w zasięgu {1} do końca tej fazy,
zadając {2} obrażeń magicznych.
Czas odnowiania: {3}",
				ParentCharacter.Name, AbilityRange, AbilityDamage, Cooldown);
		}
		protected override void Use()
		{
			var cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;
			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
			OnFailedUseFinish();
		}
		public override void Use(Character targetCharacter)
		{
			ParentCharacter.Attack(targetCharacter, AttackType.Magical, AbilityDamage);
			targetCharacter.Effects.Add(new MovementDisability(1, targetCharacter, "Al Huma's Freeze"));
			OnUseFinish();
		}
	}
}
