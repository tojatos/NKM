using UnityEngine;

namespace Unity
{
    public static class PlayerPrefsX
    {
        public static void SetBool(string name, bool booleanValue)
        {
            PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
        }

        private static bool GetBool(string name)
        {
            return PlayerPrefs.GetInt(name) == 1;
        }

        public static bool GetBool(string name, bool defaultValue)
        {
            return PlayerPrefs.HasKey(name) ? GetBool(name) : defaultValue;
        }
    }
}