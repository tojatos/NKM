using NKMObjects.Templates;

namespace NKMObjects.Abilities
{
	public class Empty : Ability
	{
		public Empty(AbilityType type) : base(type, "Pusta umiejętność"){}
		public override string GetDescription() => "Twojego bohatera najwyraźniej nie stać na lepszą umiejętność.";
	}
}
