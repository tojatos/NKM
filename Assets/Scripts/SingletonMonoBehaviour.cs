using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;
	public static T Instance => _instance ? _instance : (_instance = (T) Resources.FindObjectsOfTypeAll(typeof(T))[0]);
}