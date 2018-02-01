using System.Collections.Generic;
using Hex;
using Multiplayer.Network;
using UIManagers;

public class GameOptions
{
	public GameType GameType { get; set; }
	public List<GamePlayer> Players { get; set; }
	public HexMap Map { get; set; }
	public UIManager UIManager { get; set; }
	public Client Client { get; set; }
	public Server Server { get; set; }
}

public enum GameType
{
	Local,
	MultiplayerServer,
	MultiplayerClient
}