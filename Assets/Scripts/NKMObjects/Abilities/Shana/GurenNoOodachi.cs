using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Shana
{
	public class GurenNoOodachi : Ability, IClickable
	{
		private const int AttackIncrease = 5;
		private const int BasicAttackRangeIncrease = 4;
		private const int Duration = 3;

		public GurenNoOodachi(Game game) : base(game, AbilityType.Normal, "Guren no Oodachi", 5)
		{
//			Name = "Guren no Oodachi";
//			Cooldown = 5;
//			CurrentCooldown = 0;
//			Type = AbilityType.Normal;
		}
		public override string GetDescription()
		{
			return $@"Zwiększa atak o {AttackIncrease}, oraz zasięg ataków o {BasicAttackRangeIncrease}
Czas trwania: {Duration}	Czas odnowienia: {Cooldown}";
		}

		public void Click()
		{
//			Active.MakeAction();
			ParentCharacter.TryToTakeTurn();
			ParentCharacter.Effects.Add(new StatModifier(Game, Duration, AttackIncrease, ParentCharacter, StatType.AttackPoints, Name));
			ParentCharacter.Effects.Add(new StatModifier(Game, Duration, BasicAttackRangeIncrease, ParentCharacter, StatType.BasicAttackRange, Name));
			Finish();

		}
	}
}
