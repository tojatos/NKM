using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NKMCore;
using NKMCore.Hex;
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
        private readonly List<Dropdown> _dropdowns = new List<Dropdown>();
        public Transform WindowHandle;
        private static SessionSettings S => SessionSettings.Instance;

        private void Awake()
        {
            _multipleDropdownsObject = Instantiate(Stuff.Prefabs.First(s => s.name == "Multiple Dropdowns"), WindowHandle);
            var md = _multipleDropdownsObject.GetComponent<MultipleDropdowns>();
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
            HexMap selectedMap = Stuff.Maps[S.GetDropdownSetting(SettingType.SelectedMapIndex)];
            var numberOfPlayersSettings = new DropdownSettings
            {
                Type = SettingType.NumberOfPlayers,
                Description = "Liczba graczy:",
                Options = GetNumberOfPlayerStrings(selectedMap.MaxPlayers)
            };
            var numberOfCharacterPerPlayerSettings = new DropdownSettings
            {
                Type = SettingType.NumberOfCharactersPerPlayer,
                Description = "Liczba postaci na gracza:",
                Options = GetNumberOfCppStrings(selectedMap.MaxCharactersPerPlayer)
            };
            var bansNumberSettings = new DropdownSettings
            {
                Type = SettingType.BansNumber,
                Description = "Liczba banów na gracza",
                Options = GetNumberOfBansStrings()
            };
            _dropdowns.Add(md.AddSessionSettingsDropdown(pickTypeSettings));
            _dropdowns.Add(md.AddSessionSettingsDropdown(areBansEnabledSettings));
            _dropdowns.Add(md.AddSessionSettingsDropdown(bansNumberSettings));
            Dropdown mapSelectDropdown = md.AddSessionSettingsDropdown(mapSelectSettings);
            Dropdown numberOfPlayersDropdown = md.AddSessionSettingsDropdown(numberOfPlayersSettings);
            Dropdown numberOfCharacterPerPlayerDropdown = md.AddSessionSettingsDropdown(numberOfCharacterPerPlayerSettings);
            _dropdowns.Add(mapSelectDropdown);
            _dropdowns.Add(numberOfPlayersDropdown);
            _dropdowns.Add(numberOfCharacterPerPlayerDropdown);

            mapSelectDropdown.onValueChanged.AddListener(i => ReloadPlayerCountDropdown(i, numberOfPlayersDropdown));
            mapSelectDropdown.onValueChanged.AddListener(i => ReloadCppDropdown(i, numberOfCharacterPerPlayerDropdown));
            var validator = new GamePreparerDependenciesValidator(S.Dependencies);

            md.FinishSelectingButton.onClick.AddListener(() =>
            {
                // force writing dropdown values into SessionSettings
                _dropdowns.ForEach(d => d.onValueChanged.Invoke(d.value));

                if (!validator.AreOptionsValid)
                {
                    Popup.Create(GameObject.Find("Canvas").transform).Show("Error", "Niepoprawne opcje");
                    return;
                }

                SceneManager.LoadScene(Scenes.MainGame);
            });
        }

        [UsedImplicitly]
        public void BackButtonClick() => SceneManager.LoadScene(Scenes.MainMenu);

        private static void ReloadPlayerCountDropdown(int value, Dropdown playerCountDropdown)
        {
            int maxPlayers = Stuff.Maps[value].MaxPlayers;
            playerCountDropdown.options = GetNumberOfPlayerStrings(maxPlayers).Select(x => new Dropdown.OptionData(x)).ToList();

            if(playerCountDropdown.value < maxPlayers - 1) return;

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
            int maxCharacters = Stuff.Maps[value].MaxCharactersPerPlayer;
            cppDropdown.options = GetNumberOfCppStrings(maxCharacters).Select(x => new Dropdown.OptionData(x)).ToList();

            if(cppDropdown.value < maxCharacters) return;

            cppDropdown.value = 0;
            cppDropdown.RefreshShownValue();
        }
    }
}
