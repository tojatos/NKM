using System.Linq;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Unity
{
    public class InputPopup : MonoBehaviour
    {
        public Text Header;
        public InputField Input;
        public Button AcceptMessageButton;

        public void ClosePopup() => Destroy(gameObject);
        private void ShowPopup() => gameObject.Show();

        // private void Start() => AcceptMessageButton.onClick.AddListener(ClosePopup);

        public void Show(string header, UnityAction onButtonClick = null)
        {
            Header.text = header;
            ShowPopup();
            if(onButtonClick != null)
                AcceptMessageButton.onClick.AddListener(onButtonClick);
        }

        public static InputPopup Create(Transform parentTransform)
        {
            GameObject popup = Instantiate(Stuff.Prefabs.Single(s => s.name == "Input Popup"), parentTransform);
            return popup.GetComponent<InputPopup>();
        }

    }
}