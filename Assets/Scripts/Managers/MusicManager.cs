using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
	public class MusicManager : CreatableSingletonMonoBehaviour<MusicManager>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void DDOL() => DontDestroyOnLoad(Instance);

		public AudioSource Music;
		private GameObject MuteButton;

		private void Awake()
		{
			Music = gameObject.AddComponent<AudioSource>();
			Music.playOnAwake = false;
			Music.clip = Resources.Load("Audio/tobias_weber_-_The_Last_One_At_The_Bar_(Instrumental)") as AudioClip;
			SceneManager.sceneLoaded += (scene, mode) => OnSceneLoaded();
		}
		private void OnSceneLoaded()
		{
			MuteButton = GameObject.Find("Mute Button");
			if (MuteButton == null) return;

			MuteButton.GetComponent<Image>().sprite = SessionSettings.Instance.IsMuted ? Stuff.Sprites.Icons.Find(i => i.name == "mute") : Stuff.Sprites.Icons.Find(i => i.name == "unmute");
			MuteButton.GetComponent<Button>().onClick.AddListener(ToggleMute);
		}
		private void Start()
		{
			if (!SessionSettings.Instance.IsMuted)
			{
				Music.Play();
			}
			else
			{
				if(MuteButton!=null)
					MuteButton.GetComponent<Image>().sprite = Stuff.Sprites.Icons.Find(i => i.name == "mute");
			}
		}

		private void ToggleMute()
		{
			if (SessionSettings.Instance.IsMuted)
			{
				MuteButton.GetComponent<Image>().sprite = Stuff.Sprites.Icons.FirstOrDefault(i => i.name == "unmute");
				Music.Play();
			}
			else
			{
				MuteButton.GetComponent<Image>().sprite = Stuff.Sprites.Icons.FirstOrDefault(i => i.name == "mute");
				Music.Stop();
			}
			SessionSettings.Instance.IsMuted = !SessionSettings.Instance.IsMuted;
		}

	}
}