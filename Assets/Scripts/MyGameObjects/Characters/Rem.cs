using System.Collections.Generic;
using MyGameObjects.Abilities.Rem;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class Rem : Character
	{
		public Rem()
		{
			Name = "Rem";
			AttackPoints = new Stat(this, StatType.AttackPoints, 12);
			HealthPoints = new Stat(this, StatType.HealthPoints, 54);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 6);
			Speed = new Stat(this, StatType.Speed, 5);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 2);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 3);
			Type = FightType.Melee;

			InitiateAbilities(new List<Ability>
			{
				new DemonicForm(),
				new MorgensternHit(),
				new AlHuma(),
				new Confession()
			});

			Description = "";
			Quote = "";
			Author = "Marcin Mateja";
		}
	}
}
