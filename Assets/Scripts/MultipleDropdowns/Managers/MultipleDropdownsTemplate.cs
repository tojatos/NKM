//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Managers;
//using UIManagers;
//using UnityEngine;
//using UnityEngine.UI;
//
//namespace MultipleDropdowns.Managers
//{
//	public abstract class MultipleDropdownsTemplate
//	{
//		protected GameObject DropdownGroupPrefab;
//		protected GameObject Dropdowns;
//		private Text TitleText;
//		private Button FinishButton;
//		private GameObject MultipleDropdownObject { get; }
//		protected readonly Active Active;
//		protected readonly List<GameObject> DropdownGroups;
//		protected int NumberOfDropdowns;
//		protected string Title;
//		protected string FinishButtonText;
//
//		protected MultipleDropdownsTemplate(GameObject multipleDropdownObject)
//		{
//			Active = LocalGameStarter.Instance.Game.Active;
//
//			MultipleDropdownObject = multipleDropdownObject;
//			GetMonoBehaviourInfo();
//			DropdownGroups = new List<GameObject>();
//			FinishButton.onClick.AddListener(FinishSelecting);
//		}
//		private void GetMonoBehaviourInfo()
//		{
//			var info = MultipleDropdownObject.GetComponent<MultipleDropdownsInfo>();
//			DropdownGroupPrefab = info.DropdownGroupPrefab;
//			Dropdowns = info.Dropdowns;
//			FinishButton = info.FinishButton;
//			TitleText = info.TitleText;
//		}
//		public void Open()
//		{
//			if (MultipleDropdownObject.activeSelf)
//			{
//				throw new Exception(Title + " jest już otwarty!");
//			}
//
//			Game.UIManager.VisibleUI = new List<GameObject> { MultipleDropdownObject };
//			Fill();
//			FinishButton.GetComponentInChildren<Text>().text = FinishButtonText;
//			TitleText.text = Title;
//		}
//
//		private void Fill()
//		{
//			var stringsToFill = GetStringsToFill();
//			var i = 0;
//			foreach (var dropdown in DropdownGroups.Select(dropdownGroup => dropdownGroup.GetComponentInChildren<Dropdown>()))
//			{
//				dropdown.ClearOptions();
//				dropdown.AddOptions(stringsToFill);
//				dropdown.value = i;
//				i++;
//			}
//		}
//
//		protected List<string> GetData()
//		{
//			var stringsToFill = GetStringsToFill();
//			return DropdownGroups.Select(dropdownGroup => dropdownGroup.GetComponentInChildren<Dropdown>()).Select(dropdown => stringsToFill[dropdown.value]).ToList();
//		}
//
//		protected virtual List<string> GetStringsToFill()
//		{
//			throw new NotImplementedException();
//		}
//		protected virtual void CreateDropdowns()
//		{
//			throw new NotImplementedException();
//		}
//		protected virtual void FinishSelecting()
//		{
//			throw new NotImplementedException();
//		}
//
//	}
//}