using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Gilgamesh
{
	public class Enkidu : Ability, IClickable, IUseable
	{
		private const int AbilityRange = 8;
		private const int SnarDuration = 2;

		public Enkidu() : base(AbilityType.Normal, "Enkidu", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange, SearchFlags.StraightLine);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);
		
		public override string GetDescription() =>
$@"{ParentCharacter.Name} wypuszcza niebiańskie łańcuchy, unieruchamiając przeciwnika na {SnarDuration} fazy
i podwajając bonusy zdolności biernej na ten okres.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());
	    public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

		private void Use(Character targetCharacter)
		{
				ParentCharacter.Effects.Add(new PassiveBuff(SnarDuration, ParentCharacter, Name));
				targetCharacter.Effects.Add(new MovementDisability(SnarDuration, targetCharacter, Name));
				Finish();
		}

	}
}
