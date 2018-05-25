using System.Collections.Generic;
using Helpers;
using MyGameObjects.MyGameObject_templates;

public static class AllMyGameObjects
{


	public static List<Character> Characters { get; }

	static AllMyGameObjects()
	{
		Characters = new List<Character>();

		List<string> characterNames = GameData.Conn.GetCharacterNames();
		characterNames.ForEach(n=>Characters.Add(new Character(n)));
	}
}
