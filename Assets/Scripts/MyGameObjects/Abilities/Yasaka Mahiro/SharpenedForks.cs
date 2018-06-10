using System;
using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Yasaka_Mahiro
{
	public class SharpenedForks : Ability
	{
		private const int AbilityDamage = 5;
		private const float AbilityMissingHealthPercentDamage = 20;
		private const int AbilityRange = 7;
		private int _numberOfUses;
		public SharpenedForks()
		{
			Name = "Sharpened Forks";
			Cooldown = 3;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
			_numberOfUses = 0;
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
@"{0} rzuca 3 naostrzone widelce, zadając {1} + {2}% brakującego zdrowia przewiwnika obrażeń fizycznych przy każdym trafieniu.
Każdy widelec może zostać wymierzony w innego wroga.
Zasięg: {3}	Czas odnowienia: {4}",
						 ParentCharacter.Name, AbilityDamage, AbilityMissingHealthPercentDamage, AbilityRange, Cooldown);
		}
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			if (_numberOfUses != 0)
			{
				OnUseFinish();
			}
			else
			{
				MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
				OnFailedUseFinish();
			}
		}
		public override void Use(Character targetCharacter)
		{
			var damageValue = (int) (AbilityDamage + AbilityMissingHealthPercentDamage / 100 *
			                 (targetCharacter.HealthPoints.BaseValue - targetCharacter.HealthPoints.Value));
//			AnimationPlayer.Instance.StartCoroutine(
//				AnimationPlayer.Instance.SharpenedForkEnumerator(ParentCharacter.CharacterObject.transform,
//					targetCharacter.CharacterObject.transform)); TODO: animation
			var damage = new Damage(damageValue, DamageType.Physical);
			ParentCharacter.Attack(targetCharacter, damage);
			_numberOfUses++;
			if (_numberOfUses < 3)
			{
				Use();
				return;
			}

			OnUseFinish();
		}
		public override void OnUseFinish()
		{
			base.OnUseFinish();
			_numberOfUses = 0;
		}
		public override void Cancel()
		{
			if (_numberOfUses == 0)
				OnFailedUseFinish();
			else
				OnUseFinish();
		}
	}
}
