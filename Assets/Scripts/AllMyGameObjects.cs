using System.Collections.Generic;
using Extensions;
using NKMObjects.Templates;

public static class AllMyGameObjects
{


	public static List<Character> Characters { get; }

	static AllMyGameObjects()
	{
		Characters = new List<Character>();

		List<string> characterNames = GameData.Conn.GetCharacterNames();
//		characterNames.RemoveAll(s => s == "Yoshino");
		characterNames.ForEach(n=>Characters.Add(new Character(n)));
	}
}
