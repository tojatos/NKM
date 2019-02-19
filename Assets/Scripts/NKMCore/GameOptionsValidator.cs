using System.Linq;

namespace NKMCore
{
    public class GameOptionsValidator
    {
        private readonly GameOptions _gameOptions;
        private readonly int _numberOfPlayers;
        private readonly int _numberOfCharacters;
        
        public GameOptionsValidator(GameOptions options, int numberOfPlayers, int numberOfCharacters)
        {
            _gameOptions = options;
            _numberOfPlayers = numberOfPlayers;
            _numberOfCharacters = numberOfCharacters;
        }
        
        private bool NumberOfPlayersCorrect => 
            _gameOptions.Players.Count == _numberOfPlayers;
        private bool NumberOfCharactersPerPlayerCorrect =>
            _gameOptions.Players.All(p => p.Characters.Count == _numberOfCharacters);
        private bool IsHexMapSet =>
            _gameOptions.HexMap != null;
        private bool IsGameTypeSet =>
            _gameOptions.Type != GameType.Undefined;
        private bool IsSelectableSet => 
            _gameOptions.Selectable != null;
        private bool IsConnectionSet =>
            _gameOptions.Connection != null;

        public bool AreOptionsValid =>
            NumberOfPlayersCorrect &&
            NumberOfCharactersPerPlayerCorrect &&
            IsHexMapSet &&
            IsGameTypeSet &&
            IsSelectableSet &&
            IsConnectionSet;
        
    }
}