using Managers;

namespace MyGameObjects.MyGameObject_templates
{
	public abstract class MyGameObject
	{
		protected readonly Game Game;
		protected readonly Active Active;
		protected readonly MessageLogger MessageLogger;
		public string Name { get; set; }
		public GamePlayer Owner { get; set; }

		protected MyGameObject()
		{
			Game = GameStarter.Instance.Game;
			Active = Game.Active;
			MessageLogger = MessageLogger.Instance;
		}
	}
}