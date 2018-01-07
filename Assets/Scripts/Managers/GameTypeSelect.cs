using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class GameTypeSelect : MonoBehaviour
	{
		public void SinglePlayerButtonClick() => SceneManager.LoadScene(Scenes.PreGameOptions);
		public void MultiPlayerButtonClick() => SceneManager.LoadScene(Scenes.MultiPlayerSetup);
		public void BackButtonClick() => SceneManager.LoadScene(Scenes.MainMenu);
	}
}