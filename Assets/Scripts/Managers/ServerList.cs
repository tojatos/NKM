using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
	public class ServerList : MonoBehaviour
	{
		public InputField AddServerName;
		public InputField AddServerIP;
		public Button AddServerButton;
		private void Awake()
		{
			AddServerButton.onClick.AddListener(()=> {
				SaveServerInfo();
				RefreshList(); 
			});
		}

		private void RefreshList()
		{
			throw new System.NotImplementedException();
		}

		private void SaveServerInfo()
		{
			throw new System.NotImplementedException();
		}
	}
}