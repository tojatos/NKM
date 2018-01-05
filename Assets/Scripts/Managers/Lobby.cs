using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
	//TODO: Create a lobby

	public class Lobby : SingletonMonoBehaviour<Lobby>
	{
		public void BackButtonClick() => SceneManager.LoadScene(Scenes.GameHostOptions);
		#region MoveThisLater
		[SerializeField] private Text serverStatusText;
		private const int MAX_CONNECTION = 100;

		private int port = 5701;

		private int hostId;
		private int webHostId;

		private int reliableChannel;
		private int unreliableChannel;

		private bool isStarted = false;
		private byte error;

		private void Start()
		{
			NetworkTransport.Init();
			ConnectionConfig cc = new ConnectionConfig();

			reliableChannel = cc.AddChannel(QosType.Reliable);
			unreliableChannel = cc.AddChannel(QosType.Unreliable);

			HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

			hostId = NetworkTransport.AddHost(topo, port, null);
			//webHostId = NetworkTransport.AddWebsocketHost(topo, port, null);

			isStarted = true;
			serverStatusText.text = $"Server status:\nListening at port {port}";
		}
		void Update()
		{
			if (!isStarted) return;
			int recHostId;
			int connectionId;
			int channelId;
			byte[] recBuffer = new byte[1024];
			int bufferSize = 1024;
			int dataSize;
			byte error;
			NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
			switch (recData)
			{
				case NetworkEventType.Nothing:
					//Debug.Log("Nothing at all");
					break;
				case NetworkEventType.ConnectEvent:
					Debug.Log("Player " + connectionId + " has connected");
					break;
				case NetworkEventType.DataEvent:
					string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
					Debug.Log("Player " + connectionId + " has sent: " + msg);
					break;
				case NetworkEventType.DisconnectEvent:
					Debug.Log("Player " + connectionId + " has disconnected");
					break;
			}
		}
#endregion
	}
}