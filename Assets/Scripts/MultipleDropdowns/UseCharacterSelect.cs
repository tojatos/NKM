using System.Collections.Generic;
using System.Linq;
using Hex;
using MultipleDropdowns.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace MultipleDropdowns
{
	public class UseCharacterSelect : MultipleDropdownsTemplate
	{
		public UseCharacterSelect(GameObject multipleDropdownObject) : base(multipleDropdownObject)
		{
			NumberOfDropdowns = 1;
			Title = "Wystaw postać";
			FinishButtonText = "Zakończ wybieranie postaci";
			CreateDropdowns();
		}

		protected override List<string> GetStringsToFill()
		{
			return Active.Player.Characters.Where(c => !c.IsOnMap && c.IsAlive).Select(c => c.Name).ToList();
		}

		protected sealed override void CreateDropdowns()
		{
			for (var i = 1; i <= NumberOfDropdowns; i++)
			{
				var dropdownGroup = Object.Instantiate(DropdownGroupPrefab, Dropdowns.transform);
				dropdownGroup.GetComponentInChildren<Text>().text = "Postać ";
				DropdownGroups.Add(dropdownGroup);
			}
		}

		protected override void FinishSelecting()
		{
			var selectedNames = GetData();
			HexMapDrawer.RemoveAllHighlights();
			Active.Player.GetSpawnPoints().Where(sp => sp.CharacterOnCell == null).ToList().ForEach(c=>c.ToggleHighlight(HiglightColor.Red));
			Active.MyGameObject = Active.Player.Characters.Single(c => c.Name == selectedNames[0]);
			Active.UI = null;
		}
	}
}