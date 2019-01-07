using NKMObjects.Templates;
using UnityEngine.UI;

namespace UI.CharacterUI
{
	public class MainHPBar : SingletonMonoBehaviour<MainHPBar>
	{
		public Image HPamount;

		public void UpdateHPAmount(Character character) => 
			HPamount.fillAmount = character.HealthPoints.Value / (float) character.HealthPoints.BaseValue;
	}
}