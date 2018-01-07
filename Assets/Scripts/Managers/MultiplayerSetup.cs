using Multiplayer.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
	public class MultiplayerSetup : MonoBehaviour
	{
		[SerializeField] private InputField _ipAddress;
		[SerializeField] private GameObject _clientPrefab;

		public void HostGameButtonClick() => SceneManager.LoadScene(Scenes.GameHostOptions);
		public void JoinGameButtonClick()
		{
			if (_ipAddress.text == "") _ipAddress.text = "127.0.0.1";

			var Client = Instantiate(_clientPrefab).GetComponent<Client>();
			Client.Connect(_ipAddress.text);
		}
		public void BackButtonClick() => SceneManager.LoadScene(Scenes.GameTypeSelect);
	}
}