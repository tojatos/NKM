﻿using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Hanekawa_Tsubasa
{
	public class NineLives : Ability
	{
		private const int HealthRegainedPercent = 25;
		public NineLives(Game game) : base(game, AbilityType.Passive, "Nine Lives")
		{
			OnAwake += () =>
			{
				ParentCharacter.AfterAttack += (character, damage) =>
				{
					if(!ParentCharacter.IsAlive) return;
					int modifier = (character.Effects.ContainsType(typeof(BloodKiss))) ? 2 : 1;
					int amountToHeal = damage.Value * HealthRegainedPercent / 100 * modifier;
					ParentCharacter.Heal(ParentCharacter, amountToHeal);
				};
			};
		}
		public override string GetDescription() => 
$@"{ParentCharacter.Name} odzyskuje {HealthRegainedPercent}% wszystkich zadanych obrażeń przez w formie HP.
Jeżeli zaatakowany przeciwnik posiada efekt Blood Kiss, ta premia jest przyznawana podwójnie.";
	}
}
