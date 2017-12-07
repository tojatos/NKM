using System.Collections.Generic;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Asuna
{
	public class LambentLight : Ability
	{
		private const int AaDamageModifier = 2;
		private const int Range = 2;
		public LambentLight()
		{
			Name = "Lambent Light";
			Type = AbilityType.Passive;
			OverridesEnemyAttack = true;
		}

		public override List<HexCell> GetRangeCells()
		{
			return ParentCharacter.ParentCell.GetNeighbors(2);
		}

		public override string GetDescription()
		{
			return string.Format(
@"Jeżeli {0} użyje ataku podstawowego na przeciwnika w zasięgu {1},
zada on {2}% obrażeń."
				, ParentCharacter.Name, Range, AaDamageModifier*100);
		}

		public override void AttackEnemy(Character attackedCharacter)
		{
			var modifier = 1;
			if (GetRangeCells().Contains(attackedCharacter.ParentCell)) modifier = AaDamageModifier;
			ParentCharacter.Attack(attackedCharacter, AttackType.Physical, ParentCharacter.AttackPoints.Value * modifier);
		}
	}
}
