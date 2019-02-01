using NKMCore.Templates;

namespace NKMCore.Effects
{
	public class Disarm : Effect
	{
		public Disarm(Game game, int cooldown, Character parentCharacter, string name=null) : base(game, cooldown, parentCharacter, name)
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
