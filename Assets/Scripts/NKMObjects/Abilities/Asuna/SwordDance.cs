﻿using System;
using Extensions;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Asuna
{
	public class SwordDance : Ability, IClickable, IEnableable
	{
		private const int AbilityMaxDuration = 3;
		private const int AbilityMaxHits = 3;
		private const int AbilityBonusAttackGain = 2;

		private int _attacksToBlock = 3;
		private int _phasesRemain = 2;
		private int _currentBonusAttack;

		public SwordDance() : base(AbilityType.Ultimatum, "Sword Dance", 4)
		{
//			Name = "Sword Dance";
//			Cooldown = 4;
//			CurrentCooldown = 0;
//			Type = AbilityType.Ultimatum;
//			_attacksToBlock = 3;
//			_phasesRemain = 2;
//			_currentBonusAttack = 0;
		}

		public bool IsEnabled { get; private set; }

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

		public void ImageClick()
		{
			Active.MakeAction();
			IsEnabled = true;
			OnUseFinish(0);
			UI.CharacterUI.Abilities.Instance.UpdateButtonData();
		}

		public override void Awake()
		{
			ParentCharacter.BeforeBeingBasicAttacked += (attackingCharacter, damage) =>
			{
				if (!IsEnabled) return;

                MessageLogger.Log(string.Format("{0} blokuje atak {1}!", ParentCharacter.FormattedFirstName(), attackingCharacter.FormattedFirstName()));
                _attacksToBlock--;
                _currentBonusAttack += AbilityBonusAttackGain;
                if(_attacksToBlock == 0) Disable();
			};
			ParentCharacter.BeforeAttack += (character, damage) => damage.Value += _currentBonusAttack;
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
			IsEnabled = false;
			_attacksToBlock = 3;
			_phasesRemain = 2;
			CurrentCooldown = Cooldown;
		}
	}
}
