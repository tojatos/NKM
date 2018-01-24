//using System.Collections.Generic;
//using System.Linq;
//using MultipleDropdowns.Managers;
//using UnityEngine;
//using UnityEngine.UI;

//namespace MultipleDropdowns
//{
//	public class UseItemSelect : MultipleDropdownsTemplate
//	{
//		public UseItemSelect(GameObject multipleDropdownObject) : base(multipleDropdownObject)
//		{
//			NumberOfDropdowns = 1;
//			Title = "Użyj przedmiotu";
//			FinishButtonText = "Zakończ wybieranie przedmiotu";
//			CreateDropdowns();
//		}

//		protected override List<string> GetStringsToFill()
//		{
//			return Active.Player.Items.Select(c => c.Name).ToList();
//		}

//		protected sealed override void CreateDropdowns()
//		{
//			for (var i = 1; i <= NumberOfDropdowns; i++)
//			{
//				var dropdownGroup = Object.Instantiate(DropdownGroupPrefab, Dropdowns.transform);
//				dropdownGroup.GetComponentInChildren<Text>().text = "Przedmiot ";
//				DropdownGroups.Add(dropdownGroup);
//			}
//		}

//		protected override void FinishSelecting()
//		{
//			var selectedNames = GetData();
//			Active.MyGameObject = Active.Player.Items.Single(c => c.Name == selectedNames[0]);
//			UIManager.VisibleUI = null;
//		}
//	}
//}