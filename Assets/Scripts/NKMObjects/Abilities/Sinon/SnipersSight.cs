using System;
using System.Collections.Generic;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Sinon
{
	public class SnipersSight : Ability
	{
		public SnipersSight()
		{
			Name = "Sniper's Sight";
			Type = AbilityType.Passive;
		}

		public override string GetDescription()
		{
			return "Zasięg ataków podstawowych tej postaci jest kulisty.";
		}

		public override bool CanUse => false;
		public override void Awake() => ParentCharacter.GetBasicAttackCells = GetBasicAttackCellsOverride;

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
