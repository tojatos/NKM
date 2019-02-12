using System.Linq;
using JetBrains.Annotations;
using NKMCore;
using Unity.Extensions;
using UnityEngine.UI;

namespace Unity.UI
{
    public class Victory : SingletonMonoBehaviour<Victory>
    {
        public Text VictoryText;
        public void Show(Game game)
        {
            GamePlayer playerThatWon = game.Players.Single(p => !p.IsEliminated);
            VictoryText.text = 
$@"Koniec gry!
Wygrał <b>{playerThatWon.Name}!</b>";
            gameObject.Show();
        }

        [UsedImplicitly]
        public void GoToGameStatisticsScene() {} //TODO: SceneManager.LoadScene(Scenes.GameStatistics);
    }
}