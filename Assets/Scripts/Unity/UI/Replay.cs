using System.Collections.Generic;
using Unity.Extensions;
using Unity.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity.UI
{
    public class Replay : SingletonMonoBehaviour<Replay>
    {
        public GameObject PlayNextButton;
        public Queue<string> Actions;
        private void Awake() => PlayNextButton.AddTrigger(EventTriggerType.PointerClick, e => PlayNextAction());

        private void PlayNextAction()
        {
            if(Actions.Count == 0) return;
            string action = Actions.Dequeue();

            GameStarter.Act(GameStarter.Game, action);
        }

        public void Show() => gameObject.Show();
    }
}