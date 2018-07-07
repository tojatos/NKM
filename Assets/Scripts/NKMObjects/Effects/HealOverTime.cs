using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class HealOverTime : Effect
	{
		private readonly int _healPerTick;

		public HealOverTime(Character characterThatHeals, int healPerTick, int cooldown, Character parentCharacter, string name = null) : base(cooldown, parentCharacter, name)
		{
			Name = name ?? "Heal Over Time";
			_healPerTick = healPerTick;
			Type = EffectType.Positive;
            Character.VoidDelegate tryToActivateEffect = () => characterThatHeals.Heal(ParentCharacter, healPerTick);
			ParentCharacter.JustBeforeFirstAction += tryToActivateEffect;
			OnRemove += () => ParentCharacter.JustBeforeFirstAction -= tryToActivateEffect;
		}
		public override string GetDescription()
		{
			return "Leczy " + _healPerTick + " HP co fazę.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
	}
}
