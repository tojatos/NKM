using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Managers;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Multiplayer.Network
{
	public class Server : Networking
	{
		#region NetworkingVariables
//		private const int MAX_CONNECTION = 100;
//		private const int BUFFER_SIZE = 65535;
//
//		private int port = 5701;
//
//		private int hostId;
//		private int webHostId;
//		
//		private int reliableChannel;
//		private int unreliableChannel;
//		private int reliableFragmentedChannel;
//
//
//		private bool isStarted = false;
//		private byte error;
#endregion

		public List<Player> Players = new List<Player>();
		public Dictionary<Player, GamePlayer> GamePlayers = new Dictionary<Player, GamePlayer>();
		private ServerView ServerView;
//		private GameServerSide Game;

		public int SelectedMapIndex { get; private set; }
		public int NumberOfPlayers { get; private set; }
		public int PlayersPerCharacter { get; private set; }

		void Awake()
		{
			DontDestroyOnLoad(this);
			SceneManager.sceneLoaded += (Scene, mode) =>
			{
				if (SceneManager.GetActiveScene().name == Scenes.ServerView)
				{
					ServerView = FindObjectOfType<ServerView>();
					var gameOptions = new List<int> { NumberOfPlayers, SelectedMapIndex, PlayersPerCharacter };
					ServerView.UpdateGameOptions(gameOptions.ConvertAll(x => x.ToString()));
				}
			};
		}
		private void Start()
		{
			StartServer();

			NumberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers");
			SelectedMapIndex = PlayerPrefs.GetInt("SelectedMapIndex");
			PlayersPerCharacter = PlayerPrefs.GetInt("NumberOfCharactersPerPlayer");
		}

		private bool _isStarted;
		private void StartServer()
		{
			NetworkTransport.Init();
			ConnectionConfig cc = new ConnectionConfig();

			reliableChannel = cc.AddChannel(QosType.Reliable);
			unreliableChannel = cc.AddChannel(QosType.Unreliable);
			reliableFragmentedChannel = cc.AddChannel(QosType.ReliableFragmented);


			HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

			hostId = NetworkTransport.AddHost(topo, port, null);
			_isStarted = true;
			Debug.Log("Server started!");
		}
		private void Update()
		{
			if (!_isStarted) return;

			int recHostId;
			int connectionId;
			int channelId;
			byte[] recBuffer = new byte[65535];
			int bufferSize = BUFFER_SIZE;
			int dataSize;
			byte error;
			NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
				bufferSize, out dataSize, out error);
			switch (recData)
			{
				case NetworkEventType.ConnectEvent:
					Debug.Log("GamePlayer " + connectionId + " has connected");
					break;
				case NetworkEventType.DataEvent:
					if(error!=0)
					{
						Debug.LogError((NetworkError)error);
						return;
					}
					string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
					ReceiveMessage(connectionId, msg);
					//Debug.Log("GamePlayer " + connectionId + " has sent: " + msg);
					break;
				case NetworkEventType.DisconnectEvent:
					OnDisconnect(connectionId);
					Debug.Log("GamePlayer " + connectionId + " has disconnected");
					break;
			}
		}

		private void OnDisconnect(int connectionId)
		{
			Players.RemoveAll(p => p.ConnectionID == connectionId);
			UpdatePlayers();
			if (Players.Count < NumberOfPlayers)
			{
				ServerView.CanStart = false;
			}
		}

		public void SendDisconnnectMessage(int connId)
		{
			Send("DISCONNECT", reliableChannel, connId);
		}

		private void ReceiveMessage(int connectionId, string msg)
		{
			List<string> messages = msg.Split('|').ToList();
			foreach (var m in messages)
			{
				Queue<string> contents = new Queue<string>(msg.Split('%'));
				string header = contents.Dequeue();
				Debug.Log($"| Server receiving: {header}");
				switch (header)
				{
					case "GET_GAMEPLAYERS":
						SendGamePlayers(connectionId);
						break;
					case "CHARACTERS":
//						GamePlayer gamePlayer = JsonConvert.DeserializeObject<GamePlayer>(contents[1], new JsonSerializerSettings
//						{
//							TypeNameHandling = TypeNameHandling.Auto,
//						});
						ReceiveCharacters(Players.Single(p => p.ConnectionID == connectionId), contents.ToList());
						break;
					case "CONNECTED":
						AskForName(connectionId);
						break;
					case "NAMEIS":
						var name = contents.Dequeue();
						var player = CreatePlayer(connectionId, name);
						TryJoiningLobby(player);
						break;
					default:
						Debug.Log($"Undefined message: {m}");
						break;
				}
			}
		}

		private async void SendGamePlayers(int connectionId)
		{
			Func<bool> areAllGamePlayersReceived = () => Players.Count == GamePlayers.Count;
			await areAllGamePlayersReceived.WaitToBeTrue();

			List<string> playersWithNameAndCharacters = new List<string>();
			GamePlayers.Values.ToList().ForEach(g=> playersWithNameAndCharacters.Add(ComposeMessage('*', g.Name, g.Characters.GetClassNames().ToArray())));
			Send(ComposeMessage("GAMEPLAYERS", playersWithNameAndCharacters.ToArray()), reliableChannel, connectionId);
		}

		private Player CreatePlayer(int connId, string name)
		{
			var player = new Player(connId, name);
			Players.Add(player);
			return player;
		}
		private void TryJoiningLobby(Player player)
		{
			if(Players.Count <= NumberOfPlayers)
			{
				JoinLobby(player);
			}
			else
			{
				Players.Remove(player);
				NetworkTransport.Disconnect(hostId, player.ConnectionID, out error);
			}
		}

		private void JoinLobby(Player player)
		{
			SendGameOptions(player.ConnectionID);
			UpdatePlayers();
			if (Players.Count == NumberOfPlayers)
			{
				ServerView.CanStart = true;
			}
		}
		private void SendGameOptions(int playerConnectionId)
		{
			var msg = ComposeMessage("GAMEOPTIONS", NumberOfPlayers.ToString(), SelectedMapIndex.ToString(), PlayersPerCharacter.ToString());
			Send(msg, reliableChannel, playerConnectionId);
		}

		private void UpdatePlayers()
		{
			string msg = "PLAYERLIST%";
			Players.ForEach(p => msg += $"{p.Name}%");
			msg = msg.TrimEnd('%');
			SendToAllPlayers(msg, reliableChannel);

			ServerView.UpdatePlayers(Players);
		}

		private void AskForName(int connID)
		{
			Send("NAMEASK", reliableChannel, connID);
		}
		private void Send(string message, int channelId, int cnnID)
		{
			Debug.Log("Sending: " + message);
			byte[] msg = Encoding.Unicode.GetBytes(message);
			NetworkTransport.Send(hostId, cnnID, channelId, msg, msg.Length, out error);

		}
		private void Send(string message, int channelId, List<int> connectionIds)
		{
			connectionIds.ForEach(connId => Send(message, channelId, connId));
		}
		private void SendToAllPlayers(string message, int channelId)
		{
			var connIds = Players.Select(p => p.ConnectionID).ToList();
			Send(message, channelId, connIds);
		}
		private string ComposeMessage(char delimiter, string header, params string[] contents)
		{
			string msg = header;
			contents.ToList().ForEach(c => msg += $"{delimiter}{c}");
			return msg;
		}
		private string ComposeMessage(string header, params string[] contents)
		{
			return ComposeMessage('%', header, contents);
		}
		public void TryStartingGame()
		{
			if (NumberOfPlayers != Players.Count) return;

			StartGame();
		}

		private void StartGame()
		{
			PlayerPrefs.SetInt("GameType", (int)GameType.MultiplayerServer);
			SceneManager.LoadScene(Scenes.MainGame);
			SendToAllPlayers("GAMESTART", reliableChannel);
		}

		public async Task<List<GamePlayer>> GetCharactersFromClients()
		{
			SendToAllPlayers("GET_CHARACTERS", reliableChannel);
			Func<bool> areAllGamePlayersReceived = () => GamePlayers.Count == Players.Count;
			await areAllGamePlayersReceived.WaitToBeTrue();
			return GamePlayers.Values.ToList();

		}

		private void ReceiveCharacters(Player player, List<string> classNames)
		{
			var gamePlayer = new GamePlayer() {Name = player.Name}; //TODO: move that somewhere else

			//TODO: DRY
			var characters = Spawner.Create("Characters", classNames).Cast<Character>().ToList();
			gamePlayer.Characters.AddRange(characters);
			GamePlayers.Add(player, gamePlayer);
		}
	}
}
