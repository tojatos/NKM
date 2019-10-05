using System.Collections.Generic;
using System.Linq;
using NKMCore;
using NKMCore.Extensions;

namespace Unity.Managers
{
    public class ReplayPreparer
    {
        private readonly GamePreparerDependencies _gamePreparerDependencies;
        private readonly string _serializedCharacters;
        private readonly List<string> _actions;
        public ReplayPreparer(string[] configToParse)
        {
            string serializedPreparerDependencies = string.Join("\n", configToParse
                                                    .SkipWhile(l => l != "# START: Logging game preparer dependencies").Skip(1)
                                                    .TakeWhile(l => l != "# FINISH: Logging game preparer dependencies"));
            _serializedCharacters = string.Join("\n", configToParse
                                                    .SkipWhile(l => l != "# START: Logging characters").Skip(1)
                                                    .TakeWhile(l => l != "# FINISH: Logging characters"));

            _actions = configToParse.SkipWhile(l => l != "# FINISH: Logging characters").Skip(1).ToList();
            _gamePreparerDependencies = serializedPreparerDependencies.DeserializeGamePreparerDependencies();
        }

        public ReplayResults CreateGame(GamePreparerDependencies preparerDependencies)
        {
            var replayResults = new ReplayResults();

            _gamePreparerDependencies.Selectable = preparerDependencies.Selectable;
            _gamePreparerDependencies.SelectableAction = preparerDependencies.SelectableAction;
            _gamePreparerDependencies.SelectableManager = preparerDependencies.SelectableManager;
            _gamePreparerDependencies.Logger = new Logger(null);

            var preparer = new GamePreparer(_gamePreparerDependencies);
            replayResults.Game = preparer.CreateGame();
            replayResults.Actions = _actions;

            _serializedCharacters.DeserializeCharactersAndInsertIntoGame(replayResults.Game);

            return replayResults;
        }
    }

    public class ReplayResults
    {
        public Game Game;
        public List<string> Actions;
    }
}