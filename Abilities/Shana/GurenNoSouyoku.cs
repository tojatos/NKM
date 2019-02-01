using System.Linq;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Shana
{
	public class GurenNoSouyoku : Ability, IEnableable
	{
		private const int Duration = 4;
		private const int SpeedBonus = 3;
		public GurenNoSouyoku(Game game) : base(game, AbilityType.Passive, "Guren no Souyoku")
		{
			OnAwake += () =>
			{
				ParentCharacter.AfterBeingDamaged += damage =>
				{
					ParentCharacter.Effects.Where(e => e.Name == Name).ToList().ForEach(e => e.RemoveFromParent());
					ParentCharacter.Effects.Add(new Effects.Flying(Game, Duration, ParentCharacter, Name));
					ParentCharacter.Effects.Add(new Effects.StatModifier(Game, Duration, SpeedBonus, ParentCharacter, StatType.Speed, Name));
				};
			};
		}
		public bool IsEnabled => ParentCharacter.Effects.ContainsType(typeof(Effects.Flying));
		public override string GetDescription() =>
$@"Po otrzymaniu obrażeń rozwija skrzydła dzięki którym może poruszyć się o {SpeedBonus} pola więcej, ponadto może przelatywać przez ściany
Czas trwania efektu: {Duration} fazy.";
	}
}
