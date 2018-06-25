using Extensions;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace Managers
{
    public class Options : SingletonMonoBehaviour<Options>
    {
	    public Text PlayerName1;
	    public Text PlayerName2;
	    public Text PlayerName3;
	    public Text PlayerName4;

	    private static SessionSettings S => SessionSettings.Instance;

	    public void Show()
	    {
		    gameObject.Show();
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