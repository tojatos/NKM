using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class IncreasablePoison : Effect
	{
		public readonly Damage Damage;
		private readonly int _increase;

		public IncreasablePoison(Game game, Character characterThatAttacks, Damage initialDamage, int increase, int cooldown, Character parentCharacter, string name = null) : base(game, cooldown, parentCharacter, name)
		{
			Name = name ?? "Increasable Poison";
			Damage = initialDamage;
			Type = EffectType.Negative;
			_increase = increase;
            Character.VoidDelegate tryToActivateEffect = () =>
            {
	            characterThatAttacks.Attack(this, ParentCharacter, Damage);
	            Damage.Value += _increase;
            };
			ParentCharacter.JustBeforeFirstAction += tryToActivateEffect;
			OnRemove += () => ParentCharacter.JustBeforeFirstAction -= tryToActivateEffect;
		}
		public override string GetDescription() => 
$@"Zadaje {Damage.Value} obrażeń co fazę, zwiększając się o {_increase} po każdym użyciu.
Czas do zakończenia efektu: {CurrentCooldown}";
	}
}
