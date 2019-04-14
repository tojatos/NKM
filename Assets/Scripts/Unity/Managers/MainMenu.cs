using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Managers
{
	public class MainMenu : MonoBehaviour
	{
		public Button LocalButton;
		public Button OnlineButton;
		public Button OptionsButton;
		public Button ExitButton;
		private void Awake()
		{
			LocalButton.onClick.AddListener(()=>
			{
//				SessionSettings.Instance.SetDropdownSetting(SettingType.GameType, 0);
				SceneManager.LoadScene(Scenes.PreGameOptions);
			});
			OnlineButton.onClick.AddListener(()=>
			{
//				SessionSettings.Instance.SetDropdownSetting(SettingType.GameType, 1);
				SceneManager.LoadScene(Scenes.ServerList);
			});
			OptionsButton.onClick.AddListener(Options.Instance.Show);
			ExitButton.onClick.AddListener(Application.Quit);
			
			LocalButton.Select();
		}
	}
}