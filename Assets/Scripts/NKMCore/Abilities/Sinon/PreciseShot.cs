using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Sinon
{
	public class PreciseShot : Ability, IClickable, IUseableCharacter
	{
		private const int Damage = 40;
		private const int Range = 11;

		public PreciseShot(Game game) : base(game, AbilityType.Ultimatum, "Precise Shot", 6)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);
		
		public override string GetDescription() => 
$@"{ParentCharacter.Name} strzela w wybranego wroga, zadając {Damage} obrażeń fizycznych.

Zasięg: {Range}	Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());

		public void Use(Character character)
		{
			ParentCharacter.TryToTakeTurn();
			var damage = new Damage(Damage, DamageType.Physical);
			ParentCharacter.Attack(this, character, damage);
			Finish();
		}
	}
}
