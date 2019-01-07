using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Rem
{
	public class AlHuma : Ability, IClickable, IUseable
	{
		private const int Damage = 10;
		private const int Range = 7;

		public AlHuma(Game game) : base(game, AbilityType.Normal, "Al Huma", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);
		
		public override string GetDescription() => 
$@"{ParentCharacter.Name} zamraża jednego wroga w zasięgu {Range} na jedną turę,
ogłuszając go i zadając {Damage} obrażeń magicznych.

Czas odnowiania: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());
	    public void Use(List<HexCell> cells) => Use(cells[0].CharactersOnCell[0]);
		private void Use(Character targetCharacter)
		{
			var damage = new Damage(Damage, DamageType.Magical);
			ParentCharacter.Attack(this,targetCharacter, damage);
			targetCharacter.Effects.Add(new Stun(Game, 1, targetCharacter, Name));
			Finish();
		}
	}
}
