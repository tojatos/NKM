using System.Collections.Generic;
using System.Linq;
using NKMObjects.Templates;

namespace Hex
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
	    public Delegates.CharacterCell AfterMove;
	    public Delegates.CharacterCell AfterCharacterPlace;

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