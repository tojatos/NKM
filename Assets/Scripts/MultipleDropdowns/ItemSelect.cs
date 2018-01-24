//using System.Collections.Generic;
//using System.Linq;
//using MultipleDropdowns.Managers;
//using MyGameObjects.MyGameObject_templates;
//using UnityEngine;
//using UnityEngine.UI;

//namespace MultipleDropdowns
//{
//	public class ItemSelect : MultipleDropdownsTemplate
//	{
//		public ItemSelect(GameObject multipleDropdownObject) : base(multipleDropdownObject)
//		{
//			NumberOfDropdowns = 5;
//			Title = "Wybór przedmiotów";
//			FinishButtonText = "Zakończ wybieranie przedmiotów";
//			CreateDropdowns();
//		}

//		protected override List<string> GetStringsToFill()
//		{
//			return AllMyGameObjects.Instance.Items.GetNames();
//		}

//		protected sealed override void CreateDropdowns()
//		{
//			for (var i = 1; i <= NumberOfDropdowns; i++)
//			{
//				var dropdownGroup = Object.Instantiate(DropdownGroupPrefab, Dropdowns.transform);
//				dropdownGroup.GetComponentInChildren<Text>().text = "Przedmiot " + i;
//				DropdownGroups.Add(dropdownGroup);
//			}
//		}

//		protected override void FinishSelecting()
//		{
//			var selectedNames = GetData();
//			Validator.ValidateInitializationSelect(selectedNames);
//			var classNames = AllMyGameObjects.Instance.Items.Where(c => selectedNames.Contains(c.Name)).ToList().GetClassNames();
//			Active.GamePlayer.Items.AddRange(Spawner.Create("Items", classNames).Cast<Item>());
//			Active.GamePlayer.HasSelectedItems = true;
//		}
//	}
//}