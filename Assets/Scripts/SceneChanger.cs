using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class SceneChanger : SingletonMonoBehaviour<SceneChanger>
{
	[UsedImplicitly]
	public void LoadGame()
	{
		SceneManager.LoadScene("StartGame");
	}
	[UsedImplicitly]
	public void LoadPreGameOptions()
	{
		SceneManager.LoadScene("PreGameOptions");
	}
	[UsedImplicitly]
	public void LoadMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
