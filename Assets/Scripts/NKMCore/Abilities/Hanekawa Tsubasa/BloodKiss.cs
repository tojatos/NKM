using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Hanekawa_Tsubasa
{
	public class BloodKiss : Ability, IClickable, IUseableCharacter
	{
		private const int Range = 3;
		private const int DoTDamage = 8;
		private const int DoTTime = 4;

		public BloodKiss(Game game) : base(game, AbilityType.Normal, "Blood Kiss", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);
		
		public override string GetDescription() =>
$@"{ParentCharacter.Name} liże wroga, wywołując silne krwawienie, które zadaje {DoTDamage} nieuchronnych obrażeń przez {DoTTime} fazy.
Zasięg: {Range} Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());

		public void Use(Character targetCharacter)
		{
            ParentCharacter.TryToTakeTurn();
			var damage = new Damage(DoTDamage, DamageType.True);
			targetCharacter.Effects.Add(new Poison(Game, ParentCharacter, damage, DoTTime, targetCharacter, Name));
			Finish();
		}
	}
}
