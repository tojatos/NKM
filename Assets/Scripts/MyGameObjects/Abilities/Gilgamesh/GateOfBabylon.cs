using System.Collections.Generic;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Gilgamesh
{
	public class GateOfBabylon : Ability
	{
		private const int AbilityDamage = 25;
		private const int AbilityRange = 10;
		private const int AbilityRadius = 9;
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
			var cellRange = GetRangeCells();
			Active.Prepare(this, cellRange, false, false);
			Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, AbilityRadius);
		}
		public override void Use(List<Character> characters)
		{
			characters.ForEach(targetCharacter =>
			{
				if (targetCharacter.Owner == Active.Player) return;

				ParentCharacter.Attack(targetCharacter, targetCharacter.MagicalDefense.Value <= targetCharacter.PhysicalDefense.Value ? AttackType.Magical : AttackType.Physical, AbilityDamage);
			});
			OnUseFinish();
		}
	}
}
