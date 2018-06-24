using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Rem
{
	public class AlHuma : Ability, IClickable
	{
		private const int AbilityDamage = 10;
		private const int AbilityRange = 7;

		public AlHuma() : base(AbilityType.Normal, "Al Huma", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);
		
		public override string GetDescription() => 
$@"{ParentCharacter.Name} zamraża jednego wroga w zasięgu {AbilityRange} na jedną turę,
zadając {AbilityDamage} obrażeń magicznych.
Czas odnowiania: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());
		public override void Use(Character targetCharacter)
		{
			var damage = new Damage(AbilityDamage, DamageType.Magical);
			ParentCharacter.Attack(this,targetCharacter, damage);
			targetCharacter.Effects.Add(new Stun(1, targetCharacter, Name));
			OnUseFinish();
		}
	}
}
