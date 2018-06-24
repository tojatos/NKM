using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Crona
{
    public class BlackBlood : Ability
    {
	    public const int Damage = 10;
	    public const int Range = 2;
        
		public BlackBlood() : base(AbilityType.Passive, "Black Blood")
		{
			OnAwake += AddBlackBloodEffect;
		}

	    public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range).AddOne(ParentCharacter.ParentCell);
	    public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

	    public override string GetDescription() => $"Przy otrzymaniu obrażeń, {ParentCharacter.Name} zadaje 10 obrażeń wrogom dookoła";
	    private void AddBlackBloodEffect() 
	    {
		    if (ParentCharacter.Effects.Any(e => e.Name == "Black Blood")) return;
		    
		    ParentCharacter.Effects.Add(new Effects.BlackBlood(ParentCharacter, ParentCharacter, -1, Damage, Range));
		    
	    }
    }
}