using System.Collections.Generic;
using System.Linq;
using NKMCore.Templates;

namespace NKMCore.Hex
{
    public class HexMap
    {
	    public readonly List<HexCell> Cells;
		public readonly List<HexCell.TileType> SpawnPoints;
	    private readonly Dictionary<Character, HexCell> _charactersOnCells = new Dictionary<Character, HexCell>();

	    public void Place(Character character, HexCell cell)
	    {
		    _charactersOnCells[character] = cell;
		    AfterCharacterPlace?.Invoke(character, cell);
	    }
	    public void Move(Character character, HexCell cell)
	    {
		    _charactersOnCells[character] = cell;
		    AfterMove?.Invoke(character, cell);
	    }

	    public void Swap(Character firstCharacterToSwap, Character secondCharacterToSwap)
	    {
		    HexCell c1 = firstCharacterToSwap.ParentCell;
		    HexCell c2 = secondCharacterToSwap.ParentCell;
		    Move(firstCharacterToSwap, c2);
		    Move(secondCharacterToSwap, c1);
	    }
		    
	    public Delegates.CharacterCell AfterMove;
	    public Delegates.CharacterCell AfterCharacterPlace;
	    public Delegates.HexCellEffectD AfterCellEffectCreate;
	    public Delegates.HexCellEffectD AfterCellEffectRemove;
	    public void InvokeAfterCellEffectCreate(HexCellEffect e) => AfterCellEffectCreate?.Invoke(e);
	    public void InvokeAfterCellEffectRemove(HexCellEffect e) => AfterCellEffectRemove?.Invoke(e);

	    /// <summary>
	    /// Removes character from map or does nothing
	    /// </summary>
	    public void RemoveFromMap(Character character) => _charactersOnCells.Remove(character);
	    
	    public HexCell GetCell(Character character)
	    {
		    HexCell value;
		    _charactersOnCells.TryGetValue(character, out value);
		    return value;
	    }

	    public List<Character> GetCharacters(HexCell cell) =>
		    _charactersOnCells.Where(pair => pair.Value == cell).Select(pair => pair.Key).ToList();
	    
		public HexMap (List<HexCell> cells, List<HexCell.TileType> spawnPoints)
		{
			Cells = cells;
			SpawnPoints = spawnPoints;
		}
    }
}