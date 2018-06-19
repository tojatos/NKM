using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class MovementDisability : Effect
	{
		public MovementDisability(int cooldown, Character parentCharacter, string name=null) : base(cooldown, parentCharacter, name)
		{
			Name = name?? "Movement Disability";
			//CurrentCooldown = cooldown;
			Type = EffectType.Negative;
		}
		public override string GetDescription()
		{
			return "Ta postać nie może się ruszać.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
	}
}
