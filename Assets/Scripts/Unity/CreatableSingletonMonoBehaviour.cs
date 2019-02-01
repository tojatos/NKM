using UnityEngine;

namespace Unity
{
	/// <inheritdoc />
	///  <summary>
	///  Use
	///  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	/// 	static void DDOL() =&gt; DontDestroyOnLoad(Instance);
	///  in every children.
	///  </summary>
	public abstract class CreatableSingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;
		public static T Instance => _instance ? _instance : (_instance = new GameObject {name = typeof(T).ToString()}.AddComponent<T>().GetComponent<T>());
	}
}