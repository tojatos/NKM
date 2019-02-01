using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Crona
{
    public class BlackBlood : Ability
    {
	    public const int Damage = 10;
	    public const int Range = 2;
        
		public BlackBlood(Game game) : base(game, AbilityType.Passive, "Black Blood")
		{
			OnAwake += AddBlackBloodEffect;
		}

	    public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range).AddOne(ParentCharacter.ParentCell);
	    public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

	    public override string GetDescription() => $"Przy otrzymaniu obrażeń, {ParentCharacter.Name} zadaje {Damage} obrażeń magicznych wrogom dookoła";
	    private void AddBlackBloodEffect() 
	    {
		    if (ParentCharacter.Effects.Any(e => e.Name == "Black Blood")) return;
		    
		    ParentCharacter.Effects.Add(new Effects.BlackBlood(Game, ParentCharacter, ParentCharacter, -1, Damage, Range));
		    
	    }
    }
}