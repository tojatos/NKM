﻿using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Gilgamesh
{
	public class GateOfBabylon : Ability
	{
		private const int AbilityDamage = 25;
		private const int AbilityRange = 6;
		private const int AbilityRadius = 6;
		public GateOfBabylon()
		{
			Name = "Gate Of Babylon";
			Cooldown = 5;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} otwiera wrota Babilonu, zsyłając deszcz mieczy na wskazanym obszarze w promieniu {1},
zadając {2} obrażeń magicznych lub fizycznych, zależnie od odporności przeciwnika.
Jeżeli wróg ma więcej obrony fizycznej od magicznej, umiejętność zada obrażenia magiczne,
a w przeciwnym razie - fizyczne.
Zasięg: {3}	Czas odnowienia: {4}
",
			ParentCharacter.Name, AbilityRadius, AbilityDamage, AbilityRange, Cooldown);
		}
		public override List<HexCell> GetRangeCells()
		{
			return ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		}
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			Active.Prepare(this, cellRange, false, false);
			Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, AbilityRadius);
		}
		public override void Use(List<HexCell> cells)
		{
			List<Character> characters = cells.GetCharacters();
			characters.ForEach(targetCharacter =>
			{
				if (targetCharacter.Owner == Active.GamePlayer) return;
				DamageType damageType = targetCharacter.MagicalDefense.Value <= targetCharacter.PhysicalDefense.Value
					? DamageType.Magical
					: DamageType.Physical;
				var damage = new Damage(AbilityDamage, damageType);

				ParentCharacter.Attack(targetCharacter, damage);
			});
			OnUseFinish();
		}
	}
}
