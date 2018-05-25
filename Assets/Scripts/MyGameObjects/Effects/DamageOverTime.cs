using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Effects
{
	public class DamageOverTime : Effect
	{
		private readonly int _damagePerTick;

		public DamageOverTime(Character characterThatAttacks, int damagePerTick, AttackType attackType, int cooldown, Character parentCharacter, string name = null) : base(cooldown, parentCharacter, name)
		{
			Name = name ?? "Damage Over Time";
			_damagePerTick = damagePerTick;
			Type = EffectType.Negative;
			ParentCharacter.JustBeforeFirstAction += () => characterThatAttacks.Attack(ParentCharacter, attackType, _damagePerTick);
		}
		public override string GetDescription()
		{
			return "Zadaje " + _damagePerTick + " obrażeń co fazę.\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
	}
}
