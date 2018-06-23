using Extensions;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Hanekawa_Tsubasa
{
	public class NineLives : Ability
	{
		private const int HealthRegainedPercent = 25;
		public NineLives() : base(AbilityType.Passive, "Nine Lives")
		{
//			Name = "Nine Lives";
//			Type = AbilityType.Passive;
		}
		public override string GetDescription() => $@"{ParentCharacter.Name} odzyskuje {HealthRegainedPercent}% wszystkich zadanych obrażeń przez w formie HP.
Jeżeli zaatakowany przeciwnik posiada efekt Blood Kiss, ta premia jest przyznawana podwójnie.";

		public override void Awake()
		{
			ParentCharacter.AfterAttack += (character, damage) => 
			{
                var modifier = 1;
                if (character.Effects.ContainsType(typeof(BloodKiss)))//.OfType<Effects.DamageOverTime>().Any(e=>e.Name=="Blood Kiss"))
                {
                    modifier = 2;
                }
                var amountToHeal = damage.Value * HealthRegainedPercent / 100 * modifier;
                ParentCharacter.Heal(ParentCharacter, amountToHeal);
			};
		}
		
	}
}
