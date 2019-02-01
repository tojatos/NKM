using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Asuna
{
	public class LambentLight : Ability
	{
		private const int AaDamageModifier = 2;
		private const int Range = 2;
		
		public LambentLight(Game game) : base(game, AbilityType.Passive, "Lambent Light")
		{
			OnAwake += () =>
			{
				ParentCharacter.BeforeBasicAttack += (character, damage) =>
				{
					if (GetRangeCells().Contains(character.ParentCell)) damage.Value *= 2;
				};
			};
		}

		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

		public override string GetDescription() => 
$@"Jeżeli {ParentCharacter.Name} użyje ataku podstawowego na przeciwnika w zasięgu {Range},
zada on {AaDamageModifier * 100}% obrażeń.";
		
	}
}
