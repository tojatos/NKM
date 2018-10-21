using System.Collections.Generic;
using System.Linq;
using NKMObjects;
using NKMObjects.Templates;

public class GamePlayer
{
	public string Name { get; set; }
	public bool HasSelectedCharacters { get; set; }
	public List<NKMCharacter> Characters { get; set; } = new List<NKMCharacter>();
	public bool HasFinishedSelecting => HasSelectedCharacters;
	public bool IsEliminated => Characters.All(c => !c.IsAlive);

	public void AddCharacter(NKMCharacter character)
	{
		character.Owner = this;
		Characters.Add(character);
	}

//	public void AddCharacter(string characterName) => AddCharacter(new Character(characterName));
	public void AddCharacter(string characterName) => AddCharacter(CharacterFactory.Create(characterName));

	private void AddCharacters(List<NKMCharacter> characters)
	{
		characters.ForEach(c=> c.Owner = this);
		Characters.AddRange(characters);
	}

	public void AddCharacters(IEnumerable<string> characterNames)
	{
		List<NKMCharacter> characters = new List<NKMCharacter>();
//		characterNames.ToList().ForEach(n => characters.Add(new Character(n)));
		characterNames.ToList().ForEach(n => characters.Add(CharacterFactory.Create(n)));
		AddCharacters(characters);

	}
}