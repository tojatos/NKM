using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class MainMenu : MonoBehaviour
	{
		public void PlayButtonClick() => SceneManager.LoadScene(Scenes.PreGameOptions);
		public void OptionsButtonClick(){}//TODO: create options
		public void ExitButtonClick() => Application.Quit();
	}
}