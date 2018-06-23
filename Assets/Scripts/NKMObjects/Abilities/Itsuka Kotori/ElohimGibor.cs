using NKMObjects.Templates;

namespace NKMObjects.Abilities.Itsuka_Kotori
{
	public class ElohimGibor : Ability, IEnableable
	{
		private const int Percent = 50;
		private int _turnsWithoutBeingHurt;
		private int _amountToHeal;
		
		public ElohimGibor() : base(AbilityType.Passive, "Elohim Gibor")
		{
			OnAwake += () =>
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
			};
		}
		public bool IsEnabled => _turnsWithoutBeingHurt >= 2;
		public override string GetDescription() => 
$@"Jeżeli {ParentCharacter.Name} nie otrzyma obrażeń przez 2 tury z rzędu,
regeneruje ona ilość HP równą 75% obrażeń otrzymywanych przez nią od ostatniego uaktywnienia tej umiejętności";

	}
}
