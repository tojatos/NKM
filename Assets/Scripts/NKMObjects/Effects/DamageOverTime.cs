using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class DamageOverTime : Effect
	{
		private readonly Damage _damagePerTick;

		public DamageOverTime(Character characterThatAttacks, Damage damagePerTick, int cooldown, Character parentCharacter, string name = null) : base(cooldown, parentCharacter, name)
		{
			Name = name ?? "Damage Over Time";
			_damagePerTick = damagePerTick;
			Type = EffectType.Negative;
            Character.VoidDelegate tryToActivateEffect = () => characterThatAttacks.Attack(ParentCharacter, damagePerTick);
			ParentCharacter.JustBeforeFirstAction += tryToActivateEffect;
			OnRemove += () => ParentCharacter.JustBeforeFirstAction -= tryToActivateEffect;
		}
		public override string GetDescription()
		{
			return "Zadaje " + _damagePerTick + " obrażeń co fazę.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
	}
}
