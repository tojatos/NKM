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
		private int _phasesRemain = 3;
		private int _currentBonusAttack;

		public SwordDance() : base(AbilityType.Ultimatum, "Sword Dance", 4)
		{
			OnAwake += () =>
			{
				Validator.ToCheck.Add(() => !IsEnabled);
				ParentCharacter.BeforeBeingBasicAttacked += (attackingCharacter, damage) =>
				{
					if (!IsEnabled) return;

					Console.Log(
						$"{ParentCharacter.FormattedFirstName()} blokuje atak {attackingCharacter.FormattedFirstName()}!");
					_attacksToBlock--;
					damage.Value = 0;
					_currentBonusAttack += AbilityBonusAttackGain;
					if (_attacksToBlock == 0) Disable();
				};
				ParentCharacter.BeforeAttack += (character, damage) => damage.Value += _currentBonusAttack;
				Active.Phase.PhaseFinished += () =>
				{
					if (!IsEnabled) return;
					_phasesRemain--;
					if (_phasesRemain == 0) Disable();
				};
			};
		}

		public bool IsEnabled { get; private set; }

		public override string GetDescription()
		{
			string description = $@"{ParentCharacter.Name} na {AbilityMaxDuration} fazy lub {AbilityMaxHits} ataki blokuje wszystkie nadchodzące ataki podstawowe.
Dodatkowo, za każdy sparowany atak zadaje {AbilityBonusAttackGain} dodatkowych obrażeń na atakach i umiejętnościach.
Aktualny bonus: {_currentBonusAttack} 

Czas odnowienia: {Cooldown} (po zakończeniu efektu)";
			if (IsEnabled)
			{
				description += $@"

Pozostały czas działania: {_phasesRemain}
Pozostałe ataki do zablokowania: {_attacksToBlock}";
			}
			return description;
		}
		public void Click()
		{
			Active.MakeAction();
			IsEnabled = true;
			Finish(0);
			UI.CharacterUI.Abilities.Instance.UpdateButtonData();
		}
		private void Disable()
		{
			IsEnabled = false;
			_attacksToBlock = 3;
			_phasesRemain = 3;
			CurrentCooldown = Cooldown;
		}
	}
}
