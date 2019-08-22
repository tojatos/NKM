using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Managers
{
	public class ShortcutManager : CreatableSingletonMonoBehaviour<ShortcutManager>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void DDOL() => DontDestroyOnLoad(Instance);

		private void Update()
		{
//			if (SceneManager.GetActiveScene().name != Scenes.MainGame)
//			{
//				if (Input.GetKeyDown(KeyCode.LeftArrow)) LoadLastScene();
//				if (Input.GetKeyDown(KeyCode.RightArrow)) ClickActiveButton();
//			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (SceneManager.GetActiveScene().name == Scenes.MainGame)
				{
					if(!Popup.Instance.gameObject.activeSelf)
                        Popup.Instance.Show("Wyjście", "Czy na pewno chcesz wyjść?", GameStarter.Quit);
					else
						Popup.Instance.ClosePopup();
				}
			}
		}

		private readonly Stack<string> _lastScenes = new Stack<string>();
		private void Awake() => SceneManager.sceneLoaded += (scene, mode) =>
		{
			_lastScenes.Push(scene.name);
			GameObject.FindGameObjectsWithTag("Back Button").ToList()
				.ForEach(b => b.GetComponent<Button>().onClick.AddListener(LoadLastScene));
		};

		public void LoadLastScene()
		{
			if(_lastScenes.Count <= 1) return;
			_lastScenes.Pop(); //Remove current scene name
			SceneManager.LoadScene(_lastScenes.Pop()); //Remove last scene name and load it
		}

		private static void ClickActiveButton()
		{
			GameObject g = EventSystem.current.currentSelectedGameObject;
            if(g!=null) g.GetComponent<Button>()?.onClick.Invoke();
		}
	}
}
