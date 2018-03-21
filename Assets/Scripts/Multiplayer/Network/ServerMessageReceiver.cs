using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Multiplayer.Network
{
	public class ServerMessageReceiver
	{
		private Server Server;
		private Game Game;

		public ServerMessageReceiver(Server server)
		{
			Server = server;
		}
		public void Receive(int connectionId, string messages)
		{
			List<string> messagesSplit = messages.Split('|').ToList();
			messagesSplit.ForEach(m=>ReceiveMessage(connectionId, m));
		}

		private void ReceiveMessage(int connectionId, string message)
		{
			Queue<string> contents = new Queue<string>(message.Split('%'));
			string header = contents.Dequeue();
			//			Debug.Log($"| Server received: {header}");
			Debug.LogWarning("Received: " + message);
			ReceiveMessage(connectionId, header, contents);
		}

		private void ReceiveMessage(int connectionId, string header, Queue<string> contents)
		{
			switch (header)
			{
				case "GET_GAMEPLAYERS":
					Server.SendGamePlayers(connectionId);
					break;
				case "ACTIVE_VAR_SET":
					Server.TrySettingActiveValue(connectionId, contents);
					break;
				case "TOUCH_CELL":
					Server.TouchCell(connectionId, contents);
					break;
				case "CHARACTERS":
					Server.ReceiveCharacters(Server.Players.Single(p => p.ConnectionID == connectionId), contents);
					break;
				case "CONNECTED":
					Server.AskForName(connectionId);
					break;
				case "ACTIVE_VAR_GET":
					Server.TryGettingSerializedActiveValue(connectionId, contents.Dequeue());
					break;
				case "NAMEIS":
					CreatePlayerAndTryToJoinLobby(connectionId, contents);
					break;
				default:
					Debug.Log($"Undefined message: {header}");
					contents.ToList().ForEach(Debug.Log);
					break;
			}
		}

		private void CreatePlayerAndTryToJoinLobby(int connectionId, Queue<string> contents)
		{
			var name = contents.Dequeue();
			var player = Server.CreatePlayer(connectionId, name);
			Server.TryJoiningLobby(player);
		}
	}
}
