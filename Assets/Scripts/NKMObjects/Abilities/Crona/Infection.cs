using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Crona
{
    public class Infection : Ability, IClickable, IUseableCharacter
    {
        private const int Range = 6;
        private const int EffectCooldown = 3;

	    public Infection(Game game) : base(game, AbilityType.Ultimatum, "Infection", 5)
	    {
		    OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
	    }
	    
        public override string GetDescription() => 
$@"{ParentCharacter.Name} infekuje cel Czarną Krwią (nakłada efekt Black Blood) na {EffectCooldown} tury.
Zainfekowany wróg również otrzymuje obrażenia przy zdetonowaniu Black Blood.
Zasięg: {Range}	Czas odnowienia: {Cooldown}";
	    
	    public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
	    public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

	    public void Click() => Active.Prepare(this, GetTargetsInRange());
	    public void Use(Character character)
		{
            ParentCharacter.TryToTakeTurn();
			character.Effects.Add(new Effects.BlackBlood(Game, ParentCharacter, character, EffectCooldown, BlackBlood.Damage, BlackBlood.Range));
			Finish();
		}
    }
}