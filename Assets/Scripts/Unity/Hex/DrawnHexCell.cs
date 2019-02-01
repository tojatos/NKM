using System.Collections.Generic;
using System.Linq;
using NKMCore.Hex;
using UnityEngine;

namespace Unity.Hex
{
	public class DrawnHexCell : MonoBehaviour
	{
		public override string ToString() => HexCell.ToString();

		private static Spawner Spawner => Spawner.Instance;

		public HexCell HexCell;
		
		public List<GameObject> Highlights;
		public List<GameObject> EffectHighlights;
		public Color Color;
		public void AddHighlight(string color) => Spawner.SpawnHighlightCellObject(this, color);
		public void AddEffectHighlight(string effectName) => Spawner.SpawnEffectHighlightCellObject(this, effectName);
		public void RemoveEffectHighlight(string effectName)
		{
			GameObject highlight = EffectHighlights.FirstOrDefault(h => h.GetComponent<SpriteRenderer>().sprite.name == effectName);
			if(highlight==null) return;
//			AnimationPlayer.Add(new Destroy(highlight));
			Destroy(highlight);
            EffectHighlights.Remove(highlight);
			
		}
	}
}
