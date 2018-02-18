using System.Collections.Generic;
using Helpers;
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
		Characters = new List<Character>();

		var characterNames = GameData.Conn.GetCharacterNames();
		characterNames.ForEach(n=>Characters.Add(new Character(n)));
//		{
//			new Character("Asuna"),
//			new Character("Dekomori Sanae"),
//			new Character("Gilgamesh"),
//			new Character("Roronoa Zoro"),
//			new Character("Aqua"),
//			new Character("Hanekawa Tsubasa"),
//			new Character("Hecate"),
//			new Character("YasakaMahiro"),
//			new Character("Rem"),
//			new Character("Sinon"),
//		};
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
