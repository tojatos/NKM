using NKMCore.Templates;

namespace NKMCore.Abilities
{
	public class Empty : Ability
	{
		public Empty(Game game, AbilityType type) : base(game, type, "Pusta umiejętność"){}
		public override string GetDescription() => "Twojego bohatera najwyraźniej nie stać na lepszą umiejętność.";
	}
}
