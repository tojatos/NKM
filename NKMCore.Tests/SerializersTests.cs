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

        [Fact]
        public void CharacterSerializeAndDeserialize()
        {
            const string name = "Steve";
            var character = new Character(new Character.Properties{Name = name});

            string serializedCharacter = character.Serialize();
            Character deserializedCharacter = serializedCharacter.DeserializeCharacter();

            Assert.Equal(name, serializedCharacter);
            Assert.Equal(character.Name, deserializedCharacter.Name);
        }
    }
}