using System.Linq;
using JetBrains.Annotations;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Managers
{
    public class Options : SingletonMonoBehaviour<Options>
    {
        public InputField PlayerName1;
        public InputField PlayerName2;
        public InputField PlayerName3;
        public InputField PlayerName4;

        private static SessionSettings S => SessionSettings.Instance;

        private void Awake()
        {
            GameObject mdObject = Instantiate(Stuff.Prefabs.First(p => p.name == "Multiple Dropdowns"), GameObject.Find("Handle").transform);
            var md = mdObject.GetComponent<MultipleDropdowns>();
            md.Title.gameObject.Hide();
            md.FinishSelectingButton.gameObject.Hide();
            Dropdown x = md.AddSessionSettingsDropdown(new DropdownSettings
            {
                Description = "Efekty w tle",
                Options = new[] {"Wyłączone", "Włączone"},
                Type = SettingType.BackgroundEffectsEnabled,
            });
            x.onValueChanged.AddListener(i => S.UpdateEffects());
            PlayerName1.text = S.PlayerNames[0];
            PlayerName2.text = S.PlayerNames[1];
            PlayerName3.text = S.PlayerNames[2];
            PlayerName4.text = S.PlayerNames[3];
        }

        public void Show() => gameObject.Show();

        [UsedImplicitly]
        public void SaveButtonClick()
        {
            S.PlayerNames[0] = PlayerName1.text;
            S.PlayerNames[1] = PlayerName2.text;
            S.PlayerNames[2] = PlayerName3.text;
            S.PlayerNames[3] = PlayerName4.text;
            gameObject.Hide();
        }
    }
}