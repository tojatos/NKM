using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Hanekawa_Tsubasa
{
	public class BloodKiss : Ability, IClickable, IUseable
	{
		private const int Range = 3;
		private const int DoTDamage = 8;
		private const int DoTTime = 4;

		public BloodKiss() : base(AbilityType.Normal, "Blood Kiss", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);
		
		public override string GetDescription() =>
$@"{ParentCharacter.Name} liże wroga, wywołując silne krwawienie, które zadaje {DoTDamage} nieuchronnych obrażeń przez {DoTTime} fazy.
Zasięg: {Range} Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());
	    public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

		private void Use(NKMCharacter targetCharacter)
		{
			var damage = new Damage(DoTDamage, DamageType.True);
			targetCharacter.Effects.Add(new Poison(ParentCharacter, damage, DoTTime, targetCharacter, Name));
			Finish();
		}
	}
}
