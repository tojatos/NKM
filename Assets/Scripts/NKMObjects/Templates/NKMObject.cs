using Managers;

namespace NKMObjects.Templates
{
	public abstract class NKMObject
	{
		protected readonly Game Game;
		protected readonly Active Active;
		protected readonly MessageLogger MessageLogger;
		public string Name { get; set; }
		public GamePlayer Owner { get; set; }

		protected NKMObject()
		{
			Game = GameStarter.Instance.Game;
			Active = Game.Active;
			MessageLogger = MessageLogger.Instance;
		}
	}
}