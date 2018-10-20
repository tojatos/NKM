using NKMObjects;
using NKMObjects.Templates;
using Xunit;

namespace Assembly_CSharp.Tests
{
    public class CharacterTests
    {
        [Fact]
        public void CharacterCreatedSuccessfully()
        {
//            Character character = new Character("Aqua");
            CharacterProperties properties = new CharacterProperties
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
            Character character = CharacterFactory.CreateNonGame("My character", properties);
            Assert.Equal(true, character.IsAlive);
        }
        
        
        
    }
}