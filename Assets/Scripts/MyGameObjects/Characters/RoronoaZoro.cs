using System.Collections.Generic;
using MyGameObjects.Abilities.Roronoa_Zoro;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class RoronoaZoro : Character
	{
		public RoronoaZoro()
		{
			Name = "Roronoa Zoro";
			AttackPoints = new Stat(this, StatType.AttackPoints, 27);
			HealthPoints = new Stat(this, StatType.HealthPoints, 97);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 2);
			Speed = new Stat(this, StatType.Speed, 8);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 8);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 5);
			Type = FightType.Melee;

			InitiateAbilities(new List<Ability>
			{
				new LackOfOrientation(),
				new OniGiri(),
				new HyakuHachiPoundHou()
			});

			Description = "";
			Quote = "";
			Author = "Krzysztof Ruczkowski";
		}
	}
}
