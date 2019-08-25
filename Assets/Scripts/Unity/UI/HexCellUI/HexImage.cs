using NKMCore.Hex;
using Unity.Hex;
using UnityEngine.UI;

namespace Unity.UI.HexCellUI
{
    public class HexImage : SingletonMonoBehaviour<HexImage>
    {
        private Image _image;

        private void Awake() => _image = GetComponent<Image>();
        public void Init() => HexMapDrawer.Instance.AfterCellSelect += UpdateImage;
        private void UpdateImage(HexCell hexCell) => _image.color = HexMapDrawer.Instance.SelectDrawnCell(hexCell).Color;
    }
}