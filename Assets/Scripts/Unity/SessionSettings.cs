using System.Collections.Generic;
using System.Linq;
using NKMCore;
using NKMCore.Extensions;
using UnityEngine;

namespace Unity
{
	public class SessionSettings : CreatableSingletonMonoBehaviour<SessionSettings>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void DDOL() => DontDestroyOnLoad(Instance);

		private void Awake()
		{
			IsMuted = PlayerPrefsX.GetBool("IsMuted", false);
			for (int i = 1; i <= 4; ++i)
				PlayerNames.Add(PlayerPrefs.GetString($"PlayerName{i}", $"Player {i}"));

            Nickname = PlayerPrefs.GetString("Nickname", "");
            SelectedIP = PlayerPrefs.GetString("SelectedIP");

            _enabledSettings.ForEach(AddDropdownSetting);
            UpdateEffects();
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

		public bool IsMuted;
		public string Nickname;
		public string SelectedIP;

        public List<string> PlayerNames = new List<string>();
		public GamePreparerDependencies Dependencies;

		//Do not add items to this list from the other classes
		private readonly Dictionary<string, int> _dropdownSettings = new Dictionary<string, int>();
		public int GetDropdownSetting(string type) => _dropdownSettings[type];
		public void SetDropdownSetting(string type, int value) => _dropdownSettings[type] = value;

		private void OnApplicationQuit()
		{
			foreach (KeyValuePair<string, int> keyValuePair in _dropdownSettings)
				PlayerPrefs.SetInt(keyValuePair.Key, keyValuePair.Value);

            PlayerPrefs.SetString("Nickname", Nickname);
            PlayerPrefs.SetString("SelectedIP", SelectedIP);

			PlayerPrefsX.SetBool("IsMuted", IsMuted);

			for (int i = 1; i <= 4; ++i)
				PlayerPrefs.SetString($"PlayerName{i}", PlayerNames[i-1]);
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
		public static string PickType => "PickType";
		public static string SelectedMapIndex => "SelectedMapIndex";
		public static string NumberOfPlayers => "NumberOfPlayers";
		public static string NumberOfCharactersPerPlayer => "NumberOfCharactersPerPlayer";
		public static string AreBansEnabled => "AreBansEnabled";
		public static string BansNumber => "BansNumber";
		public static string BackgroundEffectsEnabled => "BackgroundEffectsEnabled";
		public static string GameType => "GameType";
	}
}