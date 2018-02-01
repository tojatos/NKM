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
	public class Client : Networking
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
		private bool _isSetUp;
		private bool _isConnected;
		public string PlayerName = "testname"; //TODO
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
			_isSetUp = true;
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
			if (!_isSetUp) return;

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
					_isConnected = true;
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

		public void SendUseMyGameObjectMessage(HexCell touchedCell, MyGameObject myGameObject)
		{
			var msg = MessageComposer.Compose("USE_MYGAMEOBJECT", myGameObject.Name, touchedCell.Coordinates.ToString());
			Send(msg, reliableChannel);
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
				case "WARNING":
					Debug.LogWarning(contents.Dequeue());
					break;
				case "SPAWN_CHARACTER":
					string characterName = contents.Dequeue();
					string cellCoordinates = contents.Dequeue();
					//TODO: change that
					Game.Active.MyGameObject = Game.Active.GamePlayer.Characters.First(c => c.Name == characterName);
					Game.UseMyGameObject(HexMapDrawer.Instance.Cells.First(c => c.Coordinates.ToString() == cellCoordinates));
					break;
				case "GAMEPLAYERS":
					SetGamePlayers(contents.ToList());
					break;
				case "NAMEASK":
					SendName(connectionId);
					break;
				case "GET_CHARACTERS":
					SendSelectedCharacters();
					break;
				case "PLAYERLIST":
					List<string> names = contents.ToList();
					UpdatePlayers(names);
					break;
				case "GAMEOPTIONS":
					UpdateGameOptions(contents.ToList());
					break;
				case "GAMESTART":
					SessionSettings.Instance.GameType = GameType.MultiplayerClient;
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

		private void SetGamePlayers(List<string> gamePlayersData)
		{
			GamePlayers = new List<GamePlayer>();
			gamePlayersData.ForEach(data =>
			{
				Queue<string> classNames = new Queue<string>(data.Split('*'));
				string gamePlayerName = classNames.Dequeue();
				var gamePlayer = new GamePlayer { Name = gamePlayerName };

				gamePlayer.AddCharacters(classNames);
				GamePlayers.Add(gamePlayer);
			});
		}

		private async void SendSelectedCharacters()
		{
			if (GamePlayer == null) GamePlayer = await GameStarter.Instance.GetGamePlayer();
			List<string> characterClasses = GamePlayer.Characters.GetClassNames().ToList();
			Send(MessageComposer.Compose("CHARACTERS", characterClasses.ToArray()), reliableChannel);
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


		private void SendName(int connectionId)
		{
			Send($"NAMEIS%{PlayerName}", reliableChannel, connectionId);
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
