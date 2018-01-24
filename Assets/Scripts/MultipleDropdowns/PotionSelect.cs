//using System.Collections.Generic;
//using System.Linq;
//using MultipleDropdowns.Managers;
//using MyGameObjects.MyGameObject_templates;
//using UnityEngine;
//using UnityEngine.UI;

//namespace MultipleDropdowns
//{
//	public class PotionSelect : MultipleDropdownsTemplate
//	{
//		public PotionSelect(GameObject multipleDropdownObject) : base(multipleDropdownObject)
//		{
//			NumberOfDropdowns = 2;
//			Title = "Wybór mikstur";
//			FinishButtonText = "Zakończ wybieranie mikstur";
//			CreateDropdowns();
//		}

//		protected override List<string> GetStringsToFill()
//		{
//			return AllMyGameObjects.Instance.Potions.GetNames();
//		}

//		protected sealed override void CreateDropdowns()
//		{
//			for (var i = 1; i <= NumberOfDropdowns; i++)
//			{
//				var dropdownGroup = Object.Instantiate(DropdownGroupPrefab, Dropdowns.transform);
//				dropdownGroup.GetComponentInChildren<Text>().text = "Mikstura " + i;
//				DropdownGroups.Add(dropdownGroup);
//			}
//		}

//		protected override void FinishSelecting()
//		{
//			var selectedNames = GetData();
//			Validator.ValidateInitializationSelect(selectedNames);
//			var classNames = AllMyGameObjects.Instance.Potions.Where(c => selectedNames.Contains(c.Name)).ToList().GetClassNames();
//			Active.GamePlayer.Potions.AddRange(Spawner.Create("Potions", classNames).Cast<Potion>());
//			Active.GamePlayer.HasSelectedPotions = true;
//		}
//	}
//}