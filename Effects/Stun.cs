using NKMCore.Templates;

namespace NKMCore.Effects
{
	public class Stun : Effect
	{
		public Stun(Game game, int cooldown, Character parentCharacter, string name=null) : base(game, cooldown, parentCharacter, name)
		{
			Name = name?? "Stun";
			Type = EffectType.Negative;
		}
		public override string GetDescription()
		{
			return "Ta postać nie może wykonywać akcji.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
		public override bool IsCC => true;
	}
}
