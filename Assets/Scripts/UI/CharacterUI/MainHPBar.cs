using NKMObjects.Templates;
using UnityEngine.UI;

namespace UI.CharacterUI
{
	public class MainHPBar : SingletonMonoBehaviour<MainHPBar>
	{
		public Image HpAmount;

		public void UpdateHPAmount(Character character) => 
			HpAmount.fillAmount = character.HealthPoints.Value / (float) character.HealthPoints.BaseValue;
	}
}