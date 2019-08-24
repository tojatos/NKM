using System.Collections.Generic;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Unity
{
    public class MultipleDropdowns : MonoBehaviour
    {
        public GameObject DropdownsObject;
        public Text Title;
        public Button FinishSelectingButton;
		private readonly List<Dropdown> _dropdowns = new List<Dropdown>();
		private static SessionSettings S => SessionSettings.Instance;
        public void Awake() => DropdownsObject.transform.Clear();

        public Dropdown AddSessionSettingsDropdown(DropdownSettings settings)
		{
			Dropdown dropdown = DropdownsObject.AddDropdownGroup(settings);
			dropdown.onValueChanged.AddListener(i => S.SetDropdownSetting(dropdown.name, i));
			_dropdowns.Add(dropdown);
			return dropdown;
		}
    }
}