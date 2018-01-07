﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Managers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Multiplayer.Network
{
	public class Server : MonoBehaviour
	{
		void Awake() => DontDestroyOnLoad(this);
		#region NetworkingVariables
		private const int MAX_CONNECTION = 100;

		private int port = 5701;

		private int hostId;
		private int webHostId;

		private int reliableChannel;
		private int unreliableChannel;

		private bool isStarted = false;
		private byte error;
#endregion

		private List<Player> Players = new List<Player>();
		private int _selectedMap;
		private int _maxPlayers; 
		private int _playersPerCharacter;

		private void Start()
		{
			StartServer();

			_maxPlayers = PlayerPrefs.GetInt("NumberOfPlayers");
			_selectedMap = PlayerPrefs.GetInt("SelectedMap");
			_playersPerCharacter = PlayerPrefs.GetInt("NumberOfCharactersPerPlayer");
		}

		private void StartServer()
		{
			NetworkTransport.Init();
			ConnectionConfig cc = new ConnectionConfig();

			reliableChannel = cc.AddChannel(QosType.Reliable);
			unreliableChannel = cc.AddChannel(QosType.Unreliable);

			HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

			hostId = NetworkTransport.AddHost(topo, port, null);
			isStarted = true;
			Debug.Log("Server started!");
		}
		private void Update()
		{
			if (!isStarted) return;
			int recHostId;
			int connectionId;
			int channelId;
			byte[] recBuffer = new byte[1024];
			int bufferSize = 1024;
			int dataSize;
			byte error;
			NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
				bufferSize, out dataSize, out error);
			switch (recData)
			{
				case NetworkEventType.ConnectEvent:
					Debug.Log("Player " + connectionId + " has connected");
					break;
				case NetworkEventType.DataEvent:
					string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
					ReceiveMessage(connectionId, msg);
					//Debug.Log("Player " + connectionId + " has sent: " + msg);
					break;
				case NetworkEventType.DisconnectEvent:
					OnDisconnect(connectionId);
					Debug.Log("Player " + connectionId + " has disconnected");
					break;
			}
		}

		private void OnDisconnect(int connectionId)
		{
			Players.RemoveAll(p => p.ConnectionID == connectionId);
			UpdatePlayers();
		}

		private void ReceiveMessage(int connectionId, string msg)
		{
			List<string> messages = msg.Split('|').ToList();
			messages.ForEach(m =>
			{
				List<string> contents = m.Split('%').ToList();
				string header = contents[0];
				switch (header)
				{
					case "CONNECTED":
						AskForName(connectionId);
						break;
					case "NAMEIS":
						var name = contents[1];
						var player = CreatePlayer(connectionId, name);
						TryJoiningLobby(player);
						break;
					default:
						Debug.Log($"Undefined message: {m}");
						break;
				}
			});
		}

		private Player CreatePlayer(int connId, string name)
		{
			var player = new Player(connId, name);
			Players.Add(player);
			return player;
		}
		private void TryJoiningLobby(Player player)
		{
			if(Players.Count <= _maxPlayers)
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
		}

		private void SendGameOptions(int playerConnectionId)
		{
			//TODO
		}

		private void UpdatePlayers()
		{
			string msg = "PLAYERLIST%";
			Players.ForEach(p => msg += $"{p.Name}%");
			msg = msg.TrimEnd('%');
			SendToAllPlayers(msg, reliableChannel);
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
	}
}
