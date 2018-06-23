using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Hanekawa_Tsubasa
{
	public class BloodKiss : Ability, IClickable
	{
		private const int AbilityRange = 3;
		private const int DoTDamage = 8;
		private const int DoTTime = 4;

		public BloodKiss() : base(AbilityType.Normal, "Blood Kiss", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemies();
		
		public override string GetDescription() =>
$@"{ParentCharacter.Name} liże wroga, wywołując silne krwawienie, które zadaje {DoTDamage} nieuchronnych obrażeń przez {DoTTime} fazy.
Zasięg: {AbilityRange} Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());
		public override void Use(Character targetCharacter)
		{
			var damage = new Damage(DoTDamage, DamageType.True);
			targetCharacter.Effects.Add(new DamageOverTime(ParentCharacter, damage, DoTTime, targetCharacter, Name));
			OnUseFinish();
		}
	}
}
