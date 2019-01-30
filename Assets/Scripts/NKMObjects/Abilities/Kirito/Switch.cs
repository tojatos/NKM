using System.Collections.Generic;
using System.Linq;
using Animations;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Kirito
{
    public class Switch : Ability, IClickable, IUseableCharacter
    {
        private const int Range = 7;

	    public Switch(Game game) : base(game, AbilityType.Normal, "Switch", 3)
	    {
		    OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		    CanUseOnGround = false;
	    }

	    public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange()
	    {
		    List<HexCell> targetCandidates =  GetRangeCells().WhereFriendsOf(Owner);
            List<Character> enemiesOnMap = HexMap.Cells.WhereEnemiesOf(Owner).GetCharacters();
	        List<HexCell> enemyAttackRanges = enemiesOnMap.SelectMany(enemy => enemy.GetBasicAttackCells()).ToList();
		    return enemyAttackRanges.Contains(ParentCharacter.ParentCell) ? targetCandidates : targetCandidates.Intersect(enemyAttackRanges).ToList();
	    }

	    public override string GetDescription() =>
$@"{ParentCharacter.Name} zamienia się miejscami z wybranym sojusznikiem,
jeśli któryś z nich znajduje się w zasięgu podstawowego ataku wrogiej postaci.
{ParentCharacter.Name} może użyć podstawowego ataku albo super umiejętności zaraz po użyciu tej umiejętności.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

	    public void Use(Character character)
        {
			ParentCharacter.TryToTakeTurn();
	        Swap(ParentCharacter, character);
	        ParentCharacter.HasFreeAttackUntilEndOfTheTurn = true;
	        ParentCharacter.HasFreeUltimatumAbilityUseUntilEndOfTheTurn = true; // TODO
	        Delegates.CharacterDamage onAttack = null;
	        onAttack = (c, d) =>
	        {
		        ParentCharacter.HasFreeUltimatumAbilityUseUntilEndOfTheTurn = false;
		        ParentCharacter.AfterBasicAttack -= onAttack;
	        };
	        ParentCharacter.AfterBasicAttack += onAttack;
	        Delegates.AbilityD onAbilityUse = null;
	        onAbilityUse += ability =>
	        {
		        ParentCharacter.HasFreeAttackUntilEndOfTheTurn = false;
		        ParentCharacter.AfterAbilityUse -= onAbilityUse;
	        };
	        ParentCharacter.AfterAbilityUse += onAbilityUse;
	        Delegates.CellList onMove = null;
	        onMove += cellList =>
	        {
		        ParentCharacter.HasFreeUltimatumAbilityUseUntilEndOfTheTurn = false;
		        ParentCharacter.HasFreeAttackUntilEndOfTheTurn = false;
		        ParentCharacter.AfterBasicMove -= onMove;
	        };
	        ParentCharacter.AfterBasicMove += onMove;
		       
	        Active.Turn.TurnFinished += character1 =>
	        {
		        if (character1 != ParentCharacter) return;
		        ParentCharacter.AfterBasicAttack -= onAttack;
		        ParentCharacter.AfterAbilityUse -= onAbilityUse;
		        ParentCharacter.AfterBasicMove -= onMove;
	        };
	        Finish();
        }
        
	    private void Swap(Character firstCharacterToSwap, Character secondCharacterToSwap)
	    {
		    HexCell c1 = firstCharacterToSwap.ParentCell;
		    HexCell c2 = secondCharacterToSwap.ParentCell;
		    HexMap.Move(firstCharacterToSwap, c2);
		    HexMap.Move(secondCharacterToSwap, c1);
		    AnimationPlayer.Add(new MoveTo(Game.HexMapDrawer.GetCharacterObject(firstCharacterToSwap).transform, Active.SelectDrawnCell(c2).transform.position, 0.4f));
		    AnimationPlayer.Add(new MoveTo(Game.HexMapDrawer.GetCharacterObject(secondCharacterToSwap).transform, Active.SelectDrawnCell(c1).transform.position, 0.4f));
	    }
    }
}