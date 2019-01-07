using System.Collections.Generic;
using Hex;
using UI;

public class GameOptions
{
	public List<GamePlayer> Players { get; set; }
	public HexMapScriptable MapScriptable { get; set; }
	public UIManager UIManager { get; set; }
	public GameType Type { get; set; }
	public string LogFilePath { get; set; } //optional
	public GameLog GameLog { get; set; } //optional
}

public enum GameType
{
	Local,
	Replay
}

