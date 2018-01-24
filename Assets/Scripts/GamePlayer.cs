using System;
using System.Collections.Generic;
using MyGameObjects.MyGameObject_templates;
public class GamePlayer
{
	public string Name { get; set; }
	public bool HasSelectedCharacters { get; set; }
	public List<Character> Characters { get;} = new List<Character>();
	//public List<Character> StartingCharacters { get; private set; }
	public bool HasFinishedSelecting => HasSelectedCharacters;

}