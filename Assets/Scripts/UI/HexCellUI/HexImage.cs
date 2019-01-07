using Hex;
using Managers;
using UnityEngine.UI;

namespace UI.HexCellUI
{
    public class HexImage : SingletonMonoBehaviour<HexImage>
    {
        private Image _image;

		private void Awake() => _image = GetComponent<Image>();

	    private void UpdateImage(HexCell hexCell) => _image.color = Active.SelectDrawnCell(hexCell).Color;
	    public void UpdateImage() => UpdateImage(GameStarter.Instance.Game.Active.SelectedCell);
    }
}