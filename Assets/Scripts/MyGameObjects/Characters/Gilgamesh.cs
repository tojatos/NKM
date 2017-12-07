using System.Collections.Generic;
using MyGameObjects.Abilities.Gilgamesh;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class Gilgamesh : Character
	{
		public Gilgamesh()
		{
			Name = "Gilgamesh";
			AttackPoints = new Stat(this, StatType.AttackPoints, 15);
			HealthPoints = new Stat(this, StatType.HealthPoints, 68);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 6);
			Speed = new Stat(this, StatType.Speed, 5);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 4);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 4);
			Type = FightType.Ranged;
			InitiateAbilities(new List<Ability>
			{
				new TheFistHero(),
				new Enkidu(),
				new GateOfBabylon()
			});

			Description = "";
			Quote = "";
			Author = "Jakub Mironowicz";
		}
	}
}
