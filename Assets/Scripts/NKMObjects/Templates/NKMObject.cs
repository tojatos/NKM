using Managers;

namespace NKMObjects.Templates
{
	public abstract class NKMObject
	{
		protected static Game Game => GameStarter.Instance.Game;
		protected static Active Active => Game.Active;
		protected static Console Console => Console.Instance;
		public string Name { get; set; }
		public GamePlayer Owner { get; set; }

//		protected NKMObject()
//		{
//			Game = GameStarter.Instance.Game;
//			Active = Game.Active;
//			MessageLogger = MessageLogger.Instance;
//		}
	}
}