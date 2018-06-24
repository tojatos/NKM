using System;
using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Sinon
{
	public class SnipersSight : Ability
	{
		public SnipersSight() : base(AbilityType.Passive, "Sniper's Sight")
		{
			OnAwake += () =>
			{
				ParentCharacter.GetBasicAttackCells = GetBasicAttackCellsOverride;
			};
		}

		public override string GetDescription() => "Zasięg ataków podstawowych tej postaci jest kulisty.";
		public override List<HexCell> GetRangeCells() => ParentCharacter.GetBasicAttackCells();
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

		private List<HexCell> GetBasicAttackCellsOverride()
		{
			switch (ParentCharacter.Type)
			{
				case FightType.Ranged:
					return ParentCharacter.ParentCell.GetNeighbors(ParentCharacter.BasicAttackRange.Value);
				case FightType.Melee:
					return ParentCharacter.ParentCell.GetNeighbors(ParentCharacter.BasicAttackRange.Value, SearchFlags.StopAtWalls);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
