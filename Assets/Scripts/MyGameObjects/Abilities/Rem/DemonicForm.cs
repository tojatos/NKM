using System.Linq;
using MyGameObjects.Effects;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Rem
{
	public class DemonicForm : EnableableAbility
	{
		private const int HpPercent = 30;
		private const int AdditionalAttack = 9;
		private const int AdditionalSpeed = 4;

		public DemonicForm()
		{
			Name = "Demonic Form";
			Type = AbilityType.Passive;
		}

		public override void Awake()
		{
			ParentCharacter.HealthPoints.StatChanged += TryToActivateDemonicForm;
		}

		public override string GetDescription()
		{
			return string.Format(
@"Gdy życie tej postaci spadnie poniżej <color=red>{0}</color>%,
zyskuje ona {1} ataku i {2} szybkości.",
				HpPercent, AdditionalAttack, AdditionalSpeed);
		}

		private bool _isEnabled;
		public override bool IsEnabled
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

			Active.CharacterOnMap.Deselect();
			if (ParentCharacter.Name != "Demonic Rem") ParentCharacter.Name = "Demonic Rem";
			if (ParentCharacter.Effects.All(e => e.Name != "Demonic Form Speed Boost"))
			{
				var speedBoost = new StatModifier(-1, AdditionalSpeed, ParentCharacter, StatType.Speed, "Demonic Form Speed Boost");
				speedBoost.OnRemove += TryToActivateDemonicForm;
				ParentCharacter.Effects.Add(speedBoost);

			}
			if (ParentCharacter.Effects.All(e => e.Name != "Demonic Form Attack Boost"))
			{
				var attackBoost = new StatModifier(-1, AdditionalAttack, ParentCharacter, StatType.AttackPoints, "Demonic Form Attack Boost");
				attackBoost.OnRemove += TryToActivateDemonicForm;
				ParentCharacter.Effects.Add(attackBoost);
			}
			ParentCharacter.Select();
		}
	}
}
