using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Shana
{
	public class GurenNoSouyoku : EnableableAbility
	{
		private const int Duration = 4;
		private const int SpeedBonus = 3;
		public GurenNoSouyoku()
		{
			Name = "Guren no Souyoku";
			Type = AbilityType.Passive;
		}
		public override bool IsEnabled => ParentCharacter.Effects.ContainsType(typeof(Effects.Flying));
		public override string GetDescription()
		{
			return "Po otrzymaniu obrażeń rozwija skrzydła dzięki którym może poruszyć się o 3 pola więcej, ponadto może przelatywać przez ściany\n" +
			       "Czas trwania efektu: " + Duration + " fazy.";
		}

		public override bool CanUse => false;
		
		private List<HexCell> GetBasicMoveCellsOverride()
		{
			bool isAbilityActive = ParentCharacter.Effects.ContainsType(typeof(Effects.Flying));
			if (!isAbilityActive) return ParentCharacter.DefaultGetBasicMoveCells();

			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(ParentCharacter.Speed.Value, SearchFlags.StopAtEnemyCharacters | SearchFlags.StopAtFriendlyCharacters);
			return cellRange;
		}

		public override void Awake()
		{
			ParentCharacter.GetBasicMoveCells = GetBasicMoveCellsOverride;
			ParentCharacter.AfterBeingDamaged += damage =>
			{
				ParentCharacter.Effects.Where(e => e.Name == Name).ToList().ForEach(e => e.RemoveFromParent());
				ParentCharacter.Effects.Add(new Effects.Flying(Duration, ParentCharacter, Name));
				ParentCharacter.Effects.Add(new Effects.StatModifier(Duration, SpeedBonus, ParentCharacter, StatType.Speed, Name));
			};
		}
	}
}
