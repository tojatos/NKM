using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Sinon
{
	public class TacticalEscape : Ability, IClickable
	{
		private const int SpeedIncrease = 8;
		private const int Duration = 1;

		public TacticalEscape() : base(AbilityType.Normal, "Tactical Escape", 4)
		{
//			Name = "Tactical Escape";
//			Cooldown = 4;
//			CurrentCooldown = 0;
//			Type = AbilityType.Normal;
		}
		public override string GetDescription() => 
$@"Zwiększa szybkość {ParentCharacter.Name} o {SpeedIncrease}.
Czas trwania: {Duration}	Czas odnowienia: {Cooldown}";

		public void Click()
		{
			Active.MakeAction();
			ParentCharacter.Effects.Add(new StatModifier(Duration, SpeedIncrease, ParentCharacter, StatType.Speed, Name));
			//ParentCharacter.Effects.Add(new BasicAttackInability(Duration, ParentCharacter, Name));
			OnUseFinish();

		}
	}
}
