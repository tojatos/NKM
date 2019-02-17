using System;
using System.Collections.Generic;

namespace NKMCore
{
    public class NKMRandom
    {
        private readonly Dictionary<string, int> _randoms = new Dictionary<string, int>();
        private readonly Random _random = new Random();
        
        public event Delegates.StringInt OnValueGet;

        /// <summary>
        /// Returns and removes rigged value from the dictionary if is set,
        /// otherwise returns a value between min [inclusive] and max [exclusive]
        /// </summary>
        public int Get(string name, int min, int max)
        {
            int rng = Get(name) ?? _random.Next(min, max);
            OnValueGet?.Invoke(name, rng);
            return rng;
        }

        /// <summary>
        /// Returns and removes rigged value from the dictionary if is set,
        /// otherwise returns null
        /// </summary>
        public int? Get(string name)
        {
            if (!_randoms.ContainsKey(name)) return null;
            int toReturn = _randoms[name];
            _randoms.Remove(name);
            return toReturn;
        }

        public void Set(string name, int value) => _randoms[name] = value;
    }
}