using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class Silent : Effect
	{
		public Silent(int cooldown, NKMCharacter parentCharacter, string name=null) : base(cooldown, parentCharacter, name)
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
