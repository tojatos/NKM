using UnityEngine;

/// <summary>
/// Use
/// 
/// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
///	static void DDOL() => DontDestroyOnLoad(Instance);
/// 
/// in every children.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CreatableSingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;
	public static T Instance => _instance ?? (_instance = new GameObject {name = typeof(T).ToString()}.AddComponent<T>().GetComponent<T>());
}