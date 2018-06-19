using System.Linq;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Dekomori_Sanae
{
	public class WickedEyesServant : EnableableAbility
	{
		private int _additionalDamage;
		private bool _isBeingUsed;
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
		public override void Awake()
		{
//			ParentCharacter.JustBeforeAttack += (character, damage) => damage.Value += IsEnabled ? _additionalDamage : 0;
			ParentCharacter.BeforeAttack += (character, d) =>
			{
				if (!IsEnabled || _isBeingUsed) return;
				_isBeingUsed = true; //prevent infinite loop
				var damage = new Damage(_additionalDamage, DamageType.True);
				ParentCharacter.Attack(character, damage);
				_isBeingUsed = false;

			};
			ParentCharacter.OnEnemyKill += () => _additionalDamage++;
		}
	}
}
