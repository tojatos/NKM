using System.Collections.Generic;
using System.Linq;
using NKMObjects.Templates;

public class GamePlayer
{
	public string Name { get; set; }
	public bool HasSelectedCharacters { get; set; }
	public List<Character> Characters { get; set; } = new List<Character>();
	public bool HasFinishedSelecting => HasSelectedCharacters;
	public bool IsEliminated => Characters.All(c => !c.IsAlive);

	public void AddCharacter(Character character)
	{
		character.Owner = this;
		Characters.Add(character);
	}

	public void AddCharacter(string characterName) => AddCharacter(new Character(characterName));

	private void AddCharacters(List<Character> characters)
	{
		characters.ForEach(c=> c.Owner = this);
		Characters.AddRange(characters);
	}

	public void AddCharacters(IEnumerable<string> characterNames)
	{
		List<Character> characters = new List<Character>();
		characterNames.ToList().ForEach(n => characters.Add(new Character(n)));
		AddCharacters(characters);

	}
}