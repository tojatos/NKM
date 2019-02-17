using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Rem
{
	public class MorgensternHit : Ability, IClickable
	{
		private const int AbilityDamage = 15;
		private const int AbilityRange = 4;

		public MorgensternHit(Game game) : base(game, AbilityType.Normal, "Morgenstern Hit", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

		public override string GetDescription() => 
$@"{ParentCharacter.Name} wymachuje morgenszternem wokół własnej osi,
zadając wszystkim przeciwnikom w promieniu {AbilityRange} {AbilityDamage} obrażeń fizycznych.
Czas odnowienia: {Cooldown}";

		public void Click()
		{
			ParentCharacter.TryToTakeTurn();
			GetTargetsInRange().GetCharacters().ForEach(c => ParentCharacter.Attack(this, c, new Damage(AbilityDamage, DamageType.Physical)));
			Finish();
		}
	}
}
