using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Gilgamesh
{
	public class Enkidu : Ability, IClickable
	{
		private const int AbilityRange = 8;
		private const int SnarDuration = 2;

		public Enkidu() : base(AbilityType.Normal, "Enkidu", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemies();
		
		public override string GetDescription() =>
$@"{ParentCharacter.Name} wypuszcza niebiańskie łańcuchy, unieruchamiając przeciwnika na {SnarDuration} fazy
i podwajając bonusy zdolności biernej na ten okres.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());

		public override void Use(Character targetCharacter)
		{
//			try
//			{
				ParentCharacter.Effects.Add(new PassiveBuff(SnarDuration, ParentCharacter, Name));
				targetCharacter.Effects.Add(new MovementDisability(SnarDuration, targetCharacter, Name));
				OnUseFinish();
//			}
//			catch (Exception e)
//			{
//				MessageLogger.DebugLog(e.Message);
//				OnFailedUseFinish();
//			}
		}
	}
}
