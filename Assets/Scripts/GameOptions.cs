using System.Collections.Generic;
using Hex;
using UIManagers;

public class GameOptions
{
	public List<GamePlayer> Players { get; set; }
	public HexMap Map { get; set; }
	public UIManager UIManager { get; set; }
}

public enum GameType
{
	Local,
	MultiplayerServer,
	MultiplayerClient
}