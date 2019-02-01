using NKMCore.Templates;

namespace NKMCore.Effects
{
	public class Flying : Effect
	{
		public Flying(Game game, int cooldown, Character parentCharacter, string name) : base(game, cooldown, parentCharacter, name)
		{
			Name = name ?? "Flying";
			Type = EffectType.Positive;
		}
		public override string GetDescription()
		{
			return $"{ParentCharacter.Name} może latać.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
	}
}
