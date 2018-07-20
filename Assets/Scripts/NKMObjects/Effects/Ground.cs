using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class Ground : Effect
	{
		public Ground(int cooldown, Character parentCharacter, string name=null) : base(cooldown, parentCharacter, name)
		{
			Name = name?? "Ground";
			Type = EffectType.Negative;
		}
		public override string GetDescription()
		{
			return "Ta postać jest uziemiona.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
		public override bool IsCC => true;
	}
}
