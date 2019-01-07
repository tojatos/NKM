using System.Collections.Generic;
using System.Linq;
using NKMObjects.Templates;

namespace Hex
{
    public class HexMap
    {
	    public List<HexCell> Cells;
		public readonly List<HexTileType> SpawnPoints;
	    private readonly Dictionary<Character, HexCell> _charactersOnCells = new Dictionary<Character, HexCell>();
	    
	    /// <summary>
	    /// Moves or places character on a cell
	    /// </summary>
	    public void Move(Character character, HexCell hexCell) => _charactersOnCells[character] = hexCell;
	    public void RemoveFromMap(Character character) => _charactersOnCells[character] = null;
	    
	    public HexCell GetCell(Character character) => _charactersOnCells[character];
	    public List<Character> GetCharacters(HexCell hexCell) =>
		    _charactersOnCells.Where(pair => pair.Value == hexCell).Select(pair => pair.Key).ToList();
	    
		public HexMap (List<HexCell> cells, List<HexTileType> spawnPoints)
		{
			Cells = cells;
			SpawnPoints = spawnPoints;
		}
    }
}