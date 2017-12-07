using System.Linq;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Hanekawa_Tsubasa
{
	public class NineLives : Ability
	{
		private const int HealthRegainedPercent = 25;
		public NineLives()
		{
			Name = "Nine Lives";
			Type = AbilityType.Passive;
		}
		public override string GetDescription() => $@"{ParentCharacter.Name} odzyskuje {HealthRegainedPercent}% wszystkich zadanych obrażeń przez w formie HP.
Jeżeli zaatakowany przeciwnik posiada efekt Blood Kiss, ta premia jest przyznawana podwójnie.";

		public override void OnDamage(Character targetCharacter, int damageDealt)
		{
			var modifier = 1;
			if (targetCharacter.Effects.OfType<Effects.DamageOverTime>().Any(e=>e.Name=="Blood Kiss"))
			{
				modifier = 2;
			}
			var amountToHeal = damageDealt * HealthRegainedPercent / 100 * modifier;
			ParentCharacter.Heal(ParentCharacter, amountToHeal);
		}
	}
}
