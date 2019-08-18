using System.Collections.Generic;
using System.Linq;
using NKMCore;
using NKMCore.Templates;
using Unity.Extensions;
using Unity.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Unity
{
	public class SpriteSelect : SingletonMonoBehaviour<SpriteSelect>
	{
		public Button FinishSelectingButton;
		public Text Title;
		public GameObject SpriteObjectPrefab;
		public GameObject Sprites;

		private List<int> SelectedIds { get; } = new List<int>();

		public void Open(SpriteSelectProperties props)
		{
			Clean();

			GenerateSprites(props);

			Title.text = props.SelectionTitle;
			FinishSelectingButton.onClick.AddListener(() => props.OnSelectFinish(SelectedIds));
			FinishSelectingButton.GetComponentInChildren<Text>().text = props.FinishSelectingButtonText;
			FinishSelectingButton.gameObject.SetActive(!props.Instant);

			Show();
		}

        public void Close() => Hide();

        private void Hide() => gameObject.transform.parent.gameObject.Hide();
        private void Show() => gameObject.transform.parent.gameObject.Show();

		private void GenerateSprites(SpriteSelectProperties props)
		{
			if (props.WhatIsSelected == SelectableProperties.Type.Character)
			{
				List<Character> toPickFrom = Game.GetMockCharacters();
				if (GameStarter._game != null) toPickFrom.AddRange(GameStarter._game.Characters);

				toPickFrom.FindAll(c => props.IdsToSelect.Contains(c.ID)).ForEach(c => SpawnSpriteObject(c, props.Instant));
			}
		}

		private void Clean()
		{
			SelectedIds.Clear();
			Sprites.transform.Clear(); //Careful! Removes probably on the next frame
			FinishSelectingButton.onClick.RemoveAllListeners();
		}

		private void SpawnSpriteObject(Character o, bool instant)
		{
			GameObject spriteObject = Instantiate(SpriteObjectPrefab, Sprites.transform);
			var button = spriteObject.GetComponent<Button>();
			button.onClick.AddListener(() =>
			{
                bool isSelected = ToggleSelected(o.ID);
                button.image.color = isSelected ? Color.white : Color.grey;

				if (instant)
					FinishSelectingButton.onClick.Invoke();
			});
			button.image.color = Color.grey;
			button.image.sprite = Stuff.Sprites.CharacterHexagons.SingleOrDefault(c => c.name == o.Name);
		}
		/// <summary>
		/// Adds object to selected list if is not on it already,
		/// otherwise removes it from that list.
		/// </summary>
		/// <param name="o">Object to toggle</param>
		/// <returns>Is selected</returns>
		private bool ToggleSelected(int o)
		{
			if (SelectedIds.Contains(o))
			{
				SelectedIds.Remove(o);
				return false;
			}

			SelectedIds.Add(o);
			return true;
		}

	}
}