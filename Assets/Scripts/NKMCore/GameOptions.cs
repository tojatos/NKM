using System.Collections.Generic;
using System.Data;
using NKMCore.Hex;

namespace NKMCore
{
	public class GameOptions
	{
		public List<GamePlayer> Players { get; set; }
		public HexMap HexMap { get; set; }
		public GameType Type { get; set; }
		public bool PlaceAllCharactersRandomlyAtStart { get; set; }
		public ISelectable Selectable { get; set; }
		public IDbConnection Connection { get; set; }
		public string LogFilePath { get; set; } //optional
		public GameLog GameLog { get; set; } //optional
	}

	public enum GameType
	{
		Local,
		Replay
	}
}