using System;
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
        private readonly Queue<string> _actions;
        public ReplayPreparer(string[] configToParse)
        {
            string serializedPreparerDependencies = string.Join("\n", configToParse
                                                    .SkipWhile(l => l != "# START: Logging game preparer dependencies").Skip(1)
                                                    .TakeWhile(l => l != "# FINISH: Logging game preparer dependencies"));
            _serializedCharacters = string.Join("\n", configToParse
                                                    .SkipWhile(l => l != "# START: Logging characters").Skip(1)
                                                    .TakeWhile(l => l != "# FINISH: Logging characters"));

            if (_serializedCharacters.Length == 0)
                throw new ReplayException("There are no characters in that replay! Aborting");

            _actions = new Queue<string>(configToParse.SkipWhile(l => l != "# FINISH: Logging characters").Skip(1).Select(line => line.Split(new[] {' '}, 2)[1]));

            if (_actions.Count == 0)
                throw new ReplayException("There are no actions in that replay! Aborting");

            _gamePreparerDependencies = serializedPreparerDependencies.DeserializeGamePreparerDependencies();
        }

        public ReplayResults CreateGame(GamePreparerDependencies preparerDependencies)
        {
            var replayResults = new ReplayResults();

            _gamePreparerDependencies.SelectableManager = new SelectableManager();

            var sel = new DefaultSelectable();

            _gamePreparerDependencies.SelectableAction = new SelectableAction(GameType.Local, sel);
            sel.Init(_gamePreparerDependencies.SelectableAction);

            _gamePreparerDependencies.Selectable = sel;
            _gamePreparerDependencies.Logger = new Logger(null);

            var preparer = new GamePreparer(_gamePreparerDependencies);
            replayResults.Game = preparer.CreateGame();
            replayResults.Actions = _actions;

            _serializedCharacters.DeserializeCharactersAndInsertIntoGame(replayResults.Game);

            return replayResults;
        }
    }

    public class ReplayException : Exception
    {
        public ReplayException(string msg) : base(msg) { }
    }

    public class ReplayResults
    {
        public Game Game;
        public Queue<string> Actions;
    }
}