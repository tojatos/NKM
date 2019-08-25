using System.Collections.Generic;
using System.Linq;
using NKMCore;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Managers
{
    public class ServerLobby : SingletonMonoBehaviour<ServerLobby>
    {
        public Button ReadyButton;
        public GameObject PlayersGameObject;
        public Text Map;

        private static GamePreparerDependencies Dependencies
        {
            get => SessionSettings.Instance.Dependencies;
            set => SessionSettings.Instance.Dependencies = value;
        }

        private Client _client;
        private readonly Dictionary<int, string> _players = new Dictionary<int, string>();
        private readonly Dictionary<int, bool> _readyStates = new Dictionary<int, bool>();

        private void Start()
        {
            _client = ClientManager.Instance.Client;
            Dependencies = new GamePreparerDependencies();

            Map.text = "";
            ClearPlayerList();
            ReadyButton.onClick.AddListener(() => _client.SendMessage("READY_CHANGE"));

            GameObject.FindGameObjectsWithTag("Back Button").ToList()
                .ForEach(b => b.AddTrigger(EventTriggerType.PointerClick, () => _client.Disconnect()));
            ChangeSceneOnDisconnect();
            AskServerForGameInfo();
        }

        private void AskServerForGameInfo() => _client.SendMessage("GAME_INFO");

        private void ChangeSceneOnDisconnect()
        {
            void OnDisconnect() => AsyncCaller.Instance.Call(ShortcutManager.Instance.LoadLastScene);
            _client.OnDisconnect += OnDisconnect;

            void RemoveTrigger(Scene scene, LoadSceneMode mode)
            {
                _client.OnDisconnect -= OnDisconnect;
                SceneManager.sceneLoaded -= RemoveTrigger;
            }

            SceneManager.sceneLoaded += RemoveTrigger;
        }

        public void HandleMessageFromServer(string message)
        {
            string[] data = message.Split(new []{' '}, 2);
            string header = data[0];
            string content = string.Empty;
            if(data.Length > 1) content = data[1];
            switch (header)
            {
                case "PLAYER_NUM":
                    Dependencies.NumberOfPlayers = int.Parse(content);
                    RefreshList();
                    break;
                case "PLAYERS":
                    InitPlayers(content);
                    RefreshList();
                    break;
                case "PLAYER_JOIN":
                    HandlePlayerJoin(content);
                    RefreshList();
                    break;
                case "PLAYER_LEFT":
                    int index = int.Parse(content);
                    _players.Remove(index);
                    RefreshList();
                    break;
                case "MAPNAME":
                    HandleMapname(content);
                    RefreshList();
                    break;
                case "READY":
                    string[] x = content.Split(';');
                    int playerIndex = int.Parse(x[0]);
                    bool readyState = bool.Parse(x[1]);
                    _readyStates[playerIndex] = readyState;
                    RefreshList();
                    break;
                case "GAMEINIT":
                    LoadGame();
                    break;
            }
        }

        private void LoadGame()
        {
            Dependencies.PlayerNames = _players.OrderBy(p => p.Key).Select(p => p.Value).ToList();
            SceneManager.LoadScene(Scenes.MainGame);
        }

        private void InitPlayers(string content)
        {
            string[] playerInfos = content.Split(';');
            foreach (string i in playerInfos.ToList())
            {
                string[] playerData = i.Split(':');
                int playerIndex = int.Parse(playerData[0]);
                string playerName = playerData[1];
                bool isReady = bool.Parse(playerData[2]);
                _players[playerIndex] = playerName;
                _readyStates[playerIndex] = isReady;
            }
        }

        private void HandleMapname(string content)
        {
            Dependencies.HexMap = HexMapFactory.FromScriptable(Stuff.Maps.First(m => m.name == content));
            Map.text = content;
        }

        private void HandlePlayerJoin(string content)
        {
            string[] s = content.Split(';');
            int index = int.Parse(s[0]);
            string pName = s[1];
            _players[index] = pName;
            _readyStates[index] = false;
        }


        private void ClearPlayerList() => PlayersGameObject.transform.Clear();
        private void RefreshList()
        {
            if (Dependencies.NumberOfPlayers <= 0) return;
            ClearPlayerList();
            for (int i = 0; i < Dependencies.NumberOfPlayers; i++)
            {
                if(!_players.ContainsKey(i)) CreateEmptyPlayer();
                else CreatePlayer(i);
            }
        }

        private void CreateEmptyPlayer()
        {
            Instantiate(Stuff.Prefabs.Find(p => p.name == "Empty Player"), PlayersGameObject.transform);

        }
        private void CreatePlayer(int i)
        {
            GameObject g = Instantiate(Stuff.Prefabs.Find(p => p.name == "Player Info"), PlayersGameObject.transform);
            g.transform.Find("Name").GetComponent<Text>().text = _players[i];
            g.transform.Find("Readiness").GetComponent<Text>().text = $"Ready: {(_readyStates[i] ? "<color=green>Yes</color>" : "<color=red>No</color>")}";
        }
    }
}
