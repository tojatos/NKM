﻿using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
	public class PreGameOptions : MonoBehaviour
	{
		private Dropdown _pickTypeDropdown;
		private Dropdown _playerCountDropdown;
		private Dropdown _mapSelectDropdown;
		private Dropdown _cppDropdown;
		private void Awake()
		{
			_pickTypeDropdown = GameObject.Find("PickType").GetComponentInChildren<Dropdown>();
			_mapSelectDropdown = GameObject.Find("MapSelect").GetComponentInChildren<Dropdown>();
			_playerCountDropdown = GameObject.Find("PlayerCount").GetComponentInChildren<Dropdown>();
			_cppDropdown = GameObject.Find("CPPSelect").GetComponentInChildren<Dropdown>();

			_pickTypeDropdown.options = new List<Dropdown.OptionData>()
			{
				new Dropdown.OptionData("Blind"),
				new Dropdown.OptionData("Draft"),
			};
			
			_mapSelectDropdown.options = Stuff.Maps.Select(map => new Dropdown.OptionData(map.Name)).ToList();
			_mapSelectDropdown.onValueChanged.AddListener(ReloadPlayerCountDropdown);
			_mapSelectDropdown.onValueChanged.AddListener(ReloadCppDropdown);
			//TODO: Check if map exists, same with gamePlayer number

			_mapSelectDropdown.value = SessionSettings.Instance.SelectedMapIndex;
			ReloadPlayerCountDropdown(_mapSelectDropdown.value);
			_playerCountDropdown.value = SessionSettings.Instance.NumberOfPlayers - 1;

			ReloadCppDropdown(_mapSelectDropdown.value);
			_cppDropdown.value =
				SessionSettings.Instance.NumberOfCharactersPerPlayer - 1;
		}

		[UsedImplicitly]
		public void PlayButtonClick()
		{
			SessionSettings.Instance.PickType = _pickTypeDropdown.value;
			SessionSettings.Instance.SelectedMapIndex = _mapSelectDropdown.value;
			SessionSettings.Instance.NumberOfPlayers = _playerCountDropdown.value + 1;
			SessionSettings.Instance.NumberOfCharactersPerPlayer = _cppDropdown.value + 1;

			SceneManager.LoadScene(Scenes.MainGame);
		}
		[UsedImplicitly]
		public void BackButtonClick()
		{
			SceneManager.LoadScene(Scenes.MainMenu);
		}

		private void ReloadPlayerCountDropdown(int value)
		{
			var maxPlayers = Stuff.Maps[value].MaxPlayers;
			_playerCountDropdown.options = new List<Dropdown.OptionData>();
			for (var i = 1; i <= maxPlayers; i++)
			{
				_playerCountDropdown.options.Add(new Dropdown.OptionData(i.ToString()));
			}

			_playerCountDropdown.value = 0;
			_playerCountDropdown.RefreshShownValue();
		}
		private void ReloadCppDropdown(int value)
		{
			var maxCharacters = Stuff.Maps[value].MaxCharacters;
			_cppDropdown.options = new List<Dropdown.OptionData>();
			for (var i = 1; i <= maxCharacters; i++)
			{
				_cppDropdown.options.Add(new Dropdown.OptionData(i.ToString()));
			}

			_cppDropdown.value = 0;
			_cppDropdown.RefreshShownValue();
		}


	}
}