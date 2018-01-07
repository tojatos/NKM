using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
	public class Lobby : MonoBehaviour
	{
		[SerializeField] private GameObject PlayerList;
		[SerializeField] private GameObject LobbyPlayerPrefab;

		public void BackButtonClick() => SceneManager.LoadScene(Scenes.MultiPlayerSetup);
		public void UpdatePlayers(List<string> names)
		{
			PlayerList.transform.Clear();
			names.ForEach(n =>
			{
				var lPlayer = Instantiate(LobbyPlayerPrefab, PlayerList.transform);
				lPlayer.GetComponentInChildren<Text>().text = n;
			});
		}
	}
}