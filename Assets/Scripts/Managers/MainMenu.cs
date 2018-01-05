using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class MainMenu : SingletonMonoBehaviour<MainMenu>
	{
		public void PlayButtonClick() => SceneManager.LoadScene(Scenes.GameTypeSelect);
		public void OptionsButtonClick(){}//TODO: create options
		public void ExitButtonClick() => Application.Quit();
	}
}