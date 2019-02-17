using NKMCore.Templates;

namespace NKMCore.Effects
{
	public class HPDrain : Effect
	{
		private readonly Character _characterThatAttacks;
		private readonly Damage _damagePerTick;

		public HPDrain(Game game, Character characterThatAttacks, Damage damagePerTick, int cooldown,
			Character parentCharacter, string name = null) : base(game, cooldown, parentCharacter, name)
		{
			Name = name ?? "HP Drain";
			_damagePerTick = damagePerTick;
			_characterThatAttacks = characterThatAttacks;
			Type = EffectType.Negative;
			Delegates.Void tryToActivateEffect = () =>
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
			return
$@"Zadaje {_damagePerTick} obrażeń co fazę, oraz leczy za tą samą ilość bohatera, który nałożył ten efekt (<b>{_characterThatAttacks.Name}</b>).
Czas do zakończenia efektu: {CurrentCooldown}";
		}
	}
}
