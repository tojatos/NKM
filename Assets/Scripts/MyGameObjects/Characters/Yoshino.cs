using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class Yoshino : Character
	{
		public Yoshino()
		{
			Name = "Yoshino";
			AttackPoints = new Stat(this, StatType.AttackPoints, 6);
			HealthPoints = new Stat(this, StatType.HealthPoints, 91);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 3);
			Speed = new Stat(this, StatType.Speed, 5);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 4);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 7);
			Type = FightType.Melee;

			InitiateAbilities(null);

			Description = "";
			Quote = "...Nie lubię okrutnych rzeczy. Nie lubię też... strasznych rzeczy. Ci ludzie pewnie też... bólu, strasznych rzeczy... Chyba oni też... ich nie lubią. Dlatego...";
			Author = "Jakub Mironowicz";
		}

	}
}
