using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
	public class PreGameOptions : MonoBehaviour
	{
		public GameObject Dropdowns;
		private static SessionSettings S => SessionSettings.Instance;
		private readonly List<Dropdown> _dropdowns = new List<Dropdown>();
		private Dropdown AddSessionSettingsDropdown(DropdownSettings settings)
		{
			Dropdown dropdown = Dropdowns.AddDropdownGroup(settings);
			_dropdowns.Add(dropdown);
			return dropdown;
		}

		private void Awake()
		{
			var pickTypeSettings = new DropdownSettings
			{
				Type = SettingType.PickType,
				Description = "Tryb wybierania postaci:",
				Options = new [] {"Blind", "Draft", "All random"},
			};
			var areBansEnabledSettings = new DropdownSettings
			{
				Type = SettingType.AreBansEnabled,
				Description = "Bany włączone:",
				Options = new[] {"Nie", "Tak"},
			};
			var mapSelectSettings = new DropdownSettings
			{
				Type = SettingType.SelectedMapIndex,
				Description = "Wybierz mapę:",
				Options = Stuff.Maps.Select(map => map.Name).ToArray()
			};
			HexMapScriptable selectedMapScriptable = Stuff.Maps[S.GetDropdownSetting(SettingType.SelectedMapIndex)];
			var numberOfPlayersSettings = new DropdownSettings
			{
				Type = SettingType.NumberOfPlayers,
				Description = "Liczba graczy:",
				Options = GetNumberOfPlayerStrings(selectedMapScriptable.MaxPlayers)
			};
			var numberOfCharacterPerPlayerSettings = new DropdownSettings
			{
				Type = SettingType.NumberOfCharactersPerPlayer,
				Description = "Liczba postaci na gracza:",
				Options = GetNumberOfCppStrings(selectedMapScriptable.MaxCharacters)
			};
			var bansNumberSettings = new DropdownSettings
			{
				Type = SettingType.BansNumber,
				Description = "Liczba banów na gracza",
				Options = GetNumberOfBansStrings()
			};
			AddSessionSettingsDropdown(pickTypeSettings);
			AddSessionSettingsDropdown(areBansEnabledSettings);
			AddSessionSettingsDropdown(bansNumberSettings);
			Dropdown mapSelectDropdown = AddSessionSettingsDropdown(mapSelectSettings);
			Dropdown numberOfPlayersDropdown = AddSessionSettingsDropdown(numberOfPlayersSettings);
			Dropdown numberOfCharacterPerPlayerDropdown = AddSessionSettingsDropdown(numberOfCharacterPerPlayerSettings);
			
			mapSelectDropdown.onValueChanged.AddListener(i => ReloadPlayerCountDropdown(i, numberOfPlayersDropdown));
			mapSelectDropdown.onValueChanged.AddListener(i => ReloadCppDropdown(i, numberOfCharacterPerPlayerDropdown));
			
			_dropdowns.ForEach(d => d.onValueChanged.AddListener(i => S.SetDropdownSetting(d.name, i)));
		}

		
		[UsedImplicitly]
		public void PlayButtonClick()
		{
//			_dropdowns.ForEach(d => S.DropdownSettings.Add(d.name, GetValueFromDropdown(d.name)));
//			SessionSettings.Instance.PickType = _pickTypeDropdown.value;
//			SessionSettings.Instance.SelectedMapIndex = _mapSelectDropdown.value;
//			SessionSettings.Instance.NumberOfPlayers = _playerCountDropdown.value + 1;
//			SessionSettings.Instance.NumberOfCharactersPerPlayer = _cppDropdown.value + 1;

			SceneManager.LoadScene(Scenes.MainGame);
		}
		[UsedImplicitly]
		public void BackButtonClick() => SceneManager.LoadScene(Scenes.MainMenu);

		private static void ReloadPlayerCountDropdown(int value, Dropdown playerCountDropdown)
		{
			int maxPlayers = Stuff.Maps[value].MaxPlayers;
//			playerCountDropdown.options = new List<Dropdown.OptionData>();
//			for (int i = 1; i <= maxPlayers; i++)
//			{
//				playerCountDropdown.options.Add(new Dropdown.OptionData(i.ToString()));
//			}
			playerCountDropdown.options = GetNumberOfPlayerStrings(maxPlayers).Select(x => new Dropdown.OptionData(x)).ToList();

			playerCountDropdown.value = 0;
			playerCountDropdown.RefreshShownValue();
		}

		private static string[] GetNumberOfPlayerStrings(int maxPlayers) =>
			Enumerable.Range(2, maxPlayers - 1).Select(x => x.ToString()).ToArray();//.Select(x => new Dropdown.OptionData(x.ToString())).ToList();
		
		private static string[] GetNumberOfCppStrings(int maxCharacters) =>
			Enumerable.Range(1, maxCharacters).Select(x => x.ToString()).ToArray();//.Select(x => new Dropdown.OptionData(x.ToString())).ToList();
		
		private static string[] GetNumberOfBansStrings() =>
			Enumerable.Range(1, 5).Select(x => x.ToString()).ToArray();//.Select(x => new Dropdown.OptionData(x.ToString())).ToList();
		private static void ReloadCppDropdown(int value, Dropdown cppDropdown)
		{
			int maxCharacters = Stuff.Maps[value].MaxCharacters;
			cppDropdown.options = GetNumberOfCppStrings(maxCharacters).Select(x => new Dropdown.OptionData(x)).ToList();

			cppDropdown.value = 0;
			cppDropdown.RefreshShownValue();
		}


	}
}