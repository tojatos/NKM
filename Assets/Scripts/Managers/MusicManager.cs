using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
	public class MusicManager : MonoBehaviour
	{
		public AudioSource Music;
		private GameObject MuteButton;
		private bool _isMuted;
		private UnityAction<Scene, LoadSceneMode> _sceneLoadDelegate;
		private void Awake()
		{
			_sceneLoadDelegate = (scene, mode) => OnSceneLoaded();
			if (FindObjectsOfType<MusicManager>().Length > 1) Destroy(this); //makes singleton work when changing scenes
			_isMuted = PlayerPrefsX.GetBool("IsMuted", false);
		}
		void OnEnable()
		{
			SceneManager.sceneLoaded += _sceneLoadDelegate;
		}
		void OnDisable()
		{
			SceneManager.sceneLoaded -= _sceneLoadDelegate;
		}

		private void OnSceneLoaded()
		//private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			MuteButton = GameObject.Find("Mute Button");
			if (MuteButton == null) return;

			MuteButton.GetComponent<Image>().sprite = _isMuted ? Stuff.Sprites.Icons.Find(i => i.name == "mute") : Stuff.Sprites.Icons.Find(i => i.name == "unmute");
			MuteButton.GetComponent<Button>().onClick.AddListener(ToggleMute);
		}
		private void Start()
		{
			if (!_isMuted)
			{
				Music.Play();
			}
			else
			{
				MuteButton.GetComponent<Image>().sprite = Stuff.Sprites.Icons.Find(i => i.name == "mute");
			}
			DontDestroyOnLoad(this);
		}

		private void ToggleMute()
		{
			if (_isMuted)
			{
				MuteButton.GetComponent<Image>().sprite = Stuff.Sprites.Icons.Find(i => i.name == "unmute");
				Music.Play();
			}
			else
			{
				MuteButton.GetComponent<Image>().sprite = Stuff.Sprites.Icons.Find(i => i.name == "mute");
				Music.Stop();
			}
			_isMuted = !_isMuted;
		}

		private void OnApplicationQuit()
		{
			PlayerPrefsX.SetBool("IsMuted", _isMuted);
		}
	}
}