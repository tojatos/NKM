using UnityEngine;

namespace Unity.Managers
{
	public class ClientManager : CreatableSingletonMonoBehaviour<ClientManager>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void DDOL() => DontDestroyOnLoad(Instance);

		public Client Client;

		private void Awake()
		{
			
			Client = new Client();
			Client.OnConnection += () => Debug.Log("Connected!");
			Client.OnDisconnect += () => Debug.Log("Disconnected!");
			Client.OnError += Debug.LogError;
			Client.OnMessage += message => Debug.Log($"Server: {message}");
			Client.OnConnection += () => Client.SendMessage("GREET");
		}

	}
}
