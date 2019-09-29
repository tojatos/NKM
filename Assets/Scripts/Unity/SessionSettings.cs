using System;
using System.Collections.Generic;
using NKMCore;
using NKMCore.Extensions;
using Unity.Hex;
using UnityEngine;

namespace Unity
{
    public class SessionSettings : CreatableSingletonMonoBehaviour<SessionSettings>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void DDOL() => DontDestroyOnLoad(Instance);

        public bool IsMuted;
        public string Nickname;
        public string SelectedIP;
        public string SelectedReplayFilePath;

        public List<string> PlayerNames = new List<string>();
        public GamePreparerDependencies Dependencies;

        //Do not add items to this list from the other classes
        private readonly Dictionary<string, int> _dropdownSettings = new Dictionary<string, int>();
        public int GetDropdownSetting(string type) => _dropdownSettings[type];
        public void SetDropdownSetting(string type, int value)
        {
            _dropdownSettings[type] = value;
            switch (type)
            {
                case SettingType.BansNumber:
                {
                    Dependencies.NumberOfBans = value + 1;
                } break;
                case SettingType.PickType:
                {
                    Dependencies.PickType = GetPickType(value);
                } break;
                case SettingType.SelectedMapIndex:
                {
                    Dependencies.HexMap = HexMapFactory.FromScriptable(Stuff.Maps[value]);
                } break;
                case SettingType.NumberOfPlayers:
                {
                    Dependencies.NumberOfPlayers = value + 2;

                } break;
                case SettingType.NumberOfCharactersPerPlayer:
                {
                    Dependencies.NumberOfCharactersPerPlayer = value + 1;

                } break;
                case SettingType.AreBansEnabled:
                {
                    Dependencies.BansEnabled = value == 1;
                } break;
            }
        }

        private static PickType GetPickType(int value)
        {
            switch (value)
            {
                case 0: return PickType.Blind;
                case 1: return PickType.Draft;
                case 2: return PickType.AllRandom;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void Awake()
        {
            IsMuted = PlayerPrefsX.GetBool("IsMuted", false);
            for (int i = 1; i <= 4; ++i)
                PlayerNames.Add(PlayerPrefs.GetString($"PlayerName{i}", $"Player {i}"));

            Nickname = PlayerPrefs.GetString("Nickname", "");
            SelectedIP = PlayerPrefs.GetString("SelectedIP");

            _enabledSettings.ForEach(AddDropdownSetting);
            UpdateEffects();

            string depsString = PlayerPrefs.GetString("Deps", string.Empty);
            Dependencies = depsString != string.Empty ? depsString.DeserializeGamePreparerDependencies() : new GamePreparerDependencies();

        }
        public void UpdateEffects()
        {
            if (GetDropdownSetting(SettingType.BackgroundEffectsEnabled) == 0)
                CreatableBackgroundCamera.DisableEffects();
            else
                CreatableBackgroundCamera.EnableEffects();
        }

        private void AddDropdownSetting(string settingType) =>
            AddDropdownSetting(settingType, 0);
        private void AddDropdownSetting(string settingType, int defaultValue) =>
            _dropdownSettings.Add(settingType, PlayerPrefs.GetInt(settingType, defaultValue));

        private void OnApplicationQuit()
        {
            foreach (KeyValuePair<string, int> keyValuePair in _dropdownSettings)
                PlayerPrefs.SetInt(keyValuePair.Key, keyValuePair.Value);

            PlayerPrefs.SetString("Nickname", Nickname);
            PlayerPrefs.SetString("SelectedIP", SelectedIP);

            PlayerPrefsX.SetBool("IsMuted", IsMuted);

            for (int i = 1; i <= 4; ++i)
                PlayerPrefs.SetString($"PlayerName{i}", PlayerNames[i-1]);

            PlayerPrefs.SetString("Deps", Dependencies.Serialize());
        }

        private readonly List<string> _enabledSettings = new List<string>
        {
            SettingType.AreBansEnabled,
            SettingType.NumberOfCharactersPerPlayer,
            SettingType.NumberOfPlayers,
            SettingType.PickType,
            SettingType.SelectedMapIndex,
            SettingType.BansNumber,
            SettingType.BackgroundEffectsEnabled,
        };
    }

    public static class SettingType
    {
        public const string PickType = "PickType";
        public const string SelectedMapIndex = "SelectedMapIndex";
        public const string NumberOfPlayers = "NumberOfPlayers";
        public const string NumberOfCharactersPerPlayer = "NumberOfCharactersPerPlayer";
        public const string AreBansEnabled = "AreBansEnabled";
        public const string BansNumber = "BansNumber";
        public const string BackgroundEffectsEnabled = "BackgroundEffectsEnabled";
        public const string GameType = "GameType";
    }
}