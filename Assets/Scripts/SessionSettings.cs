using System.Collections.Generic;
using UnityEngine;

public class SessionSettings : CreatableSingletonMonoBehaviour<SessionSettings>
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void DDOL() => DontDestroyOnLoad(Instance);

	private void Awake()
	{
//		PickType = PlayerPrefs.GetInt("PickType", 0);
//		SelectedMapIndex = PlayerPrefs.GetInt("SelectedMapIndex", 0);
//		NumberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers", 2);
//		NumberOfCharactersPerPlayer = PlayerPrefs.GetInt("NumberOfCharactersPerPlayer", 1);
		IsMuted = PlayerPrefsX.GetBool("IsMuted", false);
		PlayerName1 = PlayerPrefs.GetString("PlayerName1", "Player 1");
		PlayerName2 = PlayerPrefs.GetString("PlayerName2", "Player 2");
		PlayerName3 = PlayerPrefs.GetString("PlayerName3", "Player 3");
		PlayerName4 = PlayerPrefs.GetString("PlayerName4", "Player 4");
		
		AddDropdownSetting(SettingType.AreBansEnabled);
		AddDropdownSetting(SettingType.NumberOfCharactersPerPlayer);
		AddDropdownSetting(SettingType.NumberOfPlayers);
		AddDropdownSetting(SettingType.PickType);
		AddDropdownSetting(SettingType.SelectedMapIndex);
		AddDropdownSetting(SettingType.BansNumber);
	}
	private void AddDropdownSetting(string settingType, int defaultValue = 0) =>
		_dropdownSettings.Add(settingType, PlayerPrefs.GetInt(settingType, defaultValue));

	public bool IsMuted;
	public string PlayerName1;
	public string PlayerName2;
	public string PlayerName3;
	public string PlayerName4;

	//Do not add items to this list from the other classes
	private readonly Dictionary<string, int> _dropdownSettings = new Dictionary<string, int>();
	public int GetDropdownSetting(string type) => _dropdownSettings[type];
	public void SetDropdownSetting(string type, int value) => _dropdownSettings[type] = value;

	private void OnApplicationQuit()
	{
		foreach (KeyValuePair<string, int> keyValuePair in _dropdownSettings)
		{
			PlayerPrefs.SetInt(keyValuePair.Key, keyValuePair.Value);
//			Debug.Log(keyValuePair.Key + " " + keyValuePair.Value);
		}
		
//		PlayerPrefs.SetInt("PickType", PickType);
//		PlayerPrefs.SetInt("SelectedMapIndex", SelectedMapIndex);
//		PlayerPrefs.SetInt("NumberOfPlayers", NumberOfPlayers);
//		PlayerPrefs.SetInt("NumberOfCharactersPerPlayer", NumberOfCharactersPerPlayer);
//		PlayerPrefsX.SetBool("AreBansEnabled", AreBansEnabled);

		PlayerPrefsX.SetBool("IsMuted", IsMuted);
		
		PlayerPrefs.SetString("PlayerName1", PlayerName1);
		PlayerPrefs.SetString("PlayerName2", PlayerName2);
		PlayerPrefs.SetString("PlayerName3", PlayerName3);
		PlayerPrefs.SetString("PlayerName4", PlayerName4);
	}
}

public static class SettingType
{
	public static string PickType => "PickType";
	public static string SelectedMapIndex => "SelectedMapIndex";
	public static string NumberOfPlayers => "NumberOfPlayers";
	public static string NumberOfCharactersPerPlayer => "NumberOfCharactersPerPlayer";
	public static string AreBansEnabled => "AreBansEnabled";
	public static string BansNumber => "BansNumber";
	public static string GameType => "GameType";
}