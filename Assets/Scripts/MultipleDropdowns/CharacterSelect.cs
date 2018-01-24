//using System.Collections.Generic;
//using System.Linq;
//using Hex;
//using MultipleDropdowns.Managers;
//using MyGameObjects.MyGameObject_templates;
//using UnityEngine;
//using UnityEngine.UI;
//
//namespace MultipleDropdowns
//{
//	public class CharacterSelect : MultipleDropdownsTemplate
//	{
//		public CharacterSelect(GameObject multipleDropdownObject) : base(multipleDropdownObject)
//		{
//			NumberOfDropdowns = PlayerPrefs.GetInt("NumberOfCharactersPerPlayer", HexMapDrawer.Instance.HexMap.MaxCharacters);
//			Title = "Wybór postaci";
//			FinishButtonText = "Zakończ wybieranie postaci";
//			CreateDropdowns();
//		}
//
//		protected override List<string> GetStringsToFill()
//		{
//			return AllMyGameObjects.Instance.Characters.GetNames();
//		}
//
//		protected sealed override void CreateDropdowns()
//		{
//			for (var i = 1; i <= NumberOfDropdowns; i++)
//			{
//				var dropdownGroup = Object.Instantiate(DropdownGroupPrefab, Dropdowns.transform);
//				dropdownGroup.GetComponentInChildren<Text>().text = "Postać " + i;
//				DropdownGroups.Add(dropdownGroup);
//			}
//		}
//
//		protected override void FinishSelecting()
//		{
//			var selectedNames = GetData();
//			Validator.ValidateInitializationSelect(selectedNames);
//			var classNames = AllMyGameObjects.Instance.Characters.Where(c => selectedNames.Contains(c.Name)).ToList().GetClassNames();
//			Active.GamePlayer.Characters.AddRange(Spawner.Create("Characters", classNames).Cast<Character>());
//			Active.GamePlayer.HasSelectedCharacters = true;
//		}
//	}
//}