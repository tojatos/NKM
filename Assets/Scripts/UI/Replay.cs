using System.Collections.Generic;
using System.Linq;
using Extensions;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Replay : SingletonMonoBehaviour<Replay>
    {
        public GameObject PlayNextButton;
        public Queue<string[]> Actions;
//TODO: Find a way to load RNG before action
        private void Awake() => PlayNextButton.AddTrigger(EventTriggerType.PointerClick, e =>
        {
            if(Actions.Count == 0) return;
            Debug.Log(string.Join(": ", Actions.Peek()));
            PlayNextAction();
        });

        private void PlayNextAction()
        {
//            GameStarter.Instance.Game.MakeAction(Actions.Dequeue());
            
            if(Actions.Count == 0) return;
            string nextActionName = Actions.Peek()[0];
            if(new []{"ACTION TAKEN", "TURN FINISHED", "RNG"}.Contains(nextActionName)) PlayNextAction();
        }

        public void Show() => gameObject.Show();
    }
}