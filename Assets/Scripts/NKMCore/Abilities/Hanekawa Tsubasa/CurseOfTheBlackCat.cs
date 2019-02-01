﻿using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Hanekawa_Tsubasa
{
	public class CurseOfTheBlackCat : Ability, IClickable, IUseableCharacter
	{
		private const int AbilityRange = 5;
		private const int DoTDamage = 6;
		private const int DoTTime = 5;
		private const int AdditionalDamagePercent = 25;

		public CurseOfTheBlackCat(Game game) : base(game, AbilityType.Ultimatum, "Curse of The Black Cat", 7)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

		public override string GetDescription() => string.Format(
@"{0} rzuca klątwę na wroga wysysając z niego {1} HP co fazę przez {2} fazy (zadaje obrażenia nieuchronne).
Podczas trwania efektu, {0} zadaje celowi klątwy dodatkowe {3}% obrażeń.
Zasięg: {4} Czas odnowienia: {5}",
			ParentCharacter.Name, DoTDamage, DoTTime, AdditionalDamagePercent, AbilityRange, Cooldown);

		public void Click() => Active.Prepare(this, GetTargetsInRange());

		public void Use(Character targetCharacter)
		{
            ParentCharacter.TryToTakeTurn();
			var damage = new Damage(DoTDamage, DamageType.True);
            targetCharacter.Effects.Add(new HPDrain(Game, ParentCharacter, damage, DoTTime, targetCharacter, Name));
            Finish();
		}
	}
}
