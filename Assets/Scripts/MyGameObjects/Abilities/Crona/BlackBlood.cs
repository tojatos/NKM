using System.Collections.Generic;
using System.Linq;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Crona
{
    public class BlackBlood : Ability
    {
        
		public BlackBlood()
		{
			Name = "Black Blood";
			Type = AbilityType.Passive;
		}

	    public override List<HexCell> GetRangeCells()
	    {
		    List<HexCell> rangeCells = ParentCharacter.ParentCell.GetNeighbors(1);
		    rangeCells.Add(ParentCharacter.ParentCell);
		    return rangeCells;
	    }
	 

	    public override string GetDescription() => $"Przy otrzymaniu obrażeń, {ParentCharacter.Name} zadaje 10 obrażeń wrogom dookoła";

	    
	    public override void Awake() => TryToAddBlackBloodEffect();

	    private void TryToAddBlackBloodEffect() 
	    {
		    if (ParentCharacter.Effects.Any(e => e.Name == "Black Blood")) return;
		    
		    ParentCharacter.Effects.Add(new Effects.BlackBlood(ParentCharacter, ParentCharacter));
		    
	    }
    }
}