using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Crona
{
    public class Infection : Ability, IClickable
    {
        private const int Range = 4;
        private const int EffectCooldown = 3;
        public Infection() : base(AbilityType.Ultimatum, "Infection", 5)
        {
//            Name = "Infection";
//            Cooldown = 5;
//            CurrentCooldown = 0;
//            Type = AbilityType.Ultimatum;
        }
        public override string GetDescription() => 
$@"{ParentCharacter.Name} infekuje cel Czarną Krwią (nakłada efekt Black Blood) na {EffectCooldown} tury.
Zainfekowany wróg również otrzymuje obrażenia przy zdetonowaniu Black Blood.
Zasięg: {Range}	Czas odnowienia: {CurrentCooldown}";
	    
	    public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
	    public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

	    public void Click()
		{
//			List<HexCell> cellRange = GetRangeCells();
//			cellRange.RemoveNonEnemies();
//			var canUseAbility = Active.Prepare(this, cellRange);
//			if (canUseAbility) return;

//			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
//			OnFailedUseFinish();
			Active.Prepare(this, GetTargetsInRange());
		}
		public override void Use(Character character)
		{
			character.Effects.Add(new Effects.BlackBlood(ParentCharacter, character, EffectCooldown, BlackBlood.Damage, BlackBlood.Range));
			OnUseFinish();
		}
    }
}