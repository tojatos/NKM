using System.Linq;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Dekomori_Sanae
{
	public class WickedEyesServant : EnableableAbility
	{
		private int _additionalDamage;
		public WickedEyesServant()
		{
			Name = "Wicked Eye's Servant";
			Type = AbilityType.Passive;
			_additionalDamage = 3;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} zyskuje <color=blue>{1}</color> obrażeń nieuchronnych na każdym ataku i umiejętności,
jeżeli na polu gry znajduje się chociaż jedna postać z atakiem większym od {0} lub Rikka Takanashi.
Zabicie wroga dodaje dodatkowy punkt obrażeń nieuchronnych tej umiejętności na stałe."
						 ,ParentCharacter.Name, _additionalDamage);
		}
		public override bool IsEnabled
		{
			get
			{
				return Game.Players.Any(p => p.Characters.Any(c => c.IsOnMap && (c.AttackPoints.Value > ParentCharacter.AttackPoints.Value || c.Name == "Takanashi Rikka")));
			}
		}
		public override void TrueDamageModifier(Character targetCharacter, ref int damage)
		{
			damage += IsEnabled ? _additionalDamage : 0;
		}
		public override void OnEnemyKill()
		{
			_additionalDamage++;
		}
	}
}
