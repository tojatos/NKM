using Multiplayer.Network;
using UnityEngine;

namespace Multiplayer
{
	public class GameClientSide : MonoBehaviour
	{
		private Server ActiveServer;

		void Awake()
		{
			ActiveServer = FindObjectOfType<Server>();
		}
	}
}