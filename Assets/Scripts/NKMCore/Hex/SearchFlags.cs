using System;

namespace NKMCore.Hex
{
    [Flags]
    public enum SearchFlags
    {
        None = 0,
        StopAtWalls = 1,
        StopAtEnemyCharacters = 2,
        StopAtFriendlyCharacters = 4,
        StraightLine = 8,
    }
}