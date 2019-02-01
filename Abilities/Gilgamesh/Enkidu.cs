using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Gilgamesh
{
	public class Enkidu : Ability, IClickable, IUseableCharacter
	{
		private const int AbilityRange = 8;
		private const int SnarDuration = 2;

		public Enkidu(Game game) : base(game, AbilityType.Normal, "Enkidu", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(AbilityRange, SearchFlags.StraightLine);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);
		
		public override string GetDescription() =>
$@"{ParentCharacter.Name} wypuszcza niebiańskie łańcuchy, uziemiając przeciwnika na {SnarDuration} fazy
i podwajając bonusy zdolności biernej na ten okres.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());

		public void Use(Character targetCharacter)
		{
            ParentCharacter.TryToTakeTurn();
            ParentCharacter.Effects.Add(new PassiveBuff(Game, SnarDuration, ParentCharacter, Name));
            targetCharacter.Effects.Add(new Ground(Game, SnarDuration, targetCharacter, Name));
            Finish();
		}

	}
}
