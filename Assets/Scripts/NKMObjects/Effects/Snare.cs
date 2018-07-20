using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class Snare : Effect
	{
		public Snare(int cooldown, Character parentCharacter, string name=null) : base(cooldown, parentCharacter, name)
		{
			Name = name?? "Snare";
			Type = EffectType.Negative;
		}
		public override string GetDescription()
		{
			return "Ta postać nie może wykonywać zwykłego ruchu.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
		public override bool IsCC => true;
	}
}
