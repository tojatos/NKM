using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Crona
{
    public class Infection : Ability, IClickable, IUseable
    {
        private const int Range = 4;
        private const int EffectCooldown = 3;
        public Infection() : base(AbilityType.Ultimatum, "Infection", 5){}
	    //TODO: Range constraint validation?
        public override string GetDescription() => 
$@"{ParentCharacter.Name} infekuje cel Czarną Krwią (nakłada efekt Black Blood) na {EffectCooldown} tury.
Zainfekowany wróg również otrzymuje obrażenia przy zdetonowaniu Black Blood.
Zasięg: {Range}	Czas odnowienia: {CurrentCooldown}";
	    
	    public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
	    public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);
	    public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

	    public void Click() => Active.Prepare(this, GetTargetsInRange());
	    private void Use(Character character)
		{
			character.Effects.Add(new Effects.BlackBlood(ParentCharacter, character, EffectCooldown, BlackBlood.Damage, BlackBlood.Range));
			Finish();
		}
    }
}