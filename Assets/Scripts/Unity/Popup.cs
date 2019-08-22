using Unity.Extensions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Unity
{
    public class Popup : SingletonMonoBehaviour<Popup>
    {
		public Text Header;
		public Text Message;
		public Button AcceptMessageButton;

		public void ClosePopup() => gameObject.Hide();
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
    }
}