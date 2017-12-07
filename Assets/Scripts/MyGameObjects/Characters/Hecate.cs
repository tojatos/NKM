using System.Collections.Generic;
using MyGameObjects.Abilities.Hecate;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class Hecate : Character
	{
		public Hecate()
		{
			Name = "Hecate";
			AttackPoints = new Stat(this, StatType.AttackPoints, 11);
			HealthPoints = new Stat(this, StatType.HealthPoints, 59);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 7);
			Speed = new Stat(this, StatType.Speed, 3);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 1);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 4);
			Type = FightType.Ranged;
			InitiateAbilities(new List<Ability>
			{
				new ItadakiNoKura(),
				new AsterYo(),
				new SonzaiNoChikara()
			});

			Description = "";
			Quote = "";
			Author = "Jakub Mironowicz";
		}
	}
}
