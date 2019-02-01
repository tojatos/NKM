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
//			if (new[] {Scenes.MainMenu, Scenes.PreGameOptions}.Contains(SceneManager.GetActiveScene().name))
			if (SceneManager.GetActiveScene().name != Scenes.MainGame)
			{
				if (Input.GetKeyDown(KeyCode.LeftArrow)) LoadLastScene();
				if (Input.GetKeyDown(KeyCode.RightArrow)) ClickActiveButton();
			}
		}
		
		private readonly Stack<string> _lastScenes = new Stack<string>();
		private void Awake() => SceneManager.sceneLoaded += (scene, mode) =>
		{
			_lastScenes.Push(scene.name);
			GameObject.FindGameObjectsWithTag("Back Button").ToList()
				.ForEach(b => b.GetComponent<Button>().onClick.AddListener(LoadLastScene));
		};

		private void LoadLastScene()
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
