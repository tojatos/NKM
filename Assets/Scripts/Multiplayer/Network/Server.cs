using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Hex;
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
		private Game Game;

		public int SelectedMapIndex { get; private set; }
		public int NumberOfPlayers { get; private set; }
		public int CharactersPerPlayer { get; private set; }

		void Awake()
		{
			DontDestroyOnLoad(this);
			SceneManager.sceneLoaded += (Scene, mode) =>
			{
				if (SceneManager.GetActiveScene().name == Scenes.ServerView)
				{
					ServerView = FindObjectOfType<ServerView>();
					var gameOptions = new List<int> { NumberOfPlayers, SelectedMapIndex, CharactersPerPlayer };
					ServerView.UpdateGameOptions(gameOptions.ConvertAll(x => x.ToString()));
				}
				else if (SceneManager.GetActiveScene().name == Scenes.MainGame)
				{
					Game = GameStarter.Instance.Game;
				}
			};
		}
		private void Start()
		{
			StartServer();

			NumberOfPlayers = SessionSettings.Instance.NumberOfPlayers;
			SelectedMapIndex = SessionSettings.Instance.SelectedMapIndex;
			CharactersPerPlayer = SessionSettings.Instance.NumberOfCharactersPerPlayer;
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
					case "ACTIVE_VAR_SET":
						TrySettingActiveValue(connectionId, contents);
						break;
					case "TOUCH_CELL":
						TouchCell(connectionId, contents);
						break;
//					case "USE_MYGAMEOBJECT":
//						TryUsingMyGameObject(connectionId, contents);
//						break;
					case "CHARACTERS":
						ReceiveCharacters(Players.Single(p => p.ConnectionID == connectionId), contents);
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

		private void TrySettingActiveValue(int connectionId, Queue<string> contents)
		{
			try
			{
				if (GamePlayers.First(g => g.Key.ConnectionID == connectionId).Value != Game.Active.GamePlayer) throw new Exception("Nie jesteś aktywnym graczem!");

				string propertyName = contents.Dequeue();
				string serializedValue = contents.Dequeue();
				switch (propertyName)
				{
					case "GamePlayer":
						Game.Active.GamePlayer = Game.Players.Find(g => g.Name == serializedValue);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			catch (Exception e)
			{
				SendWarning(e.Message, connectionId);
				throw;
			}
		}

		private void TouchCell(int connectionId, Queue<string> contents)
		{
			HexCell touchedCell = HexMapDrawer.Instance.Cells.First(c => c.Coordinates.ToString() == contents.Dequeue());
			try
			{
				if (GamePlayers.First(g => g.Key.ConnectionID == connectionId).Value != Game.Active.GamePlayer) throw new Exception("Nie jesteś aktywnym graczem!");

				Game.TryTouchingCell(touchedCell);
				SendToAllPlayers(MessageComposer.Compose("TOUCH_CELL", touchedCell.Coordinates.ToString()), reliableChannel);
			}
			catch (Exception e)
			{
				SendWarning(e.Message, connectionId);
				throw;
			}
		}

//		private void TryUsingMyGameObject(int connectionId, Queue<string> contents)
//		{
//			try
//			{
//				if (GamePlayers.First(g => g.Key.ConnectionID == connectionId).Value != Game.Active.GamePlayer) throw new Exception("Nie jesteś aktywnym graczem!");
//
//				string characterName = contents.Dequeue();
//				string cellCoordinates = contents.Dequeue();
//				Game.Active.MyGameObject = Game.Active.GamePlayer.Characters.First(c=>c.Name==characterName && !c.IsOnMap); //TODO: this needs to be changed, use guid maybe
//				Game.TouchCell(HexMapDrawer.Instance.Cells.First(c=>c.Coordinates.ToString()==cellCoordinates));
//			}
//			catch (Exception e)
//			{
//				SendWarning(e.Message, connectionId);
//				throw;
//			}
//
//		}

			public void SendWarning(string msg, int connId) => Send(MessageComposer.Compose("WARNING", msg), reliableChannel, connId);

		private async void SendGamePlayers(int connectionId)
		{
			Func<bool> areAllGamePlayersReceived = () => Players.Count == GamePlayers.Count;
			await areAllGamePlayersReceived.WaitToBeTrue();

			List<string> playersWithNameAndCharacters = new List<string>();
			GamePlayers.Values.ToList().ForEach(g=> playersWithNameAndCharacters.Add(MessageComposer.Compose('*', g.Name, g.Characters.GetClassNamesWithGuid().Select(c => c.Key + "&" + c.Value).ToArray())));
			Send(MessageComposer.Compose("GAMEPLAYERS", playersWithNameAndCharacters.ToArray()), reliableChannel, connectionId);
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
			var msg = MessageComposer.Compose("GAMEOPTIONS", NumberOfPlayers.ToString(), SelectedMapIndex.ToString(), CharactersPerPlayer.ToString());
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

		public void TryStartingGame()
		{
			if (NumberOfPlayers != Players.Count) return;

			StartGame();
		}

		private void StartGame()
		{
			SessionSettings.Instance.GameType = GameType.MultiplayerServer;
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

		private void ReceiveCharacters(Player player, Queue<string> queue)
		{
			var gamePlayer = new GamePlayer {Name = player.Name}; //TODO: move that somewhere else

			Dictionary<string, Guid> classNamesWithGuids = queue.ToDictionary(x => x.Split('*')[0], x => new Guid(x.Split('*')[1]));
			gamePlayer.AddCharacters(classNamesWithGuids);

			GamePlayers.Add(player, gamePlayer);
		}

		public void SendSpawnCharacterMessege(HexCell cell, Character activeCharacter)
		{
			string msg = MessageComposer.Compose("SPAWN_CHARACTER", activeCharacter.Name, cell.Coordinates.ToString());
			SendToAllPlayers(msg, reliableChannel);
		}
	}
}
