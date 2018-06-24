using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Yasaka_Mahiro
{
	public class SharpenedForks : Ability, IClickable
	{
		private const int AbilityDamage = 5;
		private const float AbilityMissingHealthPercentDamage = 20;
		private const int AbilityRange = 7;
		private int _numberOfUses;

		public SharpenedForks() : base(AbilityType.Normal, "Sharpened Forks", 3)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);
		
		public override string GetDescription() =>
$@"{ParentCharacter.Name} rzuca 3 naostrzone widelce,
zadając {AbilityDamage} + {AbilityMissingHealthPercentDamage}% brakującego zdrowia przewiwnika obrażeń fizycznych przy każdym trafieniu.
Każdy widelec może zostać wymierzony w innego wroga.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public void Click() => PrepareFork();

		private void PrepareFork()
		{
			if (!CanBeUsed)
			{
				Cancel();
				return;
			}
			Active.Prepare(this, GetTargetsInRange());
		}
		public override void Use(Character targetCharacter)
		{
			var damageValue = (int) (AbilityDamage + AbilityMissingHealthPercentDamage / 100 *
			                 (targetCharacter.HealthPoints.BaseValue - targetCharacter.HealthPoints.Value));
//			AnimationPlayer.Instance.StartCoroutine(
//				AnimationPlayer.Instance.SharpenedForkEnumerator(ParentCharacter.CharacterObject.transform,
//					targetCharacter.CharacterObject.transform)); TODO: animation
			var damage = new Damage(damageValue, DamageType.Physical);
			ParentCharacter.Attack(this, targetCharacter, damage);
			_numberOfUses++;
			if (_numberOfUses < 3)
			{
				PrepareFork();
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
			if (_numberOfUses == 0) OnFailedUseFinish();
			else OnUseFinish();
		}
	}
}
