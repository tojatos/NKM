using System.Collections.Generic;
using MyGameObjects.Abilities.Sinon;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class Sinon : Character
	{
		public Sinon()
		{
			Name = "Sinon";
			AttackPoints = new Stat(this, StatType.AttackPoints, 20);
			HealthPoints = new Stat(this, StatType.HealthPoints, 38);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 10);
			Speed = new Stat(this, StatType.Speed, 2);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, -1);
			MagicalDefense = new Stat(this, StatType.PhysicalDefense, -2);
			Type = FightType.Ranged;

			InitiateAbilities(new List<Ability>
			{
				new SnipersSight(),
				new TacticalEscape(),
				new PreciseShot()
			});

			Description = "";
			Quote = "";
			Author = "Krzysztof Ruczkowski";
		}
	}
}
