﻿using System;
using System.Linq;
using NKMCore.Abilities.Monkey_D._Luffy;
using NKMCore.Abilities.Roronoa_Zoro;
using NKMCore.Templates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Managers
{
	public class MusicManager : CreatableSingletonMonoBehaviour<MusicManager>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void DDOL() => DontDestroyOnLoad(Instance);

		public AudioSource Music;
		private GameObject _muteButton;

		private void Awake()
		{
			Music = gameObject.AddComponent<AudioSource>();
			Music.playOnAwake = false;
			Music.clip = Resources.Load("Audio/mainmenu") as AudioClip;
			SceneManager.sceneLoaded += (scene, mode) => OnSceneLoaded();
		}
		private void OnSceneLoaded()
		{
			_muteButton = GameObject.Find("Mute Button");
			if (_muteButton == null) return;

			_muteButton.GetComponent<Image>().sprite = SessionSettings.Instance.IsMuted ? Stuff.Sprites.Icons.Find(i => i.name == "mute") : Stuff.Sprites.Icons.Find(i => i.name == "unmute");
			_muteButton.GetComponent<Button>().onClick.AddListener(ToggleMute);
		}
		private void Start()
		{
			if (!SessionSettings.Instance.IsMuted)
			{
				Music.Play();
			}
			else
			{
				if(_muteButton!=null)
					_muteButton.GetComponent<Image>().sprite = Stuff.Sprites.Icons.Find(i => i.name == "mute");
			}
		}

		private void ToggleMute()
		{
			if (SessionSettings.Instance.IsMuted)
			{
				_muteButton.GetComponent<Image>().sprite = Stuff.Sprites.Icons.FirstOrDefault(i => i.name == "unmute");
				Music.Play();
			}
			else
			{
				_muteButton.GetComponent<Image>().sprite = Stuff.Sprites.Icons.FirstOrDefault(i => i.name == "mute");
				Music.Stop();
			}
			SessionSettings.Instance.IsMuted = !SessionSettings.Instance.IsMuted;
		}
		
        public static void PlayAudio(string path, float volume = 0.8f)
        {
            try
            {
                var ac = Resources.Load("Audio/"+path) as AudioClip;
	            Debug.Log(ac);
                AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position, volume);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
            
        }

		public static void AddTriggers(Ability ability)
		{
			if (ability is GomuGomuNoMi)
			{
				var ab = (GomuGomuNoMi) ability;
				ab.OnClick += () => PlayAudio("gomu gomu no");
				ab.BeforePistol += () => PlayAudio("pistol");
				ab.BeforeRocket += () => PlayAudio("gomu gomu no rocket effect");
			}
			if (ability is HyakuHachiPoundHou) ((HyakuHachiPoundHou) ability).BeforeUse += () => PlayAudio(ability.Name);
			
		}

	}
}
