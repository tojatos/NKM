using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class MainMenu : SingletonMonoBehaviour<MainMenu>
	{
		public void PlayButtonClick()
		{
			SceneManager.LoadScene(Scenes.GameTypeSelect);
		}
		public void OptionsButtonClick()
		{
		}
		public void ExitButtonClick()
		{
			Application.Quit();
		}
	}
}