using System.Collections.Generic;
using MyGameObjects.MyGameObject_templates;

public class AllMyGameObjects
{
	#region Singleton
	private static AllMyGameObjects _instance;
	public static AllMyGameObjects Instance => _instance ?? (_instance = new AllMyGameObjects());
	#endregion

	public readonly List<Character> Characters;// { get; private set; }
	//public readonly List<Item> Items;// { get; private set; }
	//public readonly List<Potion> Potions;// { get; private set; }

	private AllMyGameObjects()
	{
		Characters = new List<Character>
		{
			new Character("Asuna"),
			new Character("DekomoriSanae"),
			new Character("Gilgamesh"),
			new Character("RoronoaZoro"),
			new Character("Aqua"),
			new Character("HanekawaTsubasa"),
			new Character("Hecate"),
			new Character("YasakaMahiro"),
			new Character("Rem"),
			new Character("Sinon"),
		};
		//Items = new List<Item>
		//{
		//	new GlassEye(),
		//	new RikkiBand(),
		//	new SachikoScissors(),
		//	new Yoshinon(),
		//	new YuzuruHeart()
		//};
		//Potions = new List<Potion>
		//{
		//	new FreezePotion(),
		//	new HealthPotion(),
		//	new InvisibilityPotion(),
		//	new OraclePotion()
		//};
	}
}
