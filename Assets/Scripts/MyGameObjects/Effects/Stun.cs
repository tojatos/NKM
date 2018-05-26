using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Effects
{
	public class Stun : Effect
	{
		public Stun(int cooldown, Character parentCharacter, string name=null) : base(cooldown, parentCharacter, name)
		{
			Name = name?? "Stun";
			Type = EffectType.Negative;
		}
		public override string GetDescription()
		{
			return "Ta postać nie może wykonywać akcji.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
	}
}
