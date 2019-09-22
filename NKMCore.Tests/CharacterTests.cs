using System.Collections.Generic;
using NKMCore.Templates;
using Xunit;

namespace NKMCore.Tests
{
    public class CharacterTests
    {
        [Fact]
        public void CharacterCreatedSuccessfully()
        {
            var properties = new Character.Properties
            {
                Game = null,
                Id = 5,
                Name = "Foo",
                HealthPoints      =  new  Stat(StatType.HealthPoints,      60),
                AttackPoints      =  new  Stat(StatType.AttackPoints,      20),
                BasicAttackRange  =  new  Stat(StatType.BasicAttackRange,  5),
                Speed             =  new  Stat(StatType.Speed,             7),
                PhysicalDefense   =  new  Stat(StatType.PhysicalDefense,   20),
                MagicalDefense    =  new  Stat(StatType.MagicalDefense,    15),
                Shield            =  new  Stat(StatType.Shield,            0),
                Type              =  FightType.Melee,
                Abilities = new List<Ability>(),
            };
            var character = new Character(properties);
            Assert.Null(character.Game);
            Assert.Equal(5, character.ID);
            Assert.Equal("Foo", character.Name);
            Assert.Equal(FightType.Melee, character.Type);

            Assert.Equal(60, character.HealthPoints.BaseValue);
            Assert.Equal(60, character.HealthPoints.Value);
            Assert.Equal(20, character.AttackPoints.Value);
            Assert.Equal(5, character.BasicAttackRange.Value);
            Assert.Equal(7, character.Speed.BaseValue);
            Assert.Equal(20, character.PhysicalDefense.BaseValue);
            Assert.Equal(15, character.MagicalDefense.BaseValue);
            Assert.Equal(0, character.Shield.Value);

            Assert.True(character.IsAlive);
        }
    }
}