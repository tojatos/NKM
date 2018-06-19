using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Crona
{
    public class Infection : Ability
    {
        private const int Range = 4;
        private const int EffectCooldown = 3;
        public Infection()
        {
            Name = "Infection";
            Cooldown = 5;
            CurrentCooldown = 0;
            Type = AbilityType.Ultimatum;
        }
        public override string GetDescription()
        {
            return $"{ParentCharacter.Name} infekuje cel Czarną Krwią (nakłada efekt Black Blood) na 3 tury.\nZainfekowany wróg również otrzymuje obrażenia przy zdetonowaniu Black Blood.";
        }
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
        protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
			OnFailedUseFinish();
		}
		public override void Use(Character character)
		{
			character.Effects.Add(new Effects.BlackBlood(ParentCharacter, character, EffectCooldown));
			OnUseFinish();
		}
    }
}