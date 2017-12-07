using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class MainMenu : SingletonMonoBehaviour<MainMenu>
	{
		public void PlayButtonClick()
		{
			SceneManager.LoadScene("Pre Game Options");
		}
		public void OptionsButtonClick()
		{
			//Music.Stop();
			//Music.PlayDelayed(4f);
		}
		public void ExitButtonClick()
		{
			Application.Quit();
		}
	}
}