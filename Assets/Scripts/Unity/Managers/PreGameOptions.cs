using System.Linq;
using JetBrains.Annotations;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Managers
{
	public class PreGameOptions : MonoBehaviour
	{
		private GameObject _multipleDropdownsObject;
		public Transform WindowHandle;
		private static SessionSettings S => SessionSettings.Instance;

		private void Awake()
		{
			_multipleDropdownsObject = Instantiate(Stuff.Prefabs.First(s => s.name == "Multiple Dropdowns"), WindowHandle);
			var md = _multipleDropdownsObject.GetComponent<MultipleDropdowns>();
			md.FinishSelectingButton.onClick.AddListener(() => SceneManager.LoadScene(Scenes.MainGame));
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
			md.AddSessionSettingsDropdown(pickTypeSettings);
			md.AddSessionSettingsDropdown(areBansEnabledSettings);
			md.AddSessionSettingsDropdown(bansNumberSettings);
			Dropdown mapSelectDropdown = md.AddSessionSettingsDropdown(mapSelectSettings);
			Dropdown numberOfPlayersDropdown = md.AddSessionSettingsDropdown(numberOfPlayersSettings);
			Dropdown numberOfCharacterPerPlayerDropdown = md.AddSessionSettingsDropdown(numberOfCharacterPerPlayerSettings);

			mapSelectDropdown.onValueChanged.AddListener(i => ReloadPlayerCountDropdown(i, numberOfPlayersDropdown));
			mapSelectDropdown.onValueChanged.AddListener(i => ReloadCppDropdown(i, numberOfCharacterPerPlayerDropdown));

		}

		[UsedImplicitly]
		public void BackButtonClick() => SceneManager.LoadScene(Scenes.MainMenu);

		private static void ReloadPlayerCountDropdown(int value, Dropdown playerCountDropdown)
		{
			int maxPlayers = Stuff.Maps[value].MaxPlayers;
			playerCountDropdown.options = GetNumberOfPlayerStrings(maxPlayers).Select(x => new Dropdown.OptionData(x)).ToList();

			playerCountDropdown.value = 0;
			playerCountDropdown.RefreshShownValue();
		}

		private static string[] GetNumberOfPlayerStrings(int maxPlayers) =>
			Enumerable.Range(2, maxPlayers - 1).Select(x => x.ToString()).ToArray();

		private static string[] GetNumberOfCppStrings(int maxCharacters) =>
			Enumerable.Range(1, maxCharacters).Select(x => x.ToString()).ToArray();

		private static string[] GetNumberOfBansStrings() =>
			Enumerable.Range(1, 5).Select(x => x.ToString()).ToArray();
		private static void ReloadCppDropdown(int value, Dropdown cppDropdown)
		{
			int maxCharacters = Stuff.Maps[value].MaxCharacters;
			cppDropdown.options = GetNumberOfCppStrings(maxCharacters).Select(x => new Dropdown.OptionData(x)).ToList();

			cppDropdown.value = 0;
			cppDropdown.RefreshShownValue();
		}
	}
}
