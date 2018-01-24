//using System.Collections.Generic;
//using System.Linq;
//using MultipleDropdowns.Managers;
//using UnityEngine;
//using UnityEngine.UI;

//namespace MultipleDropdowns
//{
//	public class UsePotionSelect : MultipleDropdownsTemplate
//	{
//		public UsePotionSelect(GameObject multipleDropdownObject) : base(multipleDropdownObject)
//		{
//			NumberOfDropdowns = 1;
//			Title = "Wybór mikstury";
//			FinishButtonText = "Zakończ wybieranie mikstury";
//			CreateDropdowns();
//		}

//		protected override List<string> GetStringsToFill()
//		{
//			return Active.GamePlayer.Potions.Select(c => c.Name).ToList();
//		}

//		protected sealed override void CreateDropdowns()
//		{
//			for (var i = 1; i <= NumberOfDropdowns; i++)
//			{
//				var dropdownGroup = Object.Instantiate(DropdownGroupPrefab, Dropdowns.transform);
//				dropdownGroup.GetComponentInChildren<Text>().text = "Mikstura ";
//				DropdownGroups.Add(dropdownGroup);
//			}
//		}

//		protected override void FinishSelecting()
//		{
//			var selectedNames = GetData();
//			Active.MyGameObject = Active.GamePlayer.Potions.Single(c => c.Name == selectedNames[0]);
//			UIManager.VisibleUI = null;
//		}
//	}
//}