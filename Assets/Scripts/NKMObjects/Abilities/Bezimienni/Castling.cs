using System.Collections.Generic;
using Animations;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Bezimienni
{
    public class Castling : Ability, IClickable, IUseable
    {
	    private NKMCharacter _firstCharacterToSwap;
	    private NKMCharacter _secondCharacterToSwap;
        public Castling() : base(AbilityType.Ultimatum, "Castling", 6)
        {
	        OnAwake += () => Validator.ToCheck.Add(() => GetRangeCells().GetCharacters().Count >= 2);
	        AfterUseFinish += Cleanup;
        }
	    public override List<HexCell> GetRangeCells() => new List<HexCell>(HexMapDrawer.Instance.Cells);
	    public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyCharacters();
        public override string GetDescription() => "Bezimienni zamieniają pozycjami na mapie 2 jednostki.";

	    public void Click() => PrepareCharacterSelection();

	    private void PrepareCharacterSelection()
	    {
		    List<HexCell> cellRange = GetTargetsInRange();
		    if (_firstCharacterToSwap != null) cellRange.RemoveAll(c => c.CharacterOnCell == _firstCharacterToSwap);
		    Active.Prepare(this, cellRange);
	    }

	    private void Use(NKMCharacter character)
		{
			if (_firstCharacterToSwap == null)
			{
				_firstCharacterToSwap = character;
				PrepareCharacterSelection();
			}
			else
			{
				_secondCharacterToSwap = character;
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
		    _firstCharacterToSwap.ParentCell = c2;
		    _secondCharacterToSwap.ParentCell = c1;
		    c1.CharacterOnCell = _secondCharacterToSwap;
		    c2.CharacterOnCell = _firstCharacterToSwap;
		    AnimationPlayer.Add(new MoveTo(_firstCharacterToSwap.CharacterObject.transform, c2.transform.position, 1f));
		    AnimationPlayer.Add(new MoveTo(_secondCharacterToSwap.CharacterObject.transform, c1.transform.position, 1f));
	    }

	    public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);
    }
}