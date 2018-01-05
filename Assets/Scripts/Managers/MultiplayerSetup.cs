using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
	public class MultiplayerSetup : SingletonMonoBehaviour<MultiplayerSetup>
	{
		[SerializeField]
		private Text _ipAddress;
		public void HostGameButtonClick()
		{
			//SceneManager.LoadScene(Scenes.HostGame);
		}
		public void JoinGameButtonClick()
		{
			//TODO: try connecting to address
		}
		public void BackButtonClick()
		{
			SceneManager.LoadScene(Scenes.GameTypeSelect);
		}
	}
}