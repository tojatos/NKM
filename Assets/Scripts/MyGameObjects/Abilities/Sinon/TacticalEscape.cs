using MyGameObjects.Effects;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Sinon
{
	public class TacticalEscape : Ability
	{
		private const int SpeedIncrease = 5;
		private const int Duration = 1;

		public TacticalEscape()
		{
			Name = "Tactical Escape";
			Cooldown = 4;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
		}
		public override string GetDescription()
		{
			return $@"Zwiększa szybkość {ParentCharacter.Name} o {SpeedIncrease}.
Czas trwania: {Duration}	Czas odnowienia: {Cooldown}";
		}
		protected override void Use()
		{
			ParentCharacter.Effects.Add(new StatModifier(Duration, SpeedIncrease, ParentCharacter, StatType.Speed, Name));
			//ParentCharacter.Effects.Add(new BasicAttackInability(Duration, ParentCharacter, Name));
			OnUseFinish();

		}
	}
}
