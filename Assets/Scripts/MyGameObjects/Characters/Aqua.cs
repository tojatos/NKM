using System.Collections.Generic;
using MyGameObjects.Abilities.Aqua;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class Aqua : Character
	{
		public Aqua()
		{
			Name = "Aqua";
			AttackPoints = new Stat(this, StatType.AttackPoints, 8);
			HealthPoints = new Stat(this, StatType.HealthPoints, 58);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 5);
			Speed = new Stat(this, StatType.Speed, 7);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 3);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 4);
			Type = FightType.Ranged;

			InitiateAbilities(new List<Ability>
			{
				new NaturesBeauty(),
				new Purification(),
				new Resurrection()
			});

			Description = "Niezbyt rozgarnięta pseudo-bogini badająca nadnaturalne sprawy nerdów wpadających pod traktory. Przez brak cierpliwości została wpakowana w testowanie nowej wersji SAO wraz z nikczemnym złodziejem majtek w drużynie. Uwielbia kulturalne spotkania z dobrym, ruskim szampanem oraz królem Sobieskim w lokalnej spelunie ze śmierdzącymi brudasami z okolicy. Wkurwia wszystkich dookoła i myśli, że jest fajna, bo potrafi wyczarować mini-fontannę.";
			Quote = "Dawać tu z majonezem!";
			Author = "Jakub Mironowicz";
		}
	}
}
