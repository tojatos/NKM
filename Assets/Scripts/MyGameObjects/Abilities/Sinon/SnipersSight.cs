using System;
using System.Collections.Generic;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Sinon
{
	public class SnipersSight : Ability
	{
		public SnipersSight()
		{
			Name = "Sniper's Sight";
			Type = AbilityType.Passive;
			OverridesGetBasicAttackCells = true;
		}

		public override string GetDescription()
		{
			return "Zasięg ataków podstawowych tej postaci jest kulisty.";
		}

		public override bool CanUse => false;

		public override List<HexCell> GetBasicAttackCells()
		{
			List<HexCell> cellRange;
			switch (ParentCharacter.Type)
			{
				case FightType.Ranged:
					cellRange = ParentCharacter.ParentCell.GetNeighbors(ParentCharacter.BasicAttackRange.Value);
					break;
				case FightType.Melee:
					cellRange = ParentCharacter.ParentCell.GetNeighbors(ParentCharacter.BasicAttackRange.Value, true);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return cellRange;
		}
	}
}
