using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class GameTypeSelect : SingletonMonoBehaviour<MainMenu>
	{
		public void SinglePlayerButtonClick()
		{
			SceneManager.LoadScene("Pre Game Options");
		}
		public void MultiPlayerButtonClick()
		{
		}
		public void BackButtonClick()
		{
			SceneManager.LoadScene("Main Menu");
		}
	}
}