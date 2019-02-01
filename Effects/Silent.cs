using NKMCore.Templates;

namespace NKMCore.Effects
{
	public class Silent : Effect
	{
		public Silent(Game game, int cooldown, Character parentCharacter, string name=null) : base(game, cooldown, parentCharacter, name)
		{
			Name = name?? "Silent";
			//CurrentCooldown = cooldown;
			Type = EffectType.Negative;
		}
		public override string GetDescription()
		{
			return "Ta postać nie może używać umiejętności.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
		public override bool IsCC => true;
	}
}
