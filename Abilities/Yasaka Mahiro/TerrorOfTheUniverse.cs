using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Yasaka_Mahiro
{
	public class TerrorOfTheUniverse : Ability, IClickable, IUseableCellList
	{
		private const float CurrentHealthPercentDamage = 40f;
		private const int Range = 6;
		private const int Radius = 5;
		private const int SlowDuration = 2;
		public TerrorOfTheUniverse(Game game) : base(game, AbilityType.Ultimatum, "Terror Of The Universe", 6){}
		public override string GetDescription() =>
$@"{ParentCharacter.Name} wbija wielki widelec w ziemie,
zadając obrażenia fizyczne równe {CurrentHealthPercentDamage}% obecnego HP celu wszystkim wrogom w promieniu {Radius}.
Dodatkowo, na następne {SlowDuration} fazy wszyscy trafieni przeciwnicy są spowolnieni o połowę.

Zasięg: {Range}	Czas odnowienia: {Cooldown}";

		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);

		public void Click()
		{
			Active.Prepare(this, GetRangeCells(), false, false);
			Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, Radius);
		}
		public void Use(List<HexCell> cells)
		{
			ParentCharacter.TryToTakeTurn();
			List<Character> characters = cells.GetCharacters();
			characters.ForEach(targetCharacter =>
			{
				if (targetCharacter.Owner == Active.GamePlayer) return;

				int damageValue = (int) (CurrentHealthPercentDamage / 100 * targetCharacter.HealthPoints.Value);
				var damage = new Damage(damageValue, DamageType.Physical);
				ParentCharacter.Attack(this, targetCharacter, damage);
				targetCharacter.Effects.Add(new StatModifier(Game, 2, -(targetCharacter.Speed.Value / 2), targetCharacter, StatType.Speed, Name));
				//targetCharacter.Effects.Add(new MovementDisability(SlowDuration, targetCharacter, Name));
			});
			Finish();
		}
	}
}
