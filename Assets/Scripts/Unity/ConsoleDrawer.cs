using System.Collections.Generic;
using System.Linq;
using NKMCore;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Unity
{
    public class ConsoleDrawer : SingletonMonoBehaviour<ConsoleDrawer>
    {
        private Console _console;
        public Text LogText;
        public InputField InputField;
        private const int TextsDisplayed = 8;

        private int _startingIndex;
        private bool _isDebug = true;

        public void Init(Console console)
        {
            _console = console;
        }

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
            if(_console==null) return;
            if (updateIndex) _startingIndex = Mathf.Clamp(_console.LoggedLines.Count - TextsDisplayed, 0, _console.LoggedLines.Count);
            string text = "";
            List<ConsoleLine> texts = _isDebug ? _console.LoggedLines : _console.NonDebugLines;
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
            if(_console==null) return;
            if (!InputField.isFocused || InputField.text == "" || !Input.GetKey(KeyCode.Return)) return;
        
            _console.ExecuteCommand(InputField.text);

            UpdateLogText();
            InputField.text = "";
            InputField.ActivateInputField();
        }
    }
}
