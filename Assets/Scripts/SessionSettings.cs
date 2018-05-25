using UnityEngine;

public class SessionSettings : CreatableSingletonMonoBehaviour<SessionSettings>
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void DDOL() => DontDestroyOnLoad(Instance);

	private void Awake()
	{
		SelectedMapIndex = PlayerPrefs.GetInt("SelectedMapIndex", 0);
		NumberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers", 2);
		NumberOfCharactersPerPlayer = PlayerPrefs.GetInt("NumberOfCharactersPerPlayer", 1);
		IsMuted = PlayerPrefsX.GetBool("IsMuted", false);
	}

	public int SelectedMapIndex;
	public int NumberOfPlayers;
	public int NumberOfCharactersPerPlayer;
	public bool IsMuted;

	private void OnApplicationQuit()
	{
		PlayerPrefs.SetInt("SelectedMapIndex", SelectedMapIndex);
		PlayerPrefs.SetInt("NumberOfPlayers", NumberOfPlayers);
		PlayerPrefs.SetInt("NumberOfCharactersPerPlayer", NumberOfCharactersPerPlayer);

		PlayerPrefsX.SetBool("IsMuted", IsMuted);

	}
}