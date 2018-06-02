using System;
using System.Collections.Generic;
using Hex;
using MyGameObjects.MyGameObject_templates;
using Helpers;
using System.Linq;

namespace MyGameObjects.Abilities.Shana
{
	public class GurenNoSouyoku : EnableableAbility
	{
		public GurenNoSouyoku()
		{
			Name = "Guren no Souyoku";
			Type = AbilityType.Passive;
			OverridesGetMoveCells = true;
		}
		public override bool IsEnabled => ParentCharacter.Effects.ContainsType(typeof(Effects.GurenNoSouyoku));
		public override string GetDescription()
		{
			return "Po otrzymaniu obrażeń rozwija skrzydła dzięki którym może poruszyć się o 3 pola więcej, ponadto może przelatywać przez ściany";
		}

		public override bool CanUse => false;

		public override List<HexCell> GetMoveCells()
		{
			List<HexCell> cellRange;
			bool isAbilityActive = ParentCharacter.Effects.ContainsType(typeof(Effects.GurenNoSouyoku));
			if(isAbilityActive) cellRange = ParentCharacter.ParentCell.GetNeighbors(ParentCharacter.Speed.Value + 3, false, true);
			else cellRange = ParentCharacter.ParentCell.GetNeighbors(ParentCharacter.Speed.Value, true, true);
			cellRange.RemoveAll(cell => cell.CharacterOnCell != null); //we don't want to allow stepping into our characters!
			return cellRange;
		}

		public override void Awake()
		{
			ParentCharacter.OnParentDamage += (damage) =>
			{
				ParentCharacter.Effects.Where(e => e.GetType() == typeof(Effects.GurenNoSouyoku)).ToList().ForEach(e => e.RemoveFromParent());
				ParentCharacter.Effects.Add(new Effects.GurenNoSouyoku(ParentCharacter));
			};
		}
	}
}
