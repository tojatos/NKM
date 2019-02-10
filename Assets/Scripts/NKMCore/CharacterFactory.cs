using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore
{
    public static class CharacterFactory
    {
		public static Character Create(Game game, string name) => 
			CreateCharacterFromDatabase(game, name, NKMID.GetNext("Character"));

		private static Character CreateCharacterFromDatabase(Game game, string name, uint id)
		{
			SqliteRow characterData = GameData.Conn.GetCharacterData(name);
			Character.Properties properties = GetCharacterDbProperties(characterData);

			properties.Name = name;
			properties.Id = id;
			properties.Abilities = AbilityFactory.CreateAndInitiateAbilitiesFromDatabase(name, game);
			return new Character(game, properties);
		}

		private static Character.Properties GetCharacterDbProperties(SqliteRow characterData)
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