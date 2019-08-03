using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Managers
{
	//Because unity is single-threaded, some calls must be delayed  
	public class AsyncCaller : CreatableSingletonMonoBehaviour<AsyncCaller>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void DDOL() => DontDestroyOnLoad(Instance);

		private readonly Queue<Action> _toInvoke = new Queue<Action>();

		private void Update()
		{
			if (_toInvoke.Count > 0)
				_toInvoke.Dequeue().Invoke();
		}

		public void Call(Action action) => _toInvoke.Enqueue(action);
	}
}
