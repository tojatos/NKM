using System.Linq;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Unity
{
    public class Popup : MonoBehaviour
    {
        public Text Header;
        public Text Message;
        public Button AcceptMessageButton;

        public void ClosePopup() => Destroy(gameObject);
        private void ShowPopup() => gameObject.Show();

        private void Start() => AcceptMessageButton.onClick.AddListener(ClosePopup);

        public void Show(string header, string message, UnityAction onButtonClick = null)
        {
            Header.text = header;
            Message.text = message;
            ShowPopup();
            if(onButtonClick != null)
                AcceptMessageButton.onClick.AddListener(onButtonClick);
        }

        public static Popup Create(Transform parentTransform)
        {
            GameObject popup = Instantiate(Stuff.Prefabs.Single(s => s.name == "Popup"), parentTransform);
            return popup.GetComponent<Popup>();
        }

    }
}