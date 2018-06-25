using UnityEngine;

public class SessionSettings : CreatableSingletonMonoBehaviour<SessionSettings>
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void DDOL() => DontDestroyOnLoad(Instance);

	private void Awake()
	{
		PickType = PlayerPrefs.GetInt("PickType", 0);
		SelectedMapIndex = PlayerPrefs.GetInt("SelectedMapIndex", 0);
		NumberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers", 2);
		NumberOfCharactersPerPlayer = PlayerPrefs.GetInt("NumberOfCharactersPerPlayer", 1);
		IsMuted = PlayerPrefsX.GetBool("IsMuted", false);
		PlayerName1 = PlayerPrefs.GetString("PlayerName1", "Player 1");
		PlayerName2 = PlayerPrefs.GetString("PlayerName2", "Player 2");
		PlayerName3 = PlayerPrefs.GetString("PlayerName3", "Player 3");
		PlayerName4 = PlayerPrefs.GetString("PlayerName4", "Player 4");
	}

	public int PickType;
	public int SelectedMapIndex;
	public int NumberOfPlayers;
	public int NumberOfCharactersPerPlayer;
	public bool IsMuted;
	public string PlayerName1;
	public string PlayerName2;
	public string PlayerName3;
	public string PlayerName4;

	private void OnApplicationQuit()
	{
		PlayerPrefs.SetInt("PickType", PickType);
		PlayerPrefs.SetInt("SelectedMapIndex", SelectedMapIndex);
		PlayerPrefs.SetInt("NumberOfPlayers", NumberOfPlayers);
		PlayerPrefs.SetInt("NumberOfCharactersPerPlayer", NumberOfCharactersPerPlayer);

		PlayerPrefsX.SetBool("IsMuted", IsMuted);
		
		PlayerPrefs.SetString("PlayerName1", PlayerName1);
		PlayerPrefs.SetString("PlayerName2", PlayerName2);
		PlayerPrefs.SetString("PlayerName3", PlayerName3);
		PlayerPrefs.SetString("PlayerName4", PlayerName4);

	}
}