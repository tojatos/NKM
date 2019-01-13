using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Extensions;
using Managers;
using NKMObjects.Templates;
using UnityEngine;
using UnityEngine.UI;

public class Console : SingletonMonoBehaviour<Console>
{
    private readonly List<ConsoleLine> _loggedLines = new List<ConsoleLine>();
    private List<ConsoleLine> NonDebugTexts => _loggedLines.FindAll(t => t.IsDebug == false);
    public Text LogText;
    public InputField InputField;
    private static Game Game => GameStarter.Instance.Game;
    private static Active Active => Game.Active;
    private const int TextsDisplayed = 8;

    private int _startingIndex;
    private bool _isDebug = true;

    public void Toggle()
    {
        if (gameObject.activeSelf) gameObject.Hide();
        else Show();
    }

    private void Show()
    {
        UpdateLogText();
        gameObject.Show();
        InputField.ActivateInputField();
    }

    private void UpdateLogText(bool updateIndex = true)
    {
        if (updateIndex) _startingIndex = Mathf.Clamp(_loggedLines.Count - TextsDisplayed, 0, _loggedLines.Count);
        string text = "";
        List<ConsoleLine> texts = _isDebug ? _loggedLines : NonDebugTexts;
        for (int i = _startingIndex; i < _startingIndex + TextsDisplayed; i++)
        {
            if (texts.ElementAtOrDefault(i) == null) break;
            ConsoleLine lineToAdd = texts[i];
            if (lineToAdd.IsDebug) text += "<b><color=red>" + lineToAdd + "</color></b>\n";
            else text += lineToAdd + "\n";
        }

        LogText.text = text;
    }

    public void Log(string text)
    {
        //		LogText.text += text;
        //		LogText.text += endline ? "\n": "";
        //		FinishLogging();
        _loggedLines.Add(new ConsoleLine
        {
            Text = text,
            IsDebug = false
        });
        UpdateLogText();
    }

    public void DebugLog(string text)
    {
        _loggedLines.Add(new ConsoleLine
        {
            Text = text,
            IsDebug = true
        });
        UpdateLogText();
        //		if (!Game.Active.IsDebug) return;
        //		LogText.text += "<b><color=red>" + text + "</color></b>";
        //		LogText.text += endline ? "\n" : "";
        //		FinishLogging();
    }

    public void GameLog(string text)
    {
        string path = Game.Options.LogFilePath;
        if (path == null) return;
        
        //Make sure target directory exists
        string directoryName = Path.GetDirectoryName(path);
        if(directoryName != null) Directory.CreateDirectory(directoryName);
        
        File.AppendAllText(path, text + '\n');
    }

    private void OnGUI()
    {
        if (!InputField.isFocused || InputField.text == "" || !Input.GetKey(KeyCode.Return)) return;
        string text = InputField.text;
        string[] arguments = text.Split(' ');

        if ((new[] { "set", "s" }).Contains(arguments[0]))
        {
            if ((new[] { "phase", "p" }).Contains(arguments[1])) Active.Phase.Number = int.Parse(arguments[2]);
            if ((new[] { "debug", "d" }).Contains(arguments[1])) bool.TryParse(arguments[2], out _isDebug);
            if ((new[] { "abilities", "ab" }).Contains(arguments[1]))
            {
                if ((new[] { "free", "f" }).Contains(arguments[2])) Game.Characters.FindAll(c => c.IsOnMap)
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
            if ((new[] { "gamepads", "g" }).Contains(arguments[1])) Log(string.Join("\n", Input.GetJoystickNames()));
            if ((new[] {"character", "c"}).Contains(arguments[1]))
            {
                if((new[] {"names", "n"}).Contains(arguments[2])) 
                    Game.Characters.Select(c => c.ToString()).ToList().ForEach(Log);
                if((new[] {"actionstate", "a"}).Contains(arguments[2])) 
                    Game.Characters.Select(c => c.ToString() + " " + c.TookActionInPhaseBefore.ToString()).ToList().ForEach(Log);
            }
            
        }
        else Log("<i>Nieznana komenda:</i> " + text);

        UpdateLogText();
        InputField.text = "";
        InputField.ActivateInputField();
    }
}

public class ConsoleLine
{
    public string Text;
    public bool IsDebug;
    public override string ToString() => Text;
}
