using System.Collections.Generic;
using System.Linq;
using NKMObjects.Templates;

namespace Hex
{
    public class HexMap
    {
	    public List<HexCell> Cells;
		public readonly List<HexCell.TileType> SpawnPoints;
	    private readonly Dictionary<Character, HexCell> _charactersOnCells = new Dictionary<Character, HexCell>();

	    /// <summary>
	    /// Moves or places character on a cell
	    /// </summary>
	    public void Move(Character character, HexCell hexCell)
	    {
		    _charactersOnCells[character] = hexCell;
		    AfterMove?.Invoke(character, hexCell);
	    }
	    public Delegates.CharacterCell AfterMove;

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

	    public List<Character> GetCharacters(HexCell hexCell) =>
		    _charactersOnCells.Where(pair => pair.Value == hexCell).Select(pair => pair.Key).ToList();
	    
		public HexMap (List<HexCell> cells, List<HexCell.TileType> spawnPoints)
		{
			Cells = cells;
			SpawnPoints = spawnPoints;
		}
    }
}