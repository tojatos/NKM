using System.Collections.Generic;
using MyGameObjects.Abilities.Asuna;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class Asuna : Character
	{
		public Asuna()
		{
			Name = "Asuna";
			AttackPoints = new Stat(this, StatType.AttackPoints, 12);
			HealthPoints = new Stat(this, StatType.HealthPoints, 66);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 4);
			Speed = new Stat(this, StatType.Speed, 7);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 4);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 2);
			Type = FightType.Melee;

			InitiateAbilities(new List<Ability>
			{
				new LambentLight(),
				new Dash(),
				new SwordDance()
			});

			Description = "";
			Quote = "";
			Author = "Damian Gruszecki";
		}
	}
}
