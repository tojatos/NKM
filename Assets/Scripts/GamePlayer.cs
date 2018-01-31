using System;
using System.Collections.Generic;
using System.Linq;
using MyGameObjects.MyGameObject_templates;
public class GamePlayer
{
	public string Name { get; set; }
	public bool HasSelectedCharacters { get; set; }
	public List<Character> Characters { get; set; } = new List<Character>();
	//public List<Character> StartingCharacters { get; private set; }
	public bool HasFinishedSelecting => HasSelectedCharacters;

	public void AddCharacters(List<Character> characters)
	{
		characters.ForEach(c=> c.Owner = this);
		Characters.AddRange(characters);
	}
	public void AddCharacters(IEnumerable<string> classNames)
	{
		var characters = Spawner.Create("Characters", classNames).Cast<Character>().ToList();
		AddCharacters(characters);
	}

}