using System.Collections.Generic;
using MyGameObjects.Abilities.Dekomori_Sanae;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class DekomoriSanae : Character
	{
		public DekomoriSanae()
		{
			Name = "Dekomori Sanae";
			AttackPoints = new Stat(this, StatType.AttackPoints, 16);
			HealthPoints = new Stat(this, StatType.HealthPoints, 46);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 7);
			Speed = new Stat(this, StatType.Speed, 4);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 2);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 3);
			Type = FightType.Ranged;
			InitiateAbilities(new List<Ability>
			{
				new WickedEyesServant(),
				new MjolnirHammer(),
				new MjolnirDestinyImpulse()
			});

			Description = "Uzależniona od LSD i kokainy rozpuszczona córka lokalnego bogacza. W wolnych chwilach służy ciałem i duszą Potwornemu Oku, aka Ricce Takanashi, używając ciężkich kul przyczepionych do końców kucyków jako broni. sama Sanae twierdzi, że tak naprawdę jest to młot jednego z największych skurwoli z mitologii nordyckiej. Podobno łączą jej intymne relacje z miejscowym dealerem.";
			Quote = "Desu!";
			Author = "Jakub Mironowicz";
		}
	}
}
