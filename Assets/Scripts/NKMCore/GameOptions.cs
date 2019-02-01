using System.Collections.Generic;
using Unity.Hex;

namespace NKMCore
{
	public class GameOptions
	{
		public List<GamePlayer> Players { get; set; }
		public HexMapScriptable MapScriptable { get; set; }
		public GameType Type { get; set; }
		public bool PlaceAllCharactersRandomlyAtStart { get; set; }
		public string LogFilePath { get; set; } //optional
		public GameLog GameLog { get; set; } //optional
	
	}

	public enum GameType
	{
		Local,
		Replay
	}
}