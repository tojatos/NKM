using JetBrains.Annotations;
using Unity.Extensions;
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
		    PlayerName1.text = S.PlayerNames[0];
		    PlayerName2.text = S.PlayerNames[1];
		    PlayerName3.text = S.PlayerNames[2];
		    PlayerName4.text = S.PlayerNames[3];
	    }
	    
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