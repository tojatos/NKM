using System;
using System.Linq;
using NKMObjects.Abilities.Gilgamesh;
using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class PassiveBuff : Effect
	{
		private readonly Ability _passiveAbility;
		public PassiveBuff(int cooldown, Character parentCharacter, string name = null) : base(cooldown, parentCharacter, name)
		{
			Name = name ?? "Passive Buff";
			Type = EffectType.Positive;
			try
			{
				_passiveAbility = parentCharacter.Abilities.OfType<TheFistHero>().SingleOrDefault();
				if (_passiveAbility == null) throw new Exception("Pasywna umiejętność nie znaleziona!");
			}
			catch (Exception e)
			{
				MessageLogger.DebugLog(e.Message);
			}
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
