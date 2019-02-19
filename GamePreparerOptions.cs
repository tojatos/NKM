using System.Data;
using NKMCore.Hex;

namespace NKMCore
{
    public class GamePreparerOptions
    {
        public int NumberOfPlayers;
        public int NumberOfCharactersPerPlayer;
        public bool BansEnabled;
        public int NumberOfBans;
        public HexMap HexMap;
        public PickType PickType;
		public GameType GameType;
        public ISelectable Selectable;
		public IDbConnection Connection;
		public string LogFilePath; //optional
    }
}