using System.Linq;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Dekomori_Sanae
{
	public class WickedEyesServant : Ability, IEnableable
	{
		private int _additionalDamage = 3;
		private bool _isBeingUsed;
		public WickedEyesServant(Game game) : base(game, AbilityType.Passive, "Wicked Eye's Servant")
		{
			OnAwake += () =>
			{
				ParentCharacter.BeforeAttack += (character, d) =>
				{
					if (!IsEnabled || _isBeingUsed) return;
					_isBeingUsed = true; //prevent infinite loop
					var damage = new Damage(_additionalDamage, DamageType.True);
					ParentCharacter.Attack(this, character, damage);
					_isBeingUsed = false;
				};
				ParentCharacter.OnKill += () => _additionalDamage++;
			};
		}
		public override string GetDescription() => string.Format(
@"{0} zyskuje <color=blue>{1}</color> obrażeń nieuchronnych na każdym ataku i umiejętności,
jeżeli na polu gry znajduje się chociaż jedna postać z atakiem większym od {0} lub Rikka Takanashi.
Zabicie wroga dodaje dodatkowy punkt obrażeń nieuchronnych tej umiejętności na stałe."
			,ParentCharacter.Name, _additionalDamage);
		
		public bool IsEnabled => Game.Players.Any(p => p.Characters.Any(c => c.IsOnMap && (c.AttackPoints.Value > ParentCharacter.AttackPoints.Value || c.Name == "Takanashi Rikka")));
	}
}
