using System.Collections.Generic;
using System.Linq;
using NKMCore;
using Unity.Extensions;
using Unity.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Unity
{
    public class ConsoleDrawer : SingletonMonoBehaviour<ConsoleDrawer>
    {
        private Console Console => GameStarter.Instance.Game.Console;
//    private readonly List<ConsoleLine> _loggedLines = new List<ConsoleLine>();
//    private List<ConsoleLine> NonDebugTexts => _loggedLines.FindAll(t => t.IsDebug == false);
        public Text LogText;
        public InputField InputField;
//    private static Game Game => GameStarter.Instance.Game;
//    private static Active Active => Game.Active;
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
            if (updateIndex) _startingIndex = Mathf.Clamp(Console.LoggedLines.Count - TextsDisplayed, 0, Console.LoggedLines.Count);
            string text = "";
            List<ConsoleLine> texts = _isDebug ? Console.LoggedLines : Console.NonDebugLines;
            for (int i = _startingIndex; i < _startingIndex + TextsDisplayed; i++)
            {
                if (texts.ElementAtOrDefault(i) == null) break;
                ConsoleLine lineToAdd = texts[i];
                if (lineToAdd.IsDebug) text += "<b><color=red>" + lineToAdd + "</color></b>\n";
                else text += lineToAdd + "\n";
            }

            LogText.text = text;
        }

        private void OnGUI()
        {
            if (!InputField.isFocused || InputField.text == "" || !Input.GetKey(KeyCode.Return)) return;
        
            Console.ExecuteCommand(InputField.text);

            UpdateLogText();
            InputField.text = "";
            InputField.ActivateInputField();
        }
    }
}
