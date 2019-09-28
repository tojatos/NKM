using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Managers
{
    public class ReplaySelect : SingletonMonoBehaviour<ReplaySelect>
    {
        public Button SelectReplayButton;
        public GameObject ReplayInfos;

        private static string SelectedReplayFilePath
        {
            set => SessionSettings.Instance.SelectedReplayFilePath = value;
        }

        private void Start()
        {
            SelectReplayButton.onClick.AddListener(() => SceneManager.LoadScene(Scenes.MainGame));
            RefreshList();
        }

        private void RefreshList()
        {
            ReplayInfos.transform.Clear();
            List<string> filePaths = Directory.GetFiles(PathManager.LogDirPath, "*.txt", SearchOption.AllDirectories).ToList();
            filePaths.ForEach(CreateReplayInfo);
        }

        private void CreateReplayInfo(string replayFilePath)
        {
            GameObject g = Instantiate(Stuff.Prefabs.Find(p => p.name == "Replay Info"), ReplayInfos.transform);
            g.AddTrigger(EventTriggerType.PointerClick, () => SelectedReplayFilePath = replayFilePath);
            g.transform.Find("Name").GetComponent<Text>().text = replayFilePath;
        }
    }
}