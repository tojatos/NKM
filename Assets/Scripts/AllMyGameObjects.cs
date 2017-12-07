using System.Collections.Generic;
using MyGameObjects.Characters;
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
			new Asuna(),
			new DekomoriSanae(),
			new Gilgamesh(),
			new RoronoaZoro(),
			new Aqua(),
			new HanekawaTsubasa(),
			new Hecate(),
			new YasakaMahiro(),
			new Rem(),
			new Sinon(),
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
