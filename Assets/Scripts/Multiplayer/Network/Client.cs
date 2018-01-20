using System.Collections.Generic;
using System.Linq;
using System.Text;
using Managers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Multiplayer.Network
{
	public class Client : MonoBehaviour
	{
		void Awake() => DontDestroyOnLoad(this);
		#region NetworkingVariables
		private const int MAX_CONNECTION = 100;

		private int port = 5701;

		private int hostId;
		private int webHostId;

		private int reliableChannel;
		private int unreliableChannel;

		private int ourClientId;
		private int connectionID;
		private float connectionTime;

		private bool isSetUp = false;
		private bool isConnected = false;
		private byte error;
		#endregion
		private string playerName = "testname"; //TODO

		public void Connect(string ipAddress)
		{
			NetworkTransport.Init();
			ConnectionConfig cc = new ConnectionConfig();

			reliableChannel = cc.AddChannel(QosType.Reliable);
			unreliableChannel = cc.AddChannel(QosType.Unreliable);

			HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

			hostId = NetworkTransport.AddHost(topo, 0);

			connectionID = NetworkTransport.Connect(hostId, ipAddress, port, 0, out error);

			connectionTime = Time.time;
			isSetUp = true;
			Debug.Log("Client started!");
		}
		public void Disconnect()
		{
			Debug.Log("You are disconnected from a server.");
			NetworkTransport.Disconnect(hostId, connectionID, out error);
			Destroy(this);
		}
		private void Update()
		{
			if (!isSetUp) return;

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
				case NetworkEventType.ConnectEvent:
					Debug.Log("| Client connected.");
					isConnected = true;
					Send("CONNECTED", reliableChannel, connectionId);
					SceneManager.LoadScene(Scenes.Lobby);
					break;
				case NetworkEventType.DataEvent:
					string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
					Debug.Log("| Client receiving: " + msg);
					ReceiveMessage(connectionId, msg);
					break;
			}
		}
		private void ReceiveMessage(int connectionId, string msg)
		{
			List<string> messages = msg.Split('|').ToList();
			messages.ForEach(m => ExecuteMessage(connectionId, msg));
		}

		private void ExecuteMessage(int connectionId, string msg)
		{
			Queue<string> contents = new Queue<string>(msg.Split('%'));
			string header = contents.Dequeue();
			switch (header)
			{
				case "NAMEASK":
					SendName(connectionId);
					break;
				case "PLAYERLIST":
					List<string> names = contents.ToList();
					UpdatePlayers(names);
					break;
				case "GAMEOPTIONS":
					UpdateGameOptions(contents.ToList());
					break;
				case "GAMESTART":
					SceneManager.LoadScene(Scenes.MultiPlayerGame);
					break;
				case "DISCONNECT":
					Disconnect();
					SceneManager.LoadScene(Scenes.MultiPlayerSetup);
					break;
				default:
					Debug.Log($"Undefined message: {msg}");
					break;
			}
		}

		private void UpdateGameOptions(List<string> options)
		{
			if (SceneManager.GetActiveScene().name == Scenes.Lobby)
			{
				Lobby l = FindObjectOfType<Lobby>();
				l.UpdateGameOptions(options);
			}
		}
		private void UpdatePlayers(List<string> names)
		{
			if (SceneManager.GetActiveScene().name == Scenes.Lobby)
			{
				Lobby l = FindObjectOfType<Lobby>();
				l.UpdatePlayers(names);
			}
		}

		private void SendName(int connectionId)
		{
			Send($"NAMEIS%{playerName}", reliableChannel, connectionId);
		}
		private void Send(string message, int channelId, int cnnId)
		{
			Debug.Log("| Client sending: " + message);
			byte[] msg = Encoding.Unicode.GetBytes(message);
			NetworkTransport.Send(hostId, cnnId, channelId, msg, msg.Length, out error);
		}


	}
}
