﻿using System.Collections.Generic;
using Extensions;
using NKMObjects.Templates;

namespace NKMObjects
{
    public static class CharacterFactory
    {
		public static NKMCharacter Create(string name) => 
			CreateCharacterFromDatabase(name, NKMID.GetNext("Character"));

		private static NKMCharacter CreateCharacterFromDatabase(string name, int id)
		{
			SqliteRow characterData = GameData.Conn.GetCharacterData(name);
			CharacterProperties properties = GetCharacterProperties(characterData);
			List<Ability> abilities = AbilityFactory.CreateAndInitiateAbilitiesFromDatabase(name);
			
			return new NKMCharacter(name, id, properties, abilities);
		}

		private static CharacterProperties GetCharacterProperties(SqliteRow characterData)
		{
			return new CharacterProperties
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

				Description = characterData.GetValue("Description"),
				Quote = characterData.GetValue("Quote"),
				Author = characterData.GetValue("Author.Name"),


			};
		}

		public static NKMCharacter CreateWithoutId(string name)
		{
			return CreateCharacterFromDatabase(name, -1);
		}

	    public static Character CreateNonGame(string name, CharacterProperties properties, List<Ability> abilities)
	    {
			return new Character(name, -1, properties, abilities);
	    }
        
    }
}