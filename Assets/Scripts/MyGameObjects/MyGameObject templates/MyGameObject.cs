namespace MyGameObjects.MyGameObject_templates
{
	public abstract class MyGameObject
	{
		protected readonly Active Active;
		protected readonly MessageLogger MessageLogger;
		public string Name { get; set; }
		public Player Owner { get; }

		protected MyGameObject()
		{
			Active = Active.Instance;
			MessageLogger = MessageLogger.Instance;
			Owner = Active.Player;
		}
	}
}