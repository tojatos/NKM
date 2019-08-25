using System;
using System.Linq;
using NKMCore.Abilities.Monkey_D._Luffy;
using NKMCore.Abilities.Roronoa_Zoro;
using NKMCore.Templates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
                Unmute();
            else
                Mute();

            SessionSettings.Instance.IsMuted = !SessionSettings.Instance.IsMuted;
        }

        private void Mute()
        {
            _muteButton.GetComponent<Image>().sprite = Stuff.Sprites.Icons.FirstOrDefault(i => i.name == "mute");
            Music.Stop();
        }

        private void Unmute()
        {
            _muteButton.GetComponent<Image>().sprite = Stuff.Sprites.Icons.FirstOrDefault(i => i.name == "unmute");
            Music.Play();
        }

        private static void PlayAudio(string path, float volume = 0.8f)
        {
            try
            {
                var ac = Resources.Load("Audio/"+path) as AudioClip;
                if (Camera.main != null) AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position, volume);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }

        }

        public static void AddTriggers(Ability ability)
        {
            switch (ability)
            {
                case GomuGomuNoMi gomuGomuNoMi:
                {
                    gomuGomuNoMi.OnClick += () => PlayAudio("gomu gomu no");
                    gomuGomuNoMi.BeforePistol += () => PlayAudio("pistol");
                    gomuGomuNoMi.BeforeRocket += () => PlayAudio("gomu gomu no rocket effect");
                } break;

                case HyakuHachiPoundHou hyakuHachiPoundHou:
                {
                    hyakuHachiPoundHou.BeforeUse += () => PlayAudio(ability.Name);
                } break;
                case LackOfOrientation lackOfOrientation:
                {
                    lackOfOrientation.AfterGettingLost += () => PlayAudio("op wtf " + Random.Range(1, 4));
                } break;
                case OniGiri oniGiri:
                {
                    oniGiri.AfterOniGiriPrepare += list => PlayAudio("oni");
                    oniGiri.AfterOniGiri += () => PlayAudio("giri");
                } break;
            }
        }

    }
}
