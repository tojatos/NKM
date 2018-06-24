﻿using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Asuna
{
	public class LambentLight : Ability
	{
		private const int AaDamageModifier = 2;
		private const int Range = 2;
		
		public LambentLight() : base(AbilityType.Passive, "Lambent Light")
		{
			OnAwake += () =>
			{
				ParentCharacter.BeforeBasicAttack += (character, damage) =>
				{
					if (GetRangeCells().Contains(character.ParentCell)) damage.Value *= 2;
				};
			};
		}

		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(2);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

		public override string GetDescription() => 
$@"Jeżeli {ParentCharacter.Name} użyje ataku podstawowego na przeciwnika w zasięgu {Range},
zada on {AaDamageModifier * 100}% obrażeń.";
		
	}
}
