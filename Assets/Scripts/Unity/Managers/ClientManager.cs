using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.Managers
{
	public class ClientManager : CreatableSingletonMonoBehaviour<ClientManager>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void DDOL() => DontDestroyOnLoad(Instance);

		public Client Client;
		private readonly List<string> _gameStarterMessages = new List<string>();
		private readonly List<string> _serverLobbyMessages = new List<string>();
		private readonly List<string> _serverListMessages = new List<string>();

		private void Awake()
		{
			Client = new Client();
			Client.OnConnection += () => Debug.Log("Connected!");
			Client.OnDisconnect += () => Debug.Log("Disconnected!");
			Client.OnError += message => AsyncCaller.Instance.Call(() => Debug.LogError(message));
			Client.OnError += message => AsyncCaller.Instance.Call(() => Popup.Instance.Show("Connection error", message));
			Client.OnMessage += message => Debug.Log($"Server: {message}");
			Client.OnMessage += message =>
			{
                string header = message.Split(new []{' '}, 2)[0];
                if (_availableGameStarterMessages.Contains(header))
	                _gameStarterMessages.Add(message);
                if (_availableServerLobbyMessages.Contains(header))
	                _serverLobbyMessages.Add(message);
                if (_availableServerListMessages.Contains(header))
	                _serverListMessages.Add(message);
			};
		}

		private void Update()
		{
			switch (SceneManager.GetActiveScene().name)
			{
				case Scenes.MainGame:
				{
                    string Header(string msg) => msg.Split(new []{' '}, 2)[0];
                    if (_gameStarterMessages.Any(m => m == Messages.AllRandom))
                    {
						_gameStarterMessages.Remove(Messages.AllRandom);
						GameStarter.Instance.HandleMessageFromServer(Messages.AllRandom);
						return;
                    }
                    if (_gameStarterMessages.Any(m => Header(m) == Messages.SetCharacters))
                    {
						string message = _gameStarterMessages.First(m => Header(m) == Messages.SetCharacters);
						_gameStarterMessages.RemoveAll(m => Header(m) == Messages.SetCharacters);
						GameStarter.Instance.HandleMessageFromServer(message);
						return;
                    }
                    if (GameStarter.Game == null) return;
					if(_gameStarterMessages.Any())
					{
						string message = _gameStarterMessages[0];
						_gameStarterMessages.RemoveAt(0);
						GameStarter.Instance.HandleMessageFromServer(message);
					}
				} break;
				case Scenes.ServerList:
				{
					if(_serverListMessages.Any())
					{
						string message = _serverListMessages[0];
						_serverListMessages.RemoveAt(0);
						ServerList.Instance.HandleMessageFromServer(message);
					}
				} break;
				case Scenes.ServerLobby:
				{
					if(_serverLobbyMessages.Any())
					{
						string message = _serverLobbyMessages[0];
						_serverLobbyMessages.RemoveAt(0);
						ServerLobby.Instance.HandleMessageFromServer(message);
					}
				} break;
			}
		}

		private readonly List<string> _availableGameStarterMessages = new List<string>
		{
			Messages.AllRandom,
			Messages.SetCharacters,
			Messages.Action,
			Messages.NkmRandom,
			Messages.Stop,
		};
		private readonly List<string> _availableServerLobbyMessages = new List<string>
		{
			Messages.PlayerNum,
			Messages.Players,
			Messages.PlayerJoin,
			Messages.PlayerLeft,
			Messages.MapName,
			Messages.Ready,
			Messages.GameInit,

		};
		private readonly List<string> _availableServerListMessages = new List<string>
		{
			Messages.TooManyPlayers,
			Messages.GetNickname,
			Messages.Join,
			Messages.Reject,
		};

		public static class Messages
		{
			public static string AllRandom => "ALLRANDOM";
			public static string SetCharacters => "SET_CHARACTERS";
			public static string Action => "ACTION";
			public static string NkmRandom => "NKMRANDOM";
			public static string PlayerNum => "PLAYER_NUM";
            public static string Players => "PLAYERS";
            public static string PlayerJoin => "PLAYER_JOIN";
            public static string PlayerLeft => "PLAYER_LEFT";
            public static string MapName => "MAPNAME";
            public static string Ready => "READY";
            public static string GameInit => "GAMEINIT";
			public static string TooManyPlayers => "TOO_MANY_PLAYERS";
			public static string GetNickname => "GET_NICKNAME";
			public static string Join => "JOIN";
            public static string Reject => "REJECT";
            public static string Stop => "STOP";
		}

	}
}
