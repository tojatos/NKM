﻿using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Gilgamesh
{
	public class GateOfBabylon : Ability, IClickable, IUseable
	{
		private const int Damage = 25;
		private const int Range = 6;
		private const int Radius = 6;
		public GateOfBabylon(Game game) : base(game, AbilityType.Ultimatum, "Gate Of Babylon", 5) {}
		
		public override string GetDescription() =>
$@"{ParentCharacter.Name} otwiera wrota Babilonu, zsyłając deszcz mieczy na wskazanym obszarze w promieniu {Radius},
zadając {Damage} obrażeń magicznych lub fizycznych, zależnie od odporności przeciwnika.
Jeżeli wróg ma więcej obrony fizycznej od magicznej, umiejętność zada obrażenia magiczne,
a w przeciwnym razie - fizyczne.

Zasięg: {Range}	Czas odnowienia: {Cooldown}";

		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);

		public void Click()
		{
			List<HexCell> cellRange = GetRangeCells();
			Active.Prepare(this, cellRange, false, false);
			Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, Radius);
		}
		public void Use(List<HexCell> cells)
		{
			List<Character> characters = cells.GetCharacters();
			characters.ForEach(targetCharacter =>
			{
				if (targetCharacter.Owner == Active.GamePlayer) return;
				DamageType damageType = targetCharacter.MagicalDefense.Value <= targetCharacter.PhysicalDefense.Value
					? DamageType.Magical
					: DamageType.Physical;
				var damage = new Damage(Damage, damageType);

				ParentCharacter.Attack(this, targetCharacter, damage);
			});
			Finish();
		}
	}
}
