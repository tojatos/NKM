using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Itsuka_Kotori
{
	public class ElohimGibor : EnableableAbility
	{
		private const int Percent = 50;
		private int _turnsWithoutBeingHurt;
		private int _amountToHeal;
		
		public ElohimGibor()
		{
			Name = "Elohim Gibor";
			Type = AbilityType.Passive;
		}

		public override void Awake()
		{
			ParentCharacter.JustBeforeFirstAction += () =>
			{
				++_turnsWithoutBeingHurt;
				if (_turnsWithoutBeingHurt < 2) return;
				ParentCharacter.Heal(ParentCharacter, _amountToHeal);
				_turnsWithoutBeingHurt = 0;
				_amountToHeal = 0;
			};
			ParentCharacter.AfterBeingDamaged += damage =>
			{
				_amountToHeal += (int) (damage.Value * Percent / 100f);
				_turnsWithoutBeingHurt = 0;
			};
		}

		public override string GetDescription() => $@"Jeżeli {ParentCharacter.Name} nie otrzyma obrażeń przez 2 tury z rzędu, regeneruje ona ilość HP równą 75% obrażeń otrzymywanych przez nią od ostatniego uaktywnienia tej umiejętności";

		public override bool IsEnabled => _turnsWithoutBeingHurt >= 2;
	}
}
