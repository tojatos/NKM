using System.Linq;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Gilgamesh
{
	public class TheFistHero : Ability
	{
		private const int DamageReductionPercent = 10;
		private const int AdditionalDamagePercent = 10;
		public TheFistHero() : base(AbilityType.Passive, "The Fist Hero")
		{
//			Name = "The Fist Hero";
//			Type = AbilityType.Passive;
		}
		public override string GetDescription() => $@"Dzięki nieznającemu kresu skarbcowi, {ParentCharacter.Name} jest w stanie znaleźć odpowiedź na każdego wroga.
W walce otrzymuje on {DamageReductionPercent}% mniej obrażeń, a jego ataki i umiejętności zadają dodatkowe {AdditionalDamagePercent}% obrażeń.";

		public override void Awake()
		{
			ParentCharacter.BeforeAttack += (character, damage) =>
			{
				var modifier = ParentCharacter.Effects.OfType<PassiveBuff>().SingleOrDefault() == null ? 1 : 2;
				damage.Value += modifier * damage.Value * AdditionalDamagePercent / 100;
			};
			ParentCharacter.BeforeBeingDamaged += damage =>
			{
				var modifier = ParentCharacter.Effects.OfType<PassiveBuff>().SingleOrDefault() == null ? 1 : 2;
				damage.Value -= modifier * damage.Value * DamageReductionPercent / 100;
			};
		}

	}
}
