using System.Linq;
using MyGameObjects.Effects;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Gilgamesh
{
	public class TheFistHero : Ability
	{
		private const int DamageReductionPercent = 10;
		private const int AdditionalDamagePercent = 10;
		public TheFistHero()
		{
			Name = "The Fist Hero";
			Type = AbilityType.Passive;
		}
		public override string GetDescription() => $@"Dzięki nieznającemu kresu skarbcowi, {ParentCharacter.Name} jest w stanie znaleźć odpowiedź na każdego wroga.
W walce otrzymuje on {DamageReductionPercent}% mniej obrażeń, a jego ataki i umiejętności zadają dodatkowe {AdditionalDamagePercent}% obrażeń.";

		public override void DamageModifier(Character targetCharacter, ref int damage)
		{
			var modifier = ParentCharacter.Effects.OfType<PassiveBuff>().SingleOrDefault() == null ? 1 : 2;
			damage += modifier * damage * AdditionalDamagePercent / 100;
		}

		public override void BeforeParentDamage(ref int damage)
		{
			var modifier = ParentCharacter.Effects.OfType<PassiveBuff>().SingleOrDefault() == null ? 1 : 2;
			damage -= modifier * damage * DamageReductionPercent / 100;
		}
	}
}
