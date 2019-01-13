﻿using System.Linq;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Rem
{
	public class DemonicForm : Ability, IEnableable
	{
		private const int HpPercent = 40;
		private const int AdditionalAttack = 9;
		private const int AdditionalSpeed = 4;

		public DemonicForm(Game game) : base(game, AbilityType.Passive, "Demonic Form")
		{
			OnAwake += () => ParentCharacter.HealthPoints.StatChanged += TryToActivateDemonicForm;			
		}

		public override string GetDescription() => 
$@"Gdy życie tej postaci spadnie poniżej <color=red>{HpPercent}</color>%,
zyskuje ona {AdditionalAttack} ataku i {AdditionalSpeed} szybkości.";

		private bool _isEnabled;
		public bool IsEnabled
		{
			get
			{
				if (_isEnabled) return true;

				_isEnabled = (float)ParentCharacter.HealthPoints.Value / ParentCharacter.HealthPoints.BaseValue < (float)HpPercent / 100;
				return _isEnabled;
			}
		}
		private void TryToActivateDemonicForm()
		{
			if (!IsEnabled) return;

			//Active.Character.Deselect();
			ParentCharacter.Name = "Demonic Rem";
			if (ParentCharacter.Effects.All(e => e.Name != "Demonic Form Speed Boost"))
			{
				var speedBoost = new StatModifier(Game, -1, AdditionalSpeed, ParentCharacter, StatType.Speed, "Demonic Form Speed Boost");
				speedBoost.OnRemove += TryToActivateDemonicForm;
				ParentCharacter.Effects.Add(speedBoost);

			}
			if (ParentCharacter.Effects.All(e => e.Name != "Demonic Form Attack Boost"))
			{
				var attackBoost = new StatModifier(Game, -1, AdditionalAttack, ParentCharacter, StatType.AttackPoints, "Demonic Form Attack Boost");
				attackBoost.OnRemove += TryToActivateDemonicForm;
				ParentCharacter.Effects.Add(attackBoost);
			}
//			ParentCharacter.Select();
			Active.Select(ParentCharacter);
		}
	}
}
