using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class HPDrain : Effect
	{
		private readonly NKMCharacter _characterThatAttacks;
		private readonly Damage _damagePerTick;

		public HPDrain(NKMCharacter characterThatAttacks, Damage damagePerTick, int cooldown, NKMCharacter parentCharacter, string name = null) : base(cooldown, parentCharacter, name)
		{
			Name = name ?? "HP Drain";
			_damagePerTick = damagePerTick;
			_characterThatAttacks = characterThatAttacks;
			Type = EffectType.Negative;
			Character.VoidDelegate tryToActivateEffect = () =>
			{
				_characterThatAttacks.Attack(this, ParentCharacter, _damagePerTick);
				if (ParentCharacter.IsAlive && _characterThatAttacks.IsAlive)
					ParentCharacter.Heal(_characterThatAttacks, _damagePerTick.Value);
			};
			ParentCharacter.JustBeforeFirstAction += tryToActivateEffect;
			OnRemove += () => ParentCharacter.JustBeforeFirstAction -= tryToActivateEffect;
		}
		public override string GetDescription()
		{
			return string.Format(
@"Zadaje {0} obrażeń co fazę, oraz leczy za tą samą ilość bohatera, który nałożył ten efekt (<b>{1}</b>).
Czas do zakończenia efektu: {2}",
						 _damagePerTick, _characterThatAttacks.Name, CurrentCooldown);
		}
	}
}
