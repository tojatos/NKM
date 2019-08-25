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
        public void Awake() => DropdownsObject.transform.Clear();

        public Dropdown AddSessionSettingsDropdown(DropdownSettings settings)
        {
            Dropdown dropdown = DropdownsObject.AddDropdownGroup(settings);
            return dropdown;
        }
    }
}