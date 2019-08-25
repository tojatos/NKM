using System.Linq;
using UnityEngine;

namespace Unity
{
    public class CreatableBackgroundCamera : MonoBehaviour
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void DDOL() => DontDestroyOnLoad(Instance);

        private static CreatableBackgroundCamera _instance;
        public static CreatableBackgroundCamera Instance => _instance ? _instance : _instance = CreateGameObject().AddComponent<CreatableBackgroundCamera>().GetComponent<CreatableBackgroundCamera>();

        private static GameObject CreateGameObject() =>
            Instantiate(Stuff.Prefabs.Find(p => p.name == "Background Camera"));

        public static void DisableEffects() =>
            Instance.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(p =>
            {
                p.Stop();
                p.Clear();
            });

        public static void EnableEffects() =>
            Instance.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(p => p.Play());
    }
}