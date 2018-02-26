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
	public void AddCharacters(Dictionary<string, Guid> namesWithGuids)
	{
		List<Character> characters = new List<Character>();
		foreach (var nameWithGuid in namesWithGuids)
		{
			var name = nameWithGuid.Key;
			var Guid = nameWithGuid.Value;

			var character = new Character(name, Guid);//Spawner.Create("Characters", name) as Character;//TODO new character system
//			character.Guid = Guid;
			characters.Add(character);

		}
		AddCharacters(characters);
	}

	public void AddCharacters(IEnumerable<string> classNames)
	{
		var characters = new List<Character>();
		classNames.ToList().ForEach(n => characters.Add(new Character(n)));
		AddCharacters(characters);

	}
}