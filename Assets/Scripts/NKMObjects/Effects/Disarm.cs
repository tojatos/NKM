using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class Disarm : Effect
	{
		public Disarm(int cooldown, Character parentCharacter, string name=null) : base(cooldown, parentCharacter, name)
		{
			Name = name??"Disarm";
			Type = EffectType.Negative;
		}
		public override string GetDescription()
		{
			return "Ta postać nie może atakować.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}

		public override bool IsCC => true;
	}
}
