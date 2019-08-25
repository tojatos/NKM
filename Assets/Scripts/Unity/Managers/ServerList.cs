using System.IO;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Managers
{
    public class ServerList : SingletonMonoBehaviour<ServerList>
    {

        private static string _settingsDir;
        private static string _serverListFilePath;
        public InputField AddServerName;
        public InputField AddServerIP;
        public InputField Nickname;
        public Button AddServerButton;
        public Button JoinServerButton;
        private static string SelectedIP
        {
            get => SessionSettings.Instance.SelectedIP;
            set => SessionSettings.Instance.SelectedIP = value;
        }

        public GameObject Servers;
        private Client _client;

        private void Start()
        {
            _settingsDir = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}Settings";
            _serverListFilePath = $"{_settingsDir}{Path.DirectorySeparatorChar}server_list.txt";
            Nickname.text = SessionSettings.Instance.Nickname;

            _client = ClientManager.Instance.Client;

            AddServerButton.onClick.AddListener(()=> {
                if(AddServerName.text == "" || AddServerIP.text == "") return;
                AddServerInfoToFile(AddServerName.text, AddServerIP.text);
                CreateServerInfo(AddServerName.text, AddServerIP.text);
            });
            JoinServerButton.onClick.AddListener(() =>
            {
                if(Nickname.text == "") return;
                TryToJoinAServer(SelectedIP);
            });

            RefreshList();
        }

        private static void ShowServerMessage(string msg) => Popup.Instance.Show("Server", msg);

        public void HandleMessageFromServer(string message)
        {
            string[] data = message.Split(new []{' '}, 2);
            string header = data[0];
            string content = string.Empty;
            if(data.Length > 1) content = data[1];
            switch (header)
            {
                case "TOO_MANY_PLAYERS":
                    ShowServerMessage("Too many players!");
                    _client.Disconnect();
                    break;
                case "GET_NICKNAME":
                    _client.SendMessage($"NICKNAME {Nickname.text}");
                    break;
                case "JOIN":
                    JoinLobby();
                    break;
                case "REJECT":
                    ShowServerMessage(content);
                    _client.Disconnect();
                    break;
            }
        }

        private void JoinLobby()
        {
            SessionSettings.Instance.Nickname = Nickname.text;
            SceneManager.LoadScene(Scenes.ServerLobby);
        }

        private void Update() => JoinServerButton.ToggleIf(SelectedIP == "");

        private static void TryToJoinAServer(string selectedIP)
        {
            string[] ipInfo = selectedIP.Split(':');
            string hostname = ipInfo[0];
            int port = ipInfo.Length < 2 ? 30000 : int.Parse(ipInfo[1]);
            ClientManager.Instance.Client.TryConnecting(hostname, port);
        }

        private void RefreshList()
        {
            Servers.transform.Clear();
            if(!File.Exists(_serverListFilePath)) return;
            string[] serverListLines = File.ReadAllLines(_serverListFilePath);
            string n = "";
            foreach (string line in serverListLines)
            {
                if(line == "") continue;
                if (n == "") n = line;
                else
                {
                    CreateServerInfo(n, line);
                    n = "";
                }
            }
        }

        private void CreateServerInfo(string serverName, string ip)
        {
            GameObject g = Instantiate(Stuff.Prefabs.Find(p => p.name == "Server Info"), Servers.transform);
            g.AddTrigger(EventTriggerType.PointerClick, () => SelectedIP = ip);
            g.transform.Find("Name").GetComponent<Text>().text = serverName;
            g.transform.Find("IP").GetComponent<Text>().text = ip;
        }

        private static void AddServerInfoToFile(string serverName, string ip)
        {
            Directory.CreateDirectory(_settingsDir);
            File.AppendAllLines(_serverListFilePath, new []{serverName, ip, ""});
        }
    }
}