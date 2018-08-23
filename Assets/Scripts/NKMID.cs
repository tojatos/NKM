using System.Collections.Generic;

public static class NKMID
{
    private static readonly Dictionary<string, int> IdsTaken = new Dictionary<string, int>();

    public static int GetNext(string from)
    {
        if (IdsTaken.ContainsKey(from))
        {
            IdsTaken[from]++;
            return IdsTaken[from];
        }
        IdsTaken.Add(@from, 0);
        return 0;
    }
        
}