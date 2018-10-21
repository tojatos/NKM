using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class Poison : Effect
	{
		private readonly Damage _damagePerTick;

		public Poison(NKMCharacter characterThatAttacks, Damage damagePerTick, int cooldown, NKMCharacter parentCharacter, string name = null) : base(cooldown, parentCharacter, name)
		{
			Name = name ?? "Poison";
			_damagePerTick = damagePerTick;
			Type = EffectType.Negative;
            Character.VoidDelegate tryToActivateEffect = () => characterThatAttacks.Attack(this, ParentCharacter, damagePerTick);
			ParentCharacter.JustBeforeFirstAction += tryToActivateEffect;
			OnRemove += () => ParentCharacter.JustBeforeFirstAction -= tryToActivateEffect;
		}
		public override string GetDescription()
		{
			return "Zadaje " + _damagePerTick.Value + " obrażeń co fazę.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
	}
}
