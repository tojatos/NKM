using System.Collections.Generic;
using NKMCore.Abilities.Sinon;
using NKMCore.Hex;
using NKMCore.Templates;
using Xunit;

namespace NKMCore.Tests.Abilities.Sinon
{
    class MockLogger : ILogger
    {
        public void Log(string msg) { }
    }
    public class SnipersSightTests
    {
        private const string TestHexMap = "TestName\n\n0:0;Normal\n\nSpawnPoint1";
        private const GameType _gameType = GameType.Local;
        private readonly DefaultSelectable _sel = new DefaultSelectable();

        [Fact]
        public void BasicAttackSphericityTest()
        {
            var testGameDeps = new GameDependencies
            {
                Players = new List<GamePlayer>
                {
                    new GamePlayer{Name = "Player1"},
                    new GamePlayer{Name = "Player2"},
                },
                HexMap = HexMapSerializer.Deserialize(TestHexMap),
                Type = _gameType,
                PlaceAllCharactersRandomlyAtStart = false,
                Selectable = _sel,
                SelectableManager = new SelectableManager(),
                SelectableAction = new SelectableAction(_gameType, _sel),
                Logger = new MockLogger()
            };
            var testGame = new Game(testGameDeps);
            var properties = new Character.Properties
            {
                Game = testGame,
                Id = 5,
                Name = "Foo",
                HealthPoints      =  new  Stat(StatType.HealthPoints,      60),
                AttackPoints      =  new  Stat(StatType.AttackPoints,      20),
                BasicAttackRange  =  new  Stat(StatType.BasicAttackRange,  5),
                Speed             =  new  Stat(StatType.Speed,             7),
                PhysicalDefense   =  new  Stat(StatType.PhysicalDefense,   20),
                MagicalDefense    =  new  Stat(StatType.MagicalDefense,    15),
                Shield            =  new  Stat(StatType.Shield,            0),
                Type              =  FightType.Ranged,
                Abilities = new List<Ability>{new SnipersSight(testGame)},
            };
            var c = new Character(properties);
            testGame.Players[0].Characters.Add(c);
            // testGame.Start();
            // Assert.Equal(SnipersSight.); //TODO
        }
    }
}