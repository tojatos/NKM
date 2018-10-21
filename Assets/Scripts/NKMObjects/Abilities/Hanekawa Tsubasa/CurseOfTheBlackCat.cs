using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Hanekawa_Tsubasa
{
	public class CurseOfTheBlackCat : Ability, IClickable, IUseable
	{
		private const int AbilityRange = 5;
		private const int DoTDamage = 6;
		private const int DoTTime = 5;
		private const int AdditionalDamagePercent = 25;

		public CurseOfTheBlackCat() : base(AbilityType.Ultimatum, "Curse of The Black Cat", 7)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

		public override string GetDescription() => string.Format(
@"{0} rzuca klątwę na wroga wysysając z niego {1} HP co fazę przez {2} fazy (zadaje obrażenia nieuchronne).
Podczas trwania efektu, {0} zadaje celowi klątwy dodatkowe {3}% obrażeń.
Zasięg: {4} Czas odnowienia: {5}",
			ParentCharacter.Name, DoTDamage, DoTTime, AdditionalDamagePercent, AbilityRange, Cooldown);

		public void Click() => Active.Prepare(this, GetTargetsInRange());
	    public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

		private void Use(NKMCharacter targetCharacter)
		{
			var damage = new Damage(DoTDamage, DamageType.True);
            targetCharacter.Effects.Add(new HPDrain(ParentCharacter, damage, DoTTime, targetCharacter, Name));
            Finish();
		}
	}
}
