using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
	public class MainMenu : MonoBehaviour
	{
		public Button SingleplayerButton;
		public Button MultiplayerButton;
		public Button OptionsButton;
		public Button ExitButton;
		private void Awake()
		{
			SingleplayerButton.onClick.AddListener(()=>
			{
				SessionSettings.Instance.SetDropdownSetting(SettingType.GameType, 0);
				SceneManager.LoadScene(Scenes.PreGameOptions);
			});
			MultiplayerButton.onClick.AddListener(()=>
			{
				return; //TODO
				SessionSettings.Instance.SetDropdownSetting(SettingType.GameType, 1);
				SceneManager.LoadScene(Scenes.PreGameOptions);
			});
			OptionsButton.onClick.AddListener(Options.Instance.Show);
			ExitButton.onClick.AddListener(Application.Quit);
			
			SingleplayerButton.Select();
		}
	}
}