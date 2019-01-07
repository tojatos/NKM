using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Rem
{
	public class MorgensternHit : Ability, IClickable, IUseable
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
			Active.Prepare(this, GetTargetsInRange());
			Active.MakeAction(Active.HexCells);
		}
		public void Use(List<HexCell> cells)
		{
			cells.GetCharacters().ForEach(c => ParentCharacter.Attack(this, c, new Damage(AbilityDamage, DamageType.Physical)));
			Finish();
		}
	}
}
