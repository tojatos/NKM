using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Managers
{
    public class MainMenu : MonoBehaviour
    {
        public Button LocalButton;
        public Button OnlineButton;
        public Button MapEditorButton;
        public Button OptionsButton;
        public Button ReplayButton;
        public Button ExitButton;
        private void Awake()
        {
            LocalButton.onClick.AddListener(() => SceneManager.LoadScene(Scenes.PreGameOptions));
            OnlineButton.onClick.AddListener(() => SceneManager.LoadScene(Scenes.ServerList));
            MapEditorButton.onClick.AddListener(() => SceneManager.LoadScene(Scenes.MapEditorOptions));
            ReplayButton.onClick.AddListener(() => SceneManager.LoadScene(Scenes.ReplaySelect));
            OptionsButton.onClick.AddListener(Options.Instance.Show);
            ExitButton.onClick.AddListener(Application.Quit);

            LocalButton.Select();
        }
    }
}