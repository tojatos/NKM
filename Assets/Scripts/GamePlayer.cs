using System.Collections.Generic;
using System.Linq;
using NKMObjects.Templates;

public class GamePlayer
{
	public string Name { get; set; }
	public bool HasSelectedCharacters { get; set; }
	public List<Character> Characters { get; set; } = new List<Character>();
	public bool HasFinishedSelecting => HasSelectedCharacters;

	private void AddCharacters(List<Character> characters)
	{
		characters.ForEach(c=> c.Owner = this);
		Characters.AddRange(characters);
	}

	public void AddCharacters(IEnumerable<string> classNames)
	{
		List<Character> characters = new List<Character>();
		classNames.ToList().ForEach(n => characters.Add(new Character(n)));
		AddCharacters(characters);

	}
}