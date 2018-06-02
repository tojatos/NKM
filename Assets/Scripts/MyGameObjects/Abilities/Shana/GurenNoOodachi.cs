using MyGameObjects.Effects;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Shana
{
	public class GurenNoOodachi : Ability
	{
		private const int AttackIncrease = 5;
		private const int BasicAttackRangeIncrease = 4;
		private const int Duration = 2;

		public GurenNoOodachi()
		{
			Name = "Guren no Oodachi";
			Cooldown = 5;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
		}
		public override string GetDescription()
		{
			return $@"Zwiększa atak o {AttackIncrease}, oraz zasięg ataków o {BasicAttackRangeIncrease}
Czas trwania: {Duration}	Czas odnowienia: {Cooldown}";
		}
		protected override void Use()
		{
			ParentCharacter.Effects.Add(new StatModifier(Duration, AttackIncrease, ParentCharacter, StatType.AttackPoints, Name));
			ParentCharacter.Effects.Add(new StatModifier(Duration, BasicAttackRangeIncrease, ParentCharacter, StatType.BasicAttackRange, Name));
			OnUseFinish();

		}
	}
}
