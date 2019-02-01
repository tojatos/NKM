using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity;
using Unity.Animations;
using Unity.Hex;

namespace NKMCore.Abilities.Bezimienni
{
    public class Castling : Ability, IClickable, IUseableCharacter
    {
	    private Character _firstCharacterToSwap;
	    private Character _secondCharacterToSwap;
        public Castling(Game game) : base(game, AbilityType.Ultimatum, "Castling", 6)
        {
	        OnAwake += () => Validator.ToCheck.Add(() => GetRangeCells().GetCharacters().Count >= 2);
	        AfterUseFinish += Cleanup;
        }
	    public override List<HexCell> GetRangeCells() => new List<HexCell>(HexMap.Cells);
	    public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereCharacters();
        public override string GetDescription() => "Bezimienni zamieniają pozycjami na mapie 2 jednostki.";

	    public void Click() => PrepareCharacterSelection();

	    private void PrepareCharacterSelection()
	    {
		    List<HexCell> cellRange = GetTargetsInRange();
		    if (_firstCharacterToSwap != null) cellRange.RemoveAll(c => c.FirstCharacter == _firstCharacterToSwap);
		    Active.Prepare(this, cellRange);
	    }

	    public void Use(Character character)
		{
			if (_firstCharacterToSwap == null)
			{
				_firstCharacterToSwap = character;
				PrepareCharacterSelection();
			}
			else
			{
				_secondCharacterToSwap = character;
                ParentCharacter.TryToTakeTurn();
				Swap();
				Finish();
			}
		}

	    private void Cleanup()
	    {
		  _firstCharacterToSwap = null;
		  _secondCharacterToSwap = null;   
	    }
	    
	    public override void Cancel()
	    {
		    base.Cancel();
		    Cleanup();
	    }

	    private void Swap()
	    {
		    HexCell c1 = _firstCharacterToSwap.ParentCell;
		    HexCell c2 = _secondCharacterToSwap.ParentCell;
		    HexMap.Move(_firstCharacterToSwap, c2);
		    HexMap.Move(_secondCharacterToSwap, c1);
		    
		    AnimationPlayer.Add(new MoveTo(HexMapDrawer.Instance.GetCharacterObject(_firstCharacterToSwap).transform, Active.SelectDrawnCell(c2).transform.position, 1f));
		    AnimationPlayer.Add(new MoveTo(HexMapDrawer.Instance.GetCharacterObject(_secondCharacterToSwap).transform, Active.SelectDrawnCell(c1).transform.position, 1f));
	    }
    }
}