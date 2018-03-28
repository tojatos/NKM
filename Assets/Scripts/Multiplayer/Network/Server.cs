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
		private ServerMessageReceiver MessageReceiver;
		private ServerView ServerView;
		private Game Game;

		public int SelectedMapIndex { get; private set; }
		public int NumberOfPlayers { get; private set; }
		public int CharactersPerPlayer { get; private set; }

		void Awake()
		{
			DontDestroyOnLoad(this);
			SceneManager.sceneLoaded += (scene, mode) =>
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
			MessageReceiver = new ServerMessageReceiver(this);
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
					MessageReceiver.Receive(connectionId, msg);
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

	

		

		public void SendWarning(string msg, int connId) => Send(MessageComposer.Compose("WARNING", msg), reliableChannel, connId);

		public async void SendGamePlayers(int connectionId)
		{
			Func<bool> areAllGamePlayersReceived = () => Players.Count == GamePlayers.Count;
			await areAllGamePlayersReceived.WaitToBeTrue();

			List<string> playersWithNameAndCharacters = new List<string>();
			GamePlayers.Values.ToList().ForEach(g=> playersWithNameAndCharacters.Add(MessageComposer.Compose('*', g.Name, g.Characters.GetNamesWithGuid().Select(c => c.Key + "&" + c.Value).ToArray())));
			Send(MessageComposer.Compose("GAMEPLAYERS", playersWithNameAndCharacters.ToArray()), reliableChannel, connectionId);
		}

		public Player CreatePlayer(int connId, string name)
		{
			var player = new Player(connId, name);
			Players.Add(player);
			return player;
		}

		public void TryJoiningLobby(Player player)
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

		public void AskForName(int connID)
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

		public void ReceiveCharacters(Player player, Queue<string> queue)
		{
			var gamePlayer = new GamePlayer {Name = player.Name}; //TODO: move that somewhere else

			Dictionary<string, Guid> classNamesWithGuids = queue.ToDictionary(x => x.Split('*')[0], x => new Guid(x.Split('*')[1]));
			gamePlayer.AddCharacters(classNamesWithGuids);

			GamePlayers.Add(player, gamePlayer);
		}

//		public void SendSpawnCharacterMessege(HexCell cell, Character activeCharacter)
//		{
//			string msg = MessageComposer.Compose("SPAWN_CHARACTER", activeCharacter.Name, cell.Coordinates.ToString());
//			SendToAllPlayers(msg, reliableChannel);
//		}

		public void TrySettingActiveValue(int connectionId, Queue<string> contents)
		{
			try
			{
				if (GamePlayers.First(g => g.Key.ConnectionID == connectionId).Value != Game.Active.GamePlayer) throw new Exception("Nie jesteś aktywnym graczem!");

				string propertyName = contents.Dequeue();
				string serializedValue = contents.Dequeue();

				if (serializedValue=="")
				{
					switch (propertyName)
					{
						case ActivePropertyName.GamePlayer:						
							Game.Active.GamePlayer = null;
							break;
						case ActivePropertyName.Ability:
							Game.Active.Ability = null;
							break;
						case ActivePropertyName.CharacterOnMap:
							Game.Active.CharacterOnMap = null; 
							break;
						case ActivePropertyName.MyGameObject:
							Game.Active.MyGameObject = null;
							break;
						case ActivePropertyName.Action:
							Game.Active.Action = Action.None;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}

					return;
				}
//				switch (propertyName)
//				{
//					case "GamePlayer":
//						break;
//					default:
//						throw new ArgumentOutOfRangeException();
//				}
				switch (propertyName)
				{
					case ActivePropertyName.GamePlayer:						
						Game.Active.GamePlayer = Game.Players.Find(g => g.Name == serializedValue);
						break;
					case ActivePropertyName.Ability:
						Game.Active.Ability = Game.Players.SelectMany(g => g.Characters).SelectMany(c => c.Abilities).First(a => a.Guid.ToString() == serializedValue);
						break;
					case ActivePropertyName.CharacterOnMap:
						Game.Active.CharacterOnMap = Game.Players.SelectMany(g => g.Characters).First(c => c.Guid.ToString() == serializedValue); 
						break;
					case ActivePropertyName.MyGameObject:
						Game.Active.MyGameObject = Game.Players.SelectMany(g => g.Characters).First(a => a.Guid.ToString() == serializedValue);//TODO find if a character is the only value set there
						break;
					case ActivePropertyName.Action:
						Game.Active.Action = (Action) Enum.Parse(typeof(Action), serializedValue);
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
//		public void TryGettingSerializedActiveValue(int connectionId, string propertyName)
//		{
//            string serializedValue;
//            switch (propertyName)
//            {
//                case "GamePlayer":
//                    serializedValue = Game.Active.GamePlayer.SynchronizableSerialize(ActivePropertyName.GamePlayer);
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//			Send(MessageComposer.Compose("ACTIVE_VAR", propertyName, serializedValue), reliableChannel, connectionId);
//
//			
//			
//		}
//		public static T SynchronizableDeserialize<T>(string value, string name)
//		{
//			T deserializedValue;
//			if (value == null) deserializedValue = default(T);
//			else
//			{
//				
//			}
//
//			return deserializedValue;
//		}
		
		public void TouchCell(int connectionId, Queue<string> contents)
		{
			string coordinates = contents.Dequeue();
			HexCell touchedCell = HexMapDrawer.Instance.Cells.First(c => c.Coordinates.ToString() == coordinates);
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
//		public void MakeAction(int connectionId, Queue<string> contents)
//		{
//			string coordinates = contents.Dequeue();
//			HexCell touchedCell = HexMapDrawer.Instance.Cells.First(c => c.Coordinates.ToString() == coordinates);
//			try
//			{
//				if (GamePlayers.First(g => g.Key.ConnectionID == connectionId).Value != Game.Active.GamePlayer) throw new Exception("Nie jesteś aktywnym graczem!");
//
//				Game.Active.MakeAction(touchedCell);
//				SendToAllPlayers(MessageComposer.Compose("MAKE_ACTION", touchedCell.Coordinates.ToString()), reliableChannel);
//			}
//			catch (Exception e)
//			{
//				SendWarning(e.Message, connectionId);
//				throw;
//			}
//		}
	}
}
