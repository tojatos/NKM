using System.Collections.Generic;
using Hex;
using UI;

public class GameOptions
{
	public List<GamePlayer> Players { get; set; }
	public HexMap Map { get; set; }
	public UIManager UIManager { get; set; }
	public string LogFilePath { get; set; }
}

