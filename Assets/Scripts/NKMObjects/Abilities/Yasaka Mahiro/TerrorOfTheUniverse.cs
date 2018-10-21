using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Yasaka_Mahiro
{
	public class TerrorOfTheUniverse : Ability, IClickable, IUseable
	{
		private const float CurrentHealthPercentDamage = 40f;
		private const int Range = 6;
		private const int Radius = 5;
		private const int SlowDuration = 2;
		public TerrorOfTheUniverse() : base(AbilityType.Ultimatum, "Terror Of The Universe", 6){}
		public override string GetDescription() =>
$@"{ParentCharacter.Name} wbija wielki widelec w ziemie,
zadając obrażenia fizyczne równe {CurrentHealthPercentDamage}% obecnego HP celu wszystkim wrogom w promieniu {Radius}.
Dodatkowo, na następne {SlowDuration} fazy wszyscy trafieni przeciwnicy są spowolnieni o połowę.

Zasięg: {Range}	Czas odnowienia: {Cooldown}";

		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);

		public void Click()
		{
			Active.Prepare(this, GetRangeCells(), false, false);
			Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, Radius);
		}
		public void Use(List<HexCell> cells)
		{
			List<NKMCharacter> characters = cells.GetCharacters();
			characters.ForEach(targetCharacter =>
			{
				if (targetCharacter.Owner == Active.GamePlayer) return;

				int damageValue = (int) (CurrentHealthPercentDamage / 100 * targetCharacter.HealthPoints.Value);
				var damage = new Damage(damageValue, DamageType.Physical);
				ParentCharacter.Attack(this, targetCharacter, damage);
				targetCharacter.Effects.Add(new StatModifier(2, -(targetCharacter.Speed.Value / 2), targetCharacter, StatType.Speed, Name));
				//targetCharacter.Effects.Add(new MovementDisability(SlowDuration, targetCharacter, Name));
			});
			Finish();
		}
	}
}
