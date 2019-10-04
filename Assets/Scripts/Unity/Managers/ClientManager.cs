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
            Client.OnError += message => AsyncCaller.Instance.Call(() => Popup.Create(GameObject.Find("Canvas").transform).Show("Connection error", message));
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
                    if(_gameStarterMessages.Any())
                    {
                        string message = _gameStarterMessages[0];
                        if (GameStarter.Game == null && Header(message) == Messages.NkmRandom)
                        {
                            // get first non nkmrandom msg
                            message = _gameStarterMessages.FirstOrDefault(m => Header(m) != Messages.NkmRandom);
                            if(message==null) return;
                            _gameStarterMessages.Remove(message);
                        }
                        else
                        {
                            _gameStarterMessages.RemoveAt(0);
                        }
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
            Messages.Blind,
            Messages.Draft,
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
            public const string AllRandom = "ALLRANDOM";
            public const string Blind = "BLIND";
            public const string Draft = "DRAFT";
            public const string SetCharacters = "SET_CHARACTERS";
            public const string Action = "ACTION";
            public const string NkmRandom = "NKMRANDOM";
            public const string PlayerNum = "PLAYER_NUM";
            public const string Players = "PLAYERS";
            public const string PlayerJoin = "PLAYER_JOIN";
            public const string PlayerLeft = "PLAYER_LEFT";
            public const string MapName = "MAPNAME";
            public const string Ready = "READY";
            public const string GameInit = "GAMEINIT";
            public const string TooManyPlayers = "TOO_MANY_PLAYERS";
            public const string GetNickname = "GET_NICKNAME";
            public const string Join = "JOIN";
            public const string Reject = "REJECT";
            public const string Stop = "STOP";
        }

    }
}
