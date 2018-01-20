using System.Collections.Generic;
using Multiplayer.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
	public class ServerView : MonoBehaviour
	{
		[SerializeField] private GameObject PlayerList;
		[SerializeField] private GameObject ServerViewPlayerPrefab;

		[SerializeField] private Text NumberOfPlayers;
		[SerializeField] private Text SelectedMap;
		[SerializeField] private Text PlayersPerCharacter;

		public Button StartGameButton;
		public bool CanStart { get; set; }

		private Server ActiveServer;
		void Awake() => ActiveServer = FindObjectOfType<Server>();

		public void StartGame()
		{
			ActiveServer.TryStartingGame();
		}

		public void UpdatePlayers(List<Multiplayer.Network.Player> players)
		{
			PlayerList.transform.Clear();
			players.ForEach(player =>
			{
				var sPlayer = Instantiate(ServerViewPlayerPrefab, PlayerList.transform);
				sPlayer.GetComponentInChildren<Text>().text = player.Name;
				sPlayer.transform.Find("Disconnect Button").GetComponent<Button>().onClick.AddListener(() => ActiveServer.SendDisconnnectMessage(player.ConnectionID));
			});
		}

		public void UpdateGameOptions(List<string> options)
		{
			var numberOfPlayers = options[0];
			var selectedMap = options[1];
			var playersPerCharacter = options[2];
			NumberOfPlayers.text = numberOfPlayers;
			SelectedMap.text = Stuff.Maps[int.Parse(selectedMap)].Name;
			PlayersPerCharacter.text = playersPerCharacter;
		}

		private void Update()
		{
			StartGameButton.ToggleIf(!CanStart);
		}

	}
}