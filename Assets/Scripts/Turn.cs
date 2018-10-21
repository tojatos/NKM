using System.Linq;
using Managers;
using NKMObjects.Templates;
using UI;

public class Turn
{
	private static Game Game => GameStarter.Instance.Game;
	public bool IsDone { get; private set; }
	public bool WasCharacterPlaced { get; set; }
	public NKMCharacter CharacterThatTookActionInTurn { get; set; }

	public Turn()
	{
		IsDone = false;
		WasCharacterPlaced = false;
		CharacterThatTookActionInTurn = null;
		TurnFinished += (character) =>
		{
			if (CharacterThatTookActionInTurn != null)
			{
				CharacterThatTookActionInTurn.TookActionInPhaseBefore = true;
			}
			IsDone = true;
			WasCharacterPlaced = false;
			CharacterThatTookActionInTurn = null;
			Game.Active.Reset();
			Game.HexMapDrawer.RemoveHighlights();
		};
		TurnFinished += c => Console.GameLog("TURN FINISHED");
	}
	public void Start(GamePlayer gamePlayer)
	{
		Game.Active.GamePlayer = gamePlayer;
		Game.Active.Turn.IsDone = false;
		Game.Active.Reset();
		UIManager.Instance.UpdateActivePlayerUI();
		TurnStarted?.Invoke(gamePlayer);
		
		
		if (Game.Active.Phase.Number != 0) return;
		if (Game.Active.GamePlayer.Characters.Any(c => !c.IsOnMap) && !Game.IsReplay)
		{
			UIManager.Instance.ForcePlacingChampions = true;
		}
	}

	public delegate void CharacterDelegate(NKMCharacter character);
	public delegate void PlayerDelegate(GamePlayer player);
	public event CharacterDelegate TurnFinished;
	public event PlayerDelegate TurnStarted;

	public void Finish() => TurnFinished?.Invoke(CharacterThatTookActionInTurn);
}
