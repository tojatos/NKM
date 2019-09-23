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
    }
}