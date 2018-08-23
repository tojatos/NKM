using System;
using System.Linq;
using Extensions;

public class GameLog
{
    private readonly string[][] _parsedLogData; 
    public string[][] Actions { get; } 
    public GameLog(string[] data)
    {
        _parsedLogData = data.Select(f => f.Split(new []{": "}, 2, StringSplitOptions.RemoveEmptyEntries)).ToArray();
        Actions = GetActions();
    }

    private string GetFirst(string key) => _parsedLogData.GetFirst(key);
    private string[] GetFirstSplitted(string key) => GetFirst(key).SplitData();
    
    public string GetMapName() => GetFirst("MAP");
    public string[] GetPlayerNames() => GetFirstSplitted("PLAYERS");

    public string[] GetCharacterNames(string playerName) => GetFirstSplitted(playerName).Select(c => c.ConvertToNameWithoutID()).ToArray();

    private string[][] GetActions() => _parsedLogData.SkipWhile(x => x[0] != "GAME STARTED").Skip(1).ToArray();
}