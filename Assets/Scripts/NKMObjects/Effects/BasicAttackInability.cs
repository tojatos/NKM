using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class BasicAttackInability : Effect
	{
		public BasicAttackInability(int cooldown, Character parentCharacter, string name=null) : base(cooldown, parentCharacter, name)
		{
			Name = name??"Basic Attack Inability";
			Type = EffectType.Negative;
		}
		public override string GetDescription()
		{
			return "Ta postać nie może atakować.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
	}
}
