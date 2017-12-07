using System.Collections.Generic;
using MyGameObjects.Abilities.Yasaka_Mahiro;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class YasakaMahiro : Character
	{
		public YasakaMahiro()
		{
			Name = "Yasaka Mahiro";
			AttackPoints = new Stat(this, StatType.AttackPoints, 17);
			HealthPoints = new Stat(this, StatType.HealthPoints, 48);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 6);
			Speed = new Stat(this, StatType.Speed, 6);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 3);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 3);
			Type = FightType.Ranged;
			InitiateAbilities(new List<Ability>
			{
				new WhenTheyCry(),
				new SharpenedForks(),
				new TerrorOfTheUniverse()
			});

			Description = "";
			Quote = "";
			Author = "Jakub Mironowicz";
		}
	}
}
