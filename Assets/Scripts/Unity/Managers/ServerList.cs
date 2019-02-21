﻿using System.IO;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.Managers
{
	public class ServerList : SingletonMonoBehaviour<ServerList>
	{
		public InputField AddServerName;
		public InputField AddServerIP;
		public Button AddServerButton;
		public Button JoinServerButton;
		public string SelectedIP;
		public GameObject Servers;

		private Client _client;
		
		private void Awake()
		{
			AddServerButton.onClick.AddListener(()=> {
				if(AddServerName.text == "" || AddServerIP.text == "") return;
				AddServerInfoToFile(AddServerName.text, AddServerIP.text);
				CreateServerInfo(AddServerName.text, AddServerIP.text);
			});
			JoinServerButton.onClick.AddListener(() => TryToJoinAServer(SelectedIP));
			RefreshList();
			_client = new Client();
			_client.OnConnection += () => Debug.Log("Connected!");
			_client.OnDisconnect += () => Debug.Log("Disconnected!");
			_client.OnError += Debug.LogError;
			_client.OnMessage += message => Debug.Log($"Server: {message}");
			_client.OnConnection += () => _client.SendMessage("GREET");
		}

		private void Update()
		{
			JoinServerButton.ToggleIf(SelectedIP == "");
		}

		private void TryToJoinAServer(string selectedIP)
		{
			string[] ipInfo = selectedIP.Split(':');
			_client.TryConnecting(ipInfo[0], int.Parse(ipInfo[1]));
		}


		private void RefreshList()
		{
			Servers.transform.Clear();
//			string serverListFilePath = Directory.GetFiles(Application.persistentDataPath + Path.DirectorySeparatorChar + "Settings" + Path.DirectorySeparatorChar).FirstOrDefault(f => f == "server_list.txt");
			string serverListFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Settings" +
			                            Path.DirectorySeparatorChar + "server_list.txt";
//			if (serverListFilePath == null) return;
			if(!File.Exists(serverListFilePath)) return;
			string[] serverListLines = File.ReadAllLines(serverListFilePath);
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
			string serverListFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Settings" +
			                            Path.DirectorySeparatorChar + "server_list.txt";
			// ReSharper disable once AssignNullToNotNullAttribute
			Directory.CreateDirectory(Path.GetDirectoryName(serverListFilePath));	
			File.AppendAllLines(serverListFilePath, new []{serverName, ip, ""});
		}
	}
}