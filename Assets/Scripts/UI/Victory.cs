using System.Linq;
using Extensions;
using JetBrains.Annotations;
using Managers;
using UnityEngine.UI;

namespace UI
{
    public class Victory : SingletonMonoBehaviour<Victory>
    {
        public Text VictoryText;
        private static Game Game => GameStarter.Instance.Game;
        public void Show()
        {
            GamePlayer playerThatWon = Game.Players.Single(p => !p.IsEliminated);
            VictoryText.text = 
$@"Koniec gry!
Wygrał <b>{playerThatWon.Name}!</b>";
            gameObject.Show();
        }

        [UsedImplicitly]
        public void GoToGameStatisticsScene() {} //TODO: SceneManager.LoadScene(Scenes.GameStatistics);
    }
}