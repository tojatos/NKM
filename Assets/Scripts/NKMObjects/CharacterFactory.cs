using System.Collections.Generic;
using Extensions;
using NKMObjects.Templates;

namespace NKMObjects
{
    public static class CharacterFactory
    {
		public static Character Create(Game game, string name) => 
			CreateCharacterFromDatabase(game, name, NKMID.GetNext("Character"));

		private static Character CreateCharacterFromDatabase(Game game, string name, uint id)
		{
			SqliteRow characterData = GameData.Conn.GetCharacterData(name);
			Character.Properties properties = GetCharacterProperties(characterData);
			List<Ability> abilities = AbilityFactory.CreateAndInitiateAbilitiesFromDatabase(name);
			
			return new Character(game, name, id, properties, abilities);
		}

		private static Character.Properties GetCharacterProperties(SqliteRow characterData)
		{
			return new Character.Properties
			{
				AttackPoints =
					new Stat(StatType.AttackPoints, int.Parse(characterData.GetValue("AttackPoints"))),
				HealthPoints =
					new Stat(StatType.HealthPoints, int.Parse(characterData.GetValue("HealthPoints"))),
				BasicAttackRange = new Stat(StatType.BasicAttackRange,
					int.Parse(characterData.GetValue("BasicAttackRange"))),
				Speed = new Stat(StatType.Speed, int.Parse(characterData.GetValue("Speed"))),
				PhysicalDefense = new Stat(StatType.PhysicalDefense,
					int.Parse(characterData.GetValue("PhysicalDefense"))),
				MagicalDefense = new Stat(StatType.MagicalDefense,
					int.Parse(characterData.GetValue("MagicalDefense"))),
				Shield = new Stat(StatType.Shield, 0),

				Type = characterData.GetValue("FightType").ToFightType(),
			};
		}
    }
}