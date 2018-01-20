﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
	public class GameHostOptions : MonoBehaviour
	{
		[SerializeField] private GameObject _serverPrefab;
		[SerializeField] private GameObject _clientPrefab;

		private Dropdown _playerCountDropdown;
		private Dropdown _mapSelectDropdown;
		private Dropdown _cppDropdown;
		private void Awake()
		{
			_mapSelectDropdown = GameObject.Find("MapSelect").GetComponentInChildren<Dropdown>();
			_playerCountDropdown = GameObject.Find("PlayerCount").GetComponentInChildren<Dropdown>();
			_cppDropdown = GameObject.Find("CPPSelect").GetComponentInChildren<Dropdown>();

			_mapSelectDropdown.options = Stuff.Maps.Select(map => new Dropdown.OptionData(map.Name)).ToList();
			_mapSelectDropdown.onValueChanged.AddListener(ReloadPlayerCountDropdown);
			_mapSelectDropdown.onValueChanged.AddListener(ReloadCppDropdown);
			//TODO: Check if map exists, same with player number

			_mapSelectDropdown.value = PlayerPrefs.GetInt("SelectedMap", 0);

			ReloadPlayerCountDropdown(_mapSelectDropdown.value);
			_playerCountDropdown.value = PlayerPrefs.GetInt("NumberOfPlayers", 2) - 1;

			ReloadCppDropdown(_mapSelectDropdown.value);
			_cppDropdown.value = PlayerPrefs.GetInt("NumberOfCharactersPerPlayer", 1) - 1;
		}

		public void CreateGameButtonClick()
		{
			PlayerPrefs.SetInt("SelectedMap", _mapSelectDropdown.value);
			PlayerPrefs.SetInt("NumberOfPlayers", _playerCountDropdown.value + 1);
			PlayerPrefs.SetInt("NumberOfCharactersPerPlayer", _cppDropdown.value + 1);
			Instantiate(_serverPrefab);
			//var client = Instantiate(_clientPrefab).GetComponent<Client>();
			//client.Connect("127.0.0.1");
			SceneManager.LoadScene(Scenes.ServerView);
		}
		public void BackButtonClick() => SceneManager.LoadScene(Scenes.MultiPlayerSetup);

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