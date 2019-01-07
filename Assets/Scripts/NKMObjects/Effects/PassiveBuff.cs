using System.Linq;
using NKMObjects.Abilities.Gilgamesh;
using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class PassiveBuff : Effect
	{
		private readonly Ability _passiveAbility;
		public PassiveBuff(Game game, int cooldown, Character parentCharacter, string name = null) : base(game, cooldown, parentCharacter, name)
		{
			Name = name ?? "Passive Buff";
			Type = EffectType.Positive;
            _passiveAbility = parentCharacter.Abilities.OfType<TheFistHero>().SingleOrDefault();
		}

		public override string GetDescription()
		{
			if (_passiveAbility == null)
				return "Nic nie robi - najwidoczniej pasywka twojej postaci nie może zostać zbuffowana :)";
			if (_passiveAbility.GetType() == typeof(TheFistHero)) return "Podwaja efekt zdolności biernej.";

			return "Błąd w pliku PassiveBuff.cs";
		}

	}
}
