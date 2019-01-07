using System.Collections.Generic;
using System.Linq;
using NKMObjects.Templates;

namespace Hex
{
    public class HexMap
    {
	    public readonly List<BetterHexCell> Cells;
	    private readonly Dictionary<NKMCharacter, BetterHexCell> _charactersOnCells = new Dictionary<NKMCharacter, BetterHexCell>();
	    
	    /// <summary>
	    /// Moves or places character on a cell
	    /// </summary>
	    public void Move(NKMCharacter character, BetterHexCell hexCell) => _charactersOnCells[character] = hexCell;
	    public void RemoveFromMap(NKMCharacter character) => _charactersOnCells[character] = null;
	    
	    public BetterHexCell GetCell(NKMCharacter character) => _charactersOnCells[character];
	    public List<NKMCharacter> GetCharacters(BetterHexCell hexCell) =>
		    _charactersOnCells.Where(pair => pair.Value == hexCell).Select(pair => pair.Key).ToList();
	    
		public HexMap (List<BetterHexCell> cells)
		{
			Cells = cells;
		}
    }
}