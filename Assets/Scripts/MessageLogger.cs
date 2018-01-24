using System;
using System.Collections;
using System.Linq;
using Managers;
using UIManagers;
using UnityEngine;
using UnityEngine.UI;

public class MessageLogger : SingletonMonoBehaviour<MessageLogger>
{

	public Text LogText;
	public Scrollbar Scrollbar;
	public InputField InputField;
	private Game Game;
	private int _linesCount = 4;

	void Awake()
	{
		Game = LocalGameStarter.Instance.Game;
	}
	public void Log(string text, bool endline = true)
	{
		LogText.text += text;
		LogText.text += endline ? "\n": "";
		FinishLogging();
	}

	public void DebugLog(string text, bool endline = true)
	{
		if (Game.Active.IsDebug)
		{
			LogText.text += "<b><color=red>" + text + "</color></b>";
			LogText.text += endline ? "\n" : "";
			FinishLogging();
		}
	}

	private int CountLines()
	{
		return LogText.text.Count(c => c == '\n');
	}
	private void ResizeIfNotEnoughSpace()
	{
		int trueLinesCount = CountLines();
		if (trueLinesCount > 4 && _linesCount != trueLinesCount)
		{
			int diff = trueLinesCount - _linesCount;
			LogText.rectTransform.offsetMin = new Vector2(LogText.rectTransform.offsetMin.x, LogText.rectTransform.offsetMin.y + diff * -14);
			_linesCount = trueLinesCount;
		}
	}
	private IEnumerator SetScrollbarToDown()
	{
		yield return null; // Waiting just one frame is probably good enough, yield return null does that

		Scrollbar.value = 0;
	}
	private void FinishLogging()
	{
		ResizeIfNotEnoughSpace();
		//ScrollDown();
		StartCoroutine(SetScrollbarToDown());
	}

	void Update()
	{
		if (!InputField.isFocused && Input.GetKeyDown(KeyCode.Slash))
		{
			InputField.ActivateInputField();
			InputField.text += "/";
			StartCoroutine(MoveTextEnd_NextFrame());
		}

	}
	IEnumerator MoveTextEnd_NextFrame()
	{
		yield return 0; // Skip the first frame in which this is called.

		InputField.MoveTextEnd(false); // Do this during the next frame.
	}

	private void OnGUI()
	{

		if (InputField.isFocused && InputField.text != "" && Input.GetKey(KeyCode.Return)) //TODO: enter does not work on android
		{
			var text = InputField.text;
			if (text[0] == '/')
			{
				text = text.Substring(1);
				if (text.StartsWith("set "))
				{
					text = text.Substring(4);
					if (Game.Active.CharacterOnMap != null)
					{
						if (text.StartsWith("hp "))
						{
							var value = text.Substring(3);
							Game.Active.CharacterOnMap.HealthPoints.Value = Int32.Parse(value);
							Game.Active.CharacterOnMap.RemoveIfDead();
						}
						else if(text.StartsWith("atk "))
						{
							var value = text.Substring(4);
							Game.Active.CharacterOnMap.AttackPoints.Value = Int32.Parse(value);
						}
						else if (text.StartsWith("phase "))
						{
							var value = text.Substring(6);
							Game.Active.Phase.Number = Int32.Parse(value);
						}
						else
						{
							DebugLog("<i>Nie ma takiej komendy jak </i>" + text + "<i>.</i>");
						}
						CharacterStats.Instance.UpdateCharacterStats(Game.Active.CharacterOnMap);
					}
					else
					{
						DebugLog("<i>Nie ma aktywnej postaci!</i>");
					}

				}
				else if (text.StartsWith("debug "))
				{
					var value = bool.Parse(text.Substring(6));
					Game.Active.IsDebug = value;

				}
				else if (text.StartsWith("cancel"))
				{
					Game.Active.Cancel();
				}
				else
				{
					DebugLog("<i>Nie ma takiej komendy jak </i>" + text + "<i>.</i>");
				}
			}
			else
			{
				Log("<b><</b>" + Game.Active.Player.Name + "<b>></b>: " + text);
			}
			InputField.text = "";
			InputField.ActivateInputField();
		}
	}
}