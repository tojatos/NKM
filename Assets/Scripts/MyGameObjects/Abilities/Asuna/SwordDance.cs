using System;
using Helpers;
using MyGameObjects.MyGameObject_templates;
using UIManagers;

namespace MyGameObjects.Abilities.Asuna
{
	public class SwordDance : EnableableAbility
	{
		private const int AbilityMaxDuration = 3;
		private const int AbilityMaxHits = 3;
		private const int AbilityBonusAttackGain = 2;

		private int _attacksToBlock;
		private int _phasesRemain;
		private int _currentBonusAttack;

		public SwordDance()
		{
			Name = "Sword Dance";
			Cooldown = 4;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
			_attacksToBlock = 3;
			_phasesRemain = 2;
			_currentBonusAttack = 0;
		}
		private bool _isEnabled;
		public override bool IsEnabled => _isEnabled;

		public override string GetDescription()
		{
			var description = string.Format(
@"{0} na {1} fazy lub {2} ataki blokuje wszystkie nadchodzące ataki podstawowe.
Dodatkowo, za każdy sparowany atak zadaje {3} dodatkowych obrażeń na atakach i umiejętnościach.
Aktualny bonus: {4} 

Czas odnowienia: {5} (po zakończeniu efektu)",
				ParentCharacter.Name, AbilityMaxDuration, AbilityMaxHits, AbilityBonusAttackGain, _currentBonusAttack, Cooldown);
			if (IsEnabled)
			{
				description += $@"

Pozostały czas działania: {_phasesRemain}
Pozostałe ataki do zablokowania: {_attacksToBlock}";
			}
			return description;
		}


		protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			if(IsEnabled) throw new Exception("Umiejętność nie może zostać użyta jeżeli jest już aktywna!");
		}

		protected override void Use()
		{
			_isEnabled = true;
			OnUseFinish(0);
			CharacterAbilities.Instance.UpdateButtonData();
		}

		public override bool BeforeParentBasicAttacked(Character attackingCharacter)
		{
			if (!IsEnabled) return true;

			MessageLogger.Log(string.Format("{0} blokuje atak {1}!", ParentCharacter.FormattedFirstName(), attackingCharacter.FormattedFirstName()));
			_attacksToBlock--;
			_currentBonusAttack += AbilityBonusAttackGain;
			if(_attacksToBlock == 0) Disable();
			return false;
		}

		public override void OnPhaseFinish()
		{
			base.OnPhaseFinish();
			if (!IsEnabled) return;

			_phasesRemain--;
			if (_phasesRemain == 0) Disable();
		}
		private void Disable()
		{
			_isEnabled = false;
			_attacksToBlock = 3;
			_phasesRemain = 2;
			CurrentCooldown = Cooldown;
		}
		public override void DamageModifier(Character targetCharacter, ref int damage)
		{
			damage += _currentBonusAttack;
		}
	}
}
