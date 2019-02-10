using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

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
				HexMap.Swap(_firstCharacterToSwap, _secondCharacterToSwap);
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
    }
}