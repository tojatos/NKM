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
	}

	public int PickType;
	public int SelectedMapIndex;
	public int NumberOfPlayers;
	public int NumberOfCharactersPerPlayer;
	public bool IsMuted;

	private void OnApplicationQuit()
	{
		PlayerPrefs.SetInt("PickType", PickType);
		PlayerPrefs.SetInt("SelectedMapIndex", SelectedMapIndex);
		PlayerPrefs.SetInt("NumberOfPlayers", NumberOfPlayers);
		PlayerPrefs.SetInt("NumberOfCharactersPerPlayer", NumberOfCharactersPerPlayer);

		PlayerPrefsX.SetBool("IsMuted", IsMuted);

	}
}