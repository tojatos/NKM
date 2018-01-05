﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
	public class MultiplayerSetup : SingletonMonoBehaviour<MultiplayerSetup>
	{
		[SerializeField]
		private InputField _ipAddress;
		public void HostGameButtonClick() => SceneManager.LoadScene(Scenes.GameHostOptions);
		public void JoinGameButtonClick()
		{
			if (_ipAddress.text == "") _ipAddress.text = "127.0.0.1";
			Connect(_ipAddress.text);
		}
		public void BackButtonClick() => SceneManager.LoadScene(Scenes.GameTypeSelect);
		#region MoveThisLater

		private const int MAX_CONNECTION = 100;

		private int port = 5701;

		private int hostId;
		private int webHostId;

		private int reliableChannel;
		private int unreliableChannel;

		private int ourClientId;
		private int connectionID;
		private float connectionTime;

		private bool isConnected = false;
		private bool isStarted = false;
		private byte error;

		private string playerName;

		private void Connect(string ipAddress)
		{
			NetworkTransport.Init();
			ConnectionConfig cc = new ConnectionConfig();

			reliableChannel = cc.AddChannel(QosType.Reliable);
			unreliableChannel = cc.AddChannel(QosType.Unreliable);

			HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

			hostId = NetworkTransport.AddHost(topo, 0);

			connectionID = NetworkTransport.Connect(hostId, ipAddress, port, 0, out error);

			connectionTime = Time.time;
			isConnected = true;
		}
		private void Update()
		{
			if (!isConnected) return;
			int recHostId;
			int connectionId;
			int channelId;
			byte[] recBuffer = new byte[1024];
			int bufferSize = 1024;
			int dataSize;
			byte error;
			NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
			switch (recData)
			{
				case NetworkEventType.DataEvent:
					string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
					Debug.Log("Receiving: " + msg);
					break;
			}
		}
		#endregion
	}
}