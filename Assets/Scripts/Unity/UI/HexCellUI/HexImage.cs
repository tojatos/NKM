﻿using NKMCore;
using NKMCore.Hex;
using Unity.Hex;
using UnityEngine.UI;

namespace Unity.UI.HexCellUI
{
    public class HexImage : SingletonMonoBehaviour<HexImage>
    {
        private Image _image;

		private void Awake()
	    {
		    _image = GetComponent<Image>();
		    HexMapDrawer.Instance.AfterCellSelect += UpdateImage;
	    }

	    private void UpdateImage(HexCell hexCell) => _image.color = Active.SelectDrawnCell(hexCell).Color;
    }
}