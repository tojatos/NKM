using UnityEngine;

namespace Multiplayer.Network
{
	public abstract class Networking : MonoBehaviour
	{
		protected const int MAX_CONNECTION = 100;
		protected const int BUFFER_SIZE = 65535;

		protected int port = 5701;

		protected int hostId;

		protected int reliableChannel;
		protected int unreliableChannel;
		protected int reliableFragmentedChannel;

		protected int ourClientId;
		protected int connectionID;
		protected float connectionTime;


		protected byte error;
	}
}
