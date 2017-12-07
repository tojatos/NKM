using System.Collections.Generic;
using System.Linq;
using MultipleDropdowns.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace MultipleDropdowns
{
	public class UseResurrectionSelect : MultipleDropdownsTemplate
	{
		public UseResurrectionSelect(GameObject multipleDropdownObject) : base(multipleDropdownObject)
		{
			NumberOfDropdowns = 1;
			Title = "Postać do ożywienia";
			FinishButtonText = "Zakończ wybieranie postaci";
			CreateDropdowns();
		}

		protected override List<string> GetStringsToFill()
		{
			return Active.Player.Characters.Where(c=>!c.IsAlive&&c.DeathTimer<=1).Select(c => c.Name).ToList();
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
			Active.Ability.Use(Active.Player.Characters.Single(c => c.Name == selectedNames[0]));
			Active.UI = null;
		}
	}
}