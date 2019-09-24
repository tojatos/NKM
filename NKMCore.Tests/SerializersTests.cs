using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Templates;
using Xunit;

namespace NKMCore.Tests
{
    public class SerializersTests
    {
        [Theory]
        [InlineData(GameType.Local)]
        [InlineData(GameType.Multiplayer)]
        [InlineData(GameType.Replay)]
        [InlineData(GameType.Undefined)]
        public void GameTypeSerializeAndDeserialize(GameType type)
        {
            Assert.Equal(type, type.Serialize().DeserializeGameType());
        }

        [Theory]
        [InlineData(PickType.Blind)]
        [InlineData(PickType.Draft)]
        [InlineData(PickType.AllRandom)]
        public void PickTypeSerializeAndDeserialize(PickType type)
        {
            Assert.Equal(type, type.Serialize().DeserializePickType());
        }
        [Fact]
        public void GamePreparerDependenciesSerializeAndDeserialize()
        {
          var deps = new GamePreparerDependencies
          {
              NumberOfPlayers = 2,
              PlayerNames = new List<string> {
                "Ryszard",
                "Maciej",
              },
              NumberOfCharactersPerPlayer = 4,
              BansEnabled = true,
              NumberOfBans = 2,
              PickType = PickType.Draft,
              GameType = GameType.Local,
          };
          GamePreparerDependencies newDeps = deps.Serialize().DeserializeGamePreparerDependencies();
          Assert.Equal(deps.NumberOfPlayers, newDeps.NumberOfPlayers);
          Assert.Equal(deps.PlayerNames, newDeps.PlayerNames);
          Assert.Equal(deps.NumberOfCharactersPerPlayer, newDeps.NumberOfCharactersPerPlayer);
          Assert.Equal(deps.BansEnabled, newDeps.BansEnabled);
          Assert.Equal(deps.NumberOfBans, newDeps.NumberOfBans);
          Assert.Equal(deps.HexMap, newDeps.HexMap);
          Assert.Equal(deps.PickType, newDeps.PickType);
          Assert.Equal(deps.GameType, newDeps.GameType);
        }
    }
}
