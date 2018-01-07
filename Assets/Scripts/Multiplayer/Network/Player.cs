namespace Multiplayer.Network
{
	public class Player
	{
		public int ConnectionID { get; private set; }
		public string Name { get; private set; }

		public Player(int connID, string name)
		{
			ConnectionID = connID;
			Name = name;
		}
	}
}
