using System.Collections.Generic;
using System.Linq;
using NKMCore.Templates;

namespace NKMCore
{
	public class GamePlayer
	{
		public readonly Game Game;
		public GamePlayer(Game game)
		{
			Game = game;
		}
		public string Name { get; set; }
		public bool HasSelectedCharacters { get; set; }
		public List<Character> Characters { get; set; } = new List<Character>();
		public bool HasFinishedSelecting => HasSelectedCharacters;
		public bool IsEliminated => Characters.All(c => !c.IsAlive);

		public void AddCharacter(Character character)
		{
			Characters.Add(character);
		}

		public void AddCharacter(Game game, string characterName) => AddCharacter(CharacterFactory.Create(game, characterName));
		public void AddCharacters(string[] characterNames)
		{
			Characters.AddRange(characterNames.Select(c => CharacterFactory.Create(Game, c)));
		}
	}
}