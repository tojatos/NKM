using System.Collections.Generic;
using Multiplayer.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
	public class ServerView : MonoBehaviour
	{
		[SerializeField] private GameObject PlayerList;
		[SerializeField] private GameObject LobbyPlayerPrefab;

		[SerializeField] private Text NumberOfPlayers;
		[SerializeField] private Text SelectedMap;
		[SerializeField] private Text PlayersPerCharacter;

		public Button StartGameButton;
		public bool CanStart { get; set; }

		private Server ActiveServer;
		void Awake() => ActiveServer = FindObjectOfType<Server>();

		public void StartGame()
		{

		}

		public void UpdatePlayers(List<string> names)
		{
			PlayerList.transform.Clear();
			names.ForEach(n =>
			{
				var lPlayer = Instantiate(LobbyPlayerPrefab, PlayerList.transform);
				lPlayer.GetComponentInChildren<Text>().text = n;
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