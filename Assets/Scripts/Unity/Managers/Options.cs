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

	    public void Show()
	    {
		    gameObject.Show();
		    Debug.Log(S.PlayerName1);
		    PlayerName1.text = S.PlayerName1;
		    PlayerName2.text = S.PlayerName2;
		    PlayerName3.text = S.PlayerName3;
		    PlayerName4.text = S.PlayerName4;
	    }
	    
        [UsedImplicitly]
		public void SaveButtonClick()
        {
	        S.PlayerName1 = PlayerName1.text;
	        S.PlayerName2 = PlayerName2.text;
	        S.PlayerName3 = PlayerName3.text;
	        S.PlayerName4 = PlayerName4.text;
			gameObject.Hide();
		}
    }
}