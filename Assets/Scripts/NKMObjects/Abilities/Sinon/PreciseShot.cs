using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Sinon
{
	public class PreciseShot : Ability, IClickable
	{
		private const int AbilityDamage = 40;
		private const int AbilityRange = 11;

		public PreciseShot() : base(AbilityType.Ultimatum, "Precise Shot", 6)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);
		
		public override string GetDescription() => $@"{ParentCharacter.Name} strzela w wybranego wroga, zadając {AbilityDamage} obrażeń fizycznych.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());

		public override void Use(Character character)
		{
			var damage = new Damage(AbilityDamage, DamageType.Physical);
			ParentCharacter.Attack(this, character, damage);
			OnUseFinish();
		}
	}
}
