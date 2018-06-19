using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Yasaka_Mahiro
{
	public class TerrorOfTheUniverse : Ability
	{
		private const float AbilityCurrentHealthPercentDamage = 40f;
		private const int AbilityRange = 6;
		private const int AbilityRadius = 5;
		private const int SlowDuration = 2;
		public TerrorOfTheUniverse()
		{
			Name = "Terror Of The Universe";
			Cooldown = 6;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} wbija wielki widelec w ziemie, zadając obrażenia fizyczne równe {1}% obecnego HP celu wszystkim wrogom w promieniu {2}.
Dodatkowo, na następne {3} fazy wszyscy trafieni przeciwnicy są spowolnieni o połowę.
Zasięg: {4}	Czas odnowienia: {5}",
				ParentCharacter.Name, AbilityCurrentHealthPercentDamage, AbilityRadius, SlowDuration, AbilityRange, Cooldown);
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

				int damageValue = (int) (AbilityCurrentHealthPercentDamage / 100 * targetCharacter.HealthPoints.Value);
				var damage = new Damage(damageValue, DamageType.Physical);
				ParentCharacter.Attack(targetCharacter, damage);
				targetCharacter.Effects.Add(new StatModifier(2, -(targetCharacter.Speed.Value / 2), targetCharacter, StatType.Speed, Name));
				//targetCharacter.Effects.Add(new MovementDisability(SlowDuration, targetCharacter, Name));
			});
			OnUseFinish();
		}
	}
}
