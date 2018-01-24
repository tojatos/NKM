using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Multiplayer.Network
{
	public class Client : MonoBehaviour
	{
		void Awake()
		{
			DontDestroyOnLoad(this);
			SceneManager.sceneLoaded += (scene, mode) =>
			{
				if (SceneManager.GetActiveScene().name == Scenes.MainGame)
				{
					Game = GameStarter.Instance.Game;
				}
			};
		}

		#region NetworkingVariables
		private const int MAX_CONNECTION = 100;
		private const int BUFFER_SIZE = 65535;

		private int port = 5701;

		private int hostId;
		private int webHostId;

		private int reliableChannel;
		private int unreliableChannel;
		private int reliableFragmentedChannel;

		private int ourClientId;
		private int connectionID;
		private float connectionTime;

		private bool isSetUp = false;
		private bool isConnected = false;
		private byte error;
		#endregion
		public string playerName = "testname"; //TODO
		private GamePlayer GamePlayer;
		private Game Game;

		public void Connect(string ipAddress)
		{
			NetworkTransport.Init();
			ConnectionConfig cc = new ConnectionConfig();

			reliableChannel = cc.AddChannel(QosType.Reliable);
			unreliableChannel = cc.AddChannel(QosType.Unreliable);
			reliableFragmentedChannel = cc.AddChannel(QosType.ReliableFragmented);

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
			byte[] recBuffer = new byte[65535];
			int bufferSize = BUFFER_SIZE;
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
				case "GAMEPLAYERS":
					SetGamePlayers(contents.Dequeue());
					break;
				case "NAMEASK":
					SendName(connectionId);
					break;
				case "GET_GAME_PLAYER":
					SendGamePlayer();
					break;
				case "PLAYERLIST":
					List<string> names = contents.ToList();
					UpdatePlayers(names);
					break;
				case "GAMEOPTIONS":
					UpdateGameOptions(contents.ToList());
					break;
				case "GAMESTART":
					PlayerPrefs.SetInt("GameType", (int)GameType.MultiplayerClient);
					SceneManager.LoadScene(Scenes.MainGame);
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

		private void SetGamePlayers(string gamePlayersJson)
		{
			GamePlayers = JsonConvert.DeserializeObject<List<GamePlayer>>(gamePlayersJson, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				Formatting = Formatting.Indented
			});
		}

		private async void SendGamePlayer()
		{
			if (GamePlayer == null) GamePlayer = await GameStarter.Instance.GetGamePlayer();
			var serializedPlayer = JsonConvert.SerializeObject(GamePlayer, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				Formatting = Formatting.Indented
			});
			Send(ComposeMessage("GAMEPLAYER", serializedPlayer), reliableFragmentedChannel);
		}

		public int SelectedMapIndex { get; private set; }
		public int NumberOfPlayers { get; private set; }
		public int PlayersPerCharacter { get; private set; }

		private void UpdateGameOptions(List<string> options)
		{
			NumberOfPlayers = int.Parse(options[0]);
			SelectedMapIndex = int.Parse(options[1]);
			PlayersPerCharacter = int.Parse(options[2]);
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
		private string ComposeMessage(string header, params object[] contents)
		{
			string msg = header;
			contents.ToList().ForEach(c => msg += $"%{c}");
			return msg;
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
		private void Send(string message, int channelId)
		{
			Send(message, channelId, connectionID);
		}

		private List<GamePlayer> GamePlayers;
		public async Task<List<GamePlayer>> GetPlayersFromServer()
		{
			Func<bool> areGamePlayersDerived = () => GamePlayers != null;
			if(areGamePlayersDerived()==false) Send("GET_GAMEPLAYERS", reliableChannel);
			await areGamePlayersDerived.WaitToBeTrue();
			return GamePlayers;
		}
	}
}
