using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class Flying : Effect
	{
		public Flying(int cooldown, NKMCharacter parentCharacter, string name) : base(cooldown, parentCharacter, name)
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
