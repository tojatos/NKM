using System;
using System.Collections.Generic;
using NKMCore;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Action = System.Action;

namespace Unity.Managers
{
	public class ServerLobby : SingletonMonoBehaviour<ServerLobby>
	{
		public Button ReadyButton;
		public GameObject PlayersGameObject;
		private GameOptions _options;
		private AsyncCaller AsyncCaller;
		private Client _client;
		private Dictionary<int, GamePlayer> Players = new Dictionary<int, GamePlayer>();
		private int _numberOfPlayers = 1;

		private void Start()
		{
			AsyncCaller = AsyncCaller.Instance;
			_client = ClientManager.Instance.Client;
			_options = new GameOptions {Players = new List<GamePlayer>()};
			ClearPlayerList();
			
			InitializeMessageHandler();
			ChangeSceneOnDisconnect();
			AskServerForGameInfo();
		}

		private void AskServerForGameInfo()
		{
			_client.SendMessage("GAME_INFO");
		}

		private void InitializeMessageHandler()
		{
			_client.OnMessage += HandleMessageFromServer;
			UnityAction<Scene, LoadSceneMode> removeMessageHandler = null;
			removeMessageHandler = (scene, mode) =>
			{
				_client.OnMessage -= HandleMessageFromServer;
				SceneManager.sceneLoaded -= removeMessageHandler;
			};
			SceneManager.sceneLoaded += removeMessageHandler;
		}
		
		private void ChangeSceneOnDisconnect()
		{
			Delegates.Void onDisconnect = () => ShortcutManager.Instance.LoadLastScene();
			_client.OnDisconnect += onDisconnect;
			UnityAction<Scene, LoadSceneMode> removeTrigger = null;
			removeTrigger = (scene, mode) =>
			{
				_client.OnDisconnect -= onDisconnect;
				SceneManager.sceneLoaded -= removeTrigger;
			};
			SceneManager.sceneLoaded += removeTrigger;
		}

		private void HandleMessageFromServer(string message)
		{
			string[] data = message.Split(' ');
			string header = data[0];
            string content = string.Empty;
			if(data.Length > 1) content = data[1];
			switch (header)
			{
				case "PLAYER_NUM":
					_numberOfPlayers = int.Parse(content);
					AsyncCaller.Call(RefreshList);
					break;
				case "PLAYERS":
					string[] playersNames = content.Split(';');
					for (int i = 0; i < playersNames.Length; ++i)
					{
						Players[i] = new GamePlayer {Name = playersNames[i]};
					}
					AsyncCaller.Call(RefreshList);
					break;
			}
		}



		private void ClearPlayerList() => PlayersGameObject.transform.Clear();
		private void RefreshList()
		{
			ClearPlayerList();
			for (int i = 0; i < _numberOfPlayers; i++)
			{
				if(!Players.ContainsKey(i)) CreateEmptyPlayer();
				else CreatePlayer(Players[i]);
			}
		}

		private void CreateEmptyPlayer()
		{
			Instantiate(Stuff.Prefabs.Find(p => p.name == "Empty Player"), PlayersGameObject.transform);
			
		}
		private void CreatePlayer(GamePlayer player)
		{
			GameObject g = Instantiate(Stuff.Prefabs.Find(p => p.name == "Player Info"), PlayersGameObject.transform);
			g.transform.Find("Name").GetComponent<Text>().text = player.Name;
			g.transform.Find("Readiness").GetComponent<Text>().text = "Ready: <color=red>No</color>";
		}
	}
}
