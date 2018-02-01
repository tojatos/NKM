using UnityEngine;

public class SessionSettings : CreatableSingletonMonoBehaviour<SessionSettings>
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void DDOL() => DontDestroyOnLoad(Instance);
	
	void Awake()
	{
		SelectedMapIndex = PlayerPrefs.GetInt("SelectedMapIndex", 0);
		NumberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers", 2);
		NumberOfCharactersPerPlayer = PlayerPrefs.GetInt("NumberOfCharactersPerPlayer", 1);
		GameType = GameType.Local;
		IsMuted = PlayerPrefsX.GetBool("IsMuted", false);
	}

	public int SelectedMapIndex { get; set; }
	public int NumberOfPlayers { get; set; }
	public int NumberOfCharactersPerPlayer { get; set; }
	public GameType GameType { get; set; }
	public bool IsMuted { get; set; }

	void OnApplicationQuit()
	{
		PlayerPrefs.SetInt("SelectedMapIndex", SelectedMapIndex);
		PlayerPrefs.SetInt("NumberOfPlayers", NumberOfPlayers);
		PlayerPrefs.SetInt("NumberOfCharactersPerPlayer", NumberOfCharactersPerPlayer);

		PlayerPrefsX.SetBool("IsMuted", IsMuted);

	}
}