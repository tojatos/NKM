﻿using System;
using System.Collections.Generic;

namespace NKMCore
{
    public static class NKMRandom
    {
        private static readonly Dictionary<string, int> Randoms = new Dictionary<string, int>();
        public delegate void VoidDelegate(string name, int value);
        public static event VoidDelegate OnValueGet;
        private static readonly Random Random = new Random();

        /// <summary>
        /// Returns and removes rigged value from the dictionary if is set,
        /// otherwise returns a value between min [inclusive] and max [exclusive]
        /// </summary>
        public static int Get(string name, int min, int max)
        {
            int rng = Get(name) ?? Random.Next(min, max);
            OnValueGet?.Invoke(name, rng);
            return rng;
        }

        /// <summary>
        /// Returns and removes rigged value from the dictionary if is set,
        /// otherwise returns null
        /// </summary>
        public static int? Get(string name)
        {
            if (!Randoms.ContainsKey(name)) return null;
            int toReturn = Randoms[name];
            Randoms.Remove(name);
            return toReturn;
        }

        public static void Set(string name, int value) => Randoms[name] = value;
    }
}