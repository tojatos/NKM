using System;
using System.Collections.Generic;
using System.Linq;
using Animations;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Bezimienni
{
    public class Castling : Ability
    {
	    private Character _firstCharacterToSwap;
	    private Character _secondCharacterToSwap;
        public Castling()
        {
            Name = "Castling";
            Cooldown = 6;
            CurrentCooldown = 0;
            Type = AbilityType.Ultimatum;
        }
        public override string GetDescription()
        {
	        return "Bezimienni zamieniają pozycjami na mapie 2 jednostki.";
        }
        protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			int characterCount = GetRangeCells().GetCharacters().Count;
			if (characterCount < 2)	throw new Exception("Nie ma dwóch postaci w zasięgu");
		}
	    public override List<HexCell> GetRangeCells() => new List<HexCell>(HexMapDrawer.Instance.Cells);
        protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonCharacters();
			if (_firstCharacterToSwap != null) cellRange.RemoveAll(c => c.CharacterOnCell == _firstCharacterToSwap);
			
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
			OnFailedUseFinish();
		}
		public override void Use(Character character)
		{
			if (_firstCharacterToSwap == null)
			{
				_firstCharacterToSwap = character;
				Use();
			}
			else
			{
				_secondCharacterToSwap = character;
				Swap();
				Reset();
				OnUseFinish();
			}
		}


	    private void Reset()
	    {
		  _firstCharacterToSwap = null;
		  _secondCharacterToSwap = null;   
	    }
	    
	    public override void Cancel()
	    {
		    base.Cancel();
		    Reset();
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
    }
}