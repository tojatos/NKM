using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Shana
{
	public class GurenNoSouyoku : Ability, IEnableable
	{
		private const int Duration = 4;
		private const int SpeedBonus = 3;
		public GurenNoSouyoku() : base(AbilityType.Passive, "Guren no Souyoku")
		{
			OnAwake += () =>
			{
				ParentCharacter.GetBasicMoveCells = GetBasicMoveCellsOverride;
				ParentCharacter.AfterBeingDamaged += damage =>
				{
					ParentCharacter.Effects.Where(e => e.Name == Name).ToList().ForEach(e => e.RemoveFromParent());
					ParentCharacter.Effects.Add(new Effects.Flying(Duration, ParentCharacter, Name));
					ParentCharacter.Effects.Add(new Effects.StatModifier(Duration, SpeedBonus, ParentCharacter, StatType.Speed, Name));
				};
			};
		}
		public bool IsEnabled => ParentCharacter.Effects.ContainsType(typeof(Effects.Flying));
		public override string GetDescription() =>
$@"Po otrzymaniu obrażeń rozwija skrzydła dzięki którym może poruszyć się o {SpeedBonus} pola więcej, ponadto może przelatywać przez ściany
Czas trwania efektu: {Duration} fazy.";

		private List<HexCell> GetBasicMoveCellsOverride()
		{
			bool isAbilityActive = ParentCharacter.Effects.ContainsType(typeof(Effects.Flying));
			if (!isAbilityActive) return ParentCharacter.DefaultGetBasicMoveCells();

			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(ParentCharacter.Speed.Value, SearchFlags.StopAtEnemyCharacters | SearchFlags.StopAtFriendlyCharacters);
			return cellRange;
		}
	}
}
