﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NKMCore;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;
using UnityEngine.Events;
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
		
		private GameOptions _options;
		private AsyncCaller _asyncCaller;
		private Client _client;
		private readonly Dictionary<int, GamePlayer> _players = new Dictionary<int, GamePlayer>();
		private int _numberOfPlayers;

		private void Start()
		{
			_asyncCaller = AsyncCaller.Instance;
			_client = ClientManager.Instance.Client;
			_options = new GameOptions {Players = new List<GamePlayer>()};

			Map.text = "";
			ClearPlayerList();
			
			GameObject.FindGameObjectsWithTag("Back Button").ToList()
				.ForEach(b => b.AddTrigger(EventTriggerType.PointerClick,
				() => _client.Disconnect()));
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
			_client.OnMessage += HandleMessageFromServerInMainThread;
			UnityAction<Scene, LoadSceneMode> removeMessageHandler = null;
			removeMessageHandler = (scene, mode) =>
			{
				_client.OnMessage -= HandleMessageFromServerInMainThread;
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

		private void HandleMessageFromServerInMainThread(string message) 
			=> _asyncCaller.Call(() => HandleMessageFromServer(message));

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
					RefreshList();
					break;
				case "PLAYERS":
					string[] playersNames = content.Split(';');
					for (int i = 0; i < playersNames.Length; ++i)
					{
						_players[i] = new GamePlayer {Name = playersNames[i]};
					}
					RefreshList();
					break;
				case "PLAYER_JOIN":
					HandlePlayerJoin(content);
					break;
				
				case "PLAYER_LEFT":
					int index = int.Parse(content);
					_players.Remove(index);
					RefreshList();
					break;
				case "MAPNAME":
					HandleMapname(content);
					break;
			}
		}

		private void HandleMapname(string content)
		{
			_options.HexMap = HexMapFactory.FromScriptable(Stuff.Maps.First(m => m.name == content));
			Map.text = content;
		}

		private void HandlePlayerJoin(string content)
		{
			string[] s = content.Split(';');
			int index = int.Parse(s[0]);
			string pName = s[1];
			_players[index] = new GamePlayer {Name = pName};
			RefreshList();
		}


		private void ClearPlayerList() => PlayersGameObject.transform.Clear();
		private void RefreshList()
		{
			ClearPlayerList();
			for (int i = 0; i < _numberOfPlayers; i++)
			{
				if(!_players.ContainsKey(i)) CreateEmptyPlayer();
				else CreatePlayer(_players[i]);
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
