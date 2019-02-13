using System.Collections.Generic;
using System.IO;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore
{
    public class Console
    {
        public readonly List<ConsoleLine> LoggedLines = new List<ConsoleLine>();
        public List<ConsoleLine> NonDebugLines => LoggedLines.FindAll(t => t.IsDebug == false);
    
        private readonly Game _game;
        private Active Active => _game.Active;

        public Console(Game game)
        {
            _game = game;
        }

        private bool _isDebug = true;

        public void Log(string text) => AddLog(text);
        public void DebugLog(string text) => AddLog(text, true);
        private void AddLog(string text, bool isDebug = false)
        {
            LoggedLines.Add(new ConsoleLine
            {
                Text = text,
                IsDebug = isDebug,
            });
        }
        public void GameLog(string text)
        {
            string path = _game.Options.LogFilePath;
            if (path == null) return;
        
            //Make sure target directory exists
            string directoryName = Path.GetDirectoryName(path);
            if(directoryName != null) Directory.CreateDirectory(directoryName);
        
            File.AppendAllText(path, text + '\n');
        }
        public void ExecuteCommand(string text)
        {
            string[] arguments = text.Split(' ');
            if(arguments.Length == 0) return; //TODO: check for arguments below to avoid IndexOutOfRange, maybe use a library?

            if ((new[] { "set", "s" }).Contains(arguments[0]))
            {
                if ((new[] { "phase", "p" }).Contains(arguments[1])) Active.Phase.Number = int.Parse(arguments[2]);
                if ((new[] { "debug", "d" }).Contains(arguments[1])) bool.TryParse(arguments[2], out _isDebug);
                if ((new[] { "abilities", "ab" }).Contains(arguments[1]))
                {
                    if ((new[] { "free", "f" }).Contains(arguments[2])) _game.Characters.FindAll(c => c.IsOnMap)
                        .ForEach(c => c.Abilities.ForEach(a => a.CurrentCooldown = 0));
                }
                if (Active.Character == null) return;
                if ((new[] { "hp", "h" }).Contains(arguments[1])) Active.Character.HealthPoints.Value = int.Parse(arguments[2]);
                if ((new[] { "atk", "at", "a" }).Contains(arguments[1])) Active.Character.AttackPoints.Value = int.Parse(arguments[2]);
                if ((new[] { "speed", "sp", "s" }).Contains(arguments[1])) Active.Character.Speed.Value = int.Parse(arguments[2]);
                if ((new[] { "range", "rang", "r" }).Contains(arguments[1])) Active.Character.BasicAttackRange.Value = int.Parse(arguments[2]);
                if ((new[] { "shield", "sh" }).Contains(arguments[1])) Active.Character.Shield.Value = int.Parse(arguments[2]);

            }
            else if ((new[] {"get", "g"}).Contains(arguments[0]))
            {
                if ((new[] {"character", "c"}).Contains(arguments[1]))
                {
                    if((new[] {"names", "n"}).Contains(arguments[2])) 
                        _game.Characters.Select(c => c.ToString()).ToList().ForEach(Log);
                    if((new[] {"actionstate", "a"}).Contains(arguments[2])) 
                        _game.Characters.Select(c => c.ToString() + " " + c.TookActionInPhaseBefore.ToString()).ToList().ForEach(Log);
                }
            
            }
            else Log("Nieznana komenda: " + text);
        }

        public void AddTriggersToEvents(Character character)
        {
            character.JustBeforeFirstAction += () => GameLog($"ACTION TAKEN: {character}");
            character.AfterAttack += (targetCharacter, damage) => Log(
                $"{character.FormattedFirstName()} atakuje {targetCharacter.FormattedFirstName()}, zadając <color=red><b>{damage.Value}</b></color> obrażeń!");
            character.AfterHeal += (targetCharacter, value) =>
                Log(targetCharacter != character
                    ? $"{character.FormattedFirstName()} ulecza {targetCharacter.FormattedFirstName()} o <color=blue><b>{value}</b></color> punktów życia!"
                    : $"{character.FormattedFirstName()} ulecza się o <color=blue><b>{value}</b></color> punktów życia!");
        
            character.OnDeath += () => Log($"{character.FormattedFirstName()} umiera!");
            character.AfterBasicMove += moveCells => 
                GameLog($"MOVE: {string.Join("; ", moveCells.Select(p => p.Coordinates))}"); //logging after action to make reading rng work
        
        }
    }

    public class ConsoleLine
    {
        public string Text;
        public bool IsDebug;
        public override string ToString() => Text;
    }
}