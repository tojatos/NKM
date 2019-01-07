using Extensions;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Gilgamesh
{
	public class TheFistHero : Ability
	{
		private const int DamageReductionPercent = 10;
		private const int AdditionalDamagePercent = 10;
		public TheFistHero(Game game) : base(game, AbilityType.Passive, "The Fist Hero")
		{
			OnAwake += () =>
			{
				ParentCharacter.BeforeAttack += (character, damage) =>
				{
					int modifier = ParentCharacter.Effects.ContainsType(typeof(PassiveBuff)) ? 2 : 1;
					damage.Value += modifier * damage.Value * AdditionalDamagePercent / 100;
				};
				ParentCharacter.BeforeBeingDamaged += damage =>
				{
					int modifier = ParentCharacter.Effects.ContainsType(typeof(PassiveBuff)) ? 2 : 1;
					damage.Value -= modifier * damage.Value * DamageReductionPercent / 100;
				};
			};
		}
		public override string GetDescription() => 
$@"Dzięki nieznającemu kresu skarbcowi, {ParentCharacter.Name} jest w stanie znaleźć odpowiedź na każdego wroga.
W walce otrzymuje on {DamageReductionPercent}% mniej obrażeń, a jego ataki i umiejętności zadają dodatkowe {AdditionalDamagePercent}% obrażeń.";
	}
}
