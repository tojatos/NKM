using System.Collections.Generic;
using NKMCore;
using NKMCore.Templates;
using Xunit;

namespace Assembly_CSharp.Tests
{
    public class CharacterTests
    {
        [Fact]
        public void CharacterCreatedSuccessfully()
        {
            var properties = new Character.Properties
            {
                HealthPoints      =  new  Stat(StatType.HealthPoints,      60),
                AttackPoints      =  new  Stat(StatType.AttackPoints,      20),
                BasicAttackRange  =  new  Stat(StatType.BasicAttackRange,  5),
                Speed             =  new  Stat(StatType.Speed,             7),
                PhysicalDefense   =  new  Stat(StatType.PhysicalDefense,   20),
                MagicalDefense    =  new  Stat(StatType.MagicalDefense,    15),
                Shield            =  new  Stat(StatType.Shield,            0),
                Type              =  FightType.Melee,                                   
            };
            List<Ability> abilities = new List<Ability>();
            Game game = new Game();
            Character character = new Character(game, "My character", 1, properties, abilities);
            Assert.Equal(true, character.IsAlive);
        }
        
        
        
    }
}