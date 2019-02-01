using NKMCore.Templates;

namespace NKMCore.Effects
{
	public class Snare : Effect
	{
		public Snare(Game game, int cooldown, Character parentCharacter, string name=null) : base(game, cooldown, parentCharacter, name)
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
