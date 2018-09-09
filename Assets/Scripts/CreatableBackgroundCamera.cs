using UnityEngine;

public class CreatableBackgroundCamera : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void DDOL() => DontDestroyOnLoad(ThisObject);

	private static CreatableBackgroundCamera ThisObject => 
		Instantiate(Stuff.Prefabs.Find(p => p.name == "Background Camera"))
			.AddComponent<CreatableBackgroundCamera>()
			.GetComponent<CreatableBackgroundCamera>();
}