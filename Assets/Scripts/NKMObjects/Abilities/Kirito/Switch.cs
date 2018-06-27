using System.Collections.Generic;
using System.Linq;
using Animations;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Kirito
{
    public class Switch : Ability, IClickable, IUseable
    {
        private const int Range = 7;

	    public Switch() : base(AbilityType.Normal, "Switch", 3)
	    {
		    OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
	    }

	    public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
        public override List<HexCell> GetTargetsInRange()
	    {
		    List<HexCell> targetCandidates =  GetRangeCells().WhereOnlyFriendsOf(Owner);
            List<Character> enemiesOnMap = HexMapDrawer.Instance.Cells.WhereOnlyEnemiesOf(Owner).GetCharacters();
	        List<HexCell> enemyAttackRanges = enemiesOnMap.SelectMany(enemy => enemy.GetBasicAttackCells()).ToList();
		    return enemyAttackRanges.Contains(ParentCharacter.ParentCell) ? targetCandidates : targetCandidates.Intersect(enemyAttackRanges).ToList();
	    }

	    public override string GetDescription() =>
$@"{ParentCharacter.Name} zamienia się miejscami z wybranym sojusznikiem,
jeśli któryś z nich znajduje się w zasięgu podstawowego ataku wrogiej postaci.
{ParentCharacter.Name} może użyć podstawowego ataku lub super umiejętności po użyciu tej umiejętności.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());
	    public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

	    private void Use(Character character)
        {
	        Swap(ParentCharacter, character);
	        ParentCharacter.HasFreeAttack = true;
	        ParentCharacter.HasFreeUltimatumAbilityUse = true; // TODO
	        Finish();
        }
        
	    private static void Swap(Character firstCharacterToSwap, Character secondCharacterToSwap)
	    {
		    HexCell c1 = firstCharacterToSwap.ParentCell;
		    HexCell c2 = secondCharacterToSwap.ParentCell;
		    firstCharacterToSwap.ParentCell = c2;
		    secondCharacterToSwap.ParentCell = c1;
		    c1.CharacterOnCell = secondCharacterToSwap;
		    c2.CharacterOnCell = firstCharacterToSwap;
		    AnimationPlayer.Add(new MoveTo(firstCharacterToSwap.CharacterObject.transform, c2.transform.position, 0.4f));
		    AnimationPlayer.Add(new MoveTo(secondCharacterToSwap.CharacterObject.transform, c1.transform.position, 0.4f));
	    }
    }
}