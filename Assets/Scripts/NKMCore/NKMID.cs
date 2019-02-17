using System.Collections.Generic;

namespace NKMCore
{
    public static class NKMID
    {
        private static readonly Dictionary<string, uint> IdsTaken = new Dictionary<string, uint>();

        public static uint GetNext(string from)
        {
            if (IdsTaken.ContainsKey(from))
            {
                IdsTaken[from]++;
                return IdsTaken[from];
            }
            IdsTaken.Add(from, 0);
            return 0;
        }
        
    }
}