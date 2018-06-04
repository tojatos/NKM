using System.Linq;
using MyGameObjects.MyGameObject_templates;
using UIManagers;

public class Turn
{
	private readonly Game Game;
	public bool IsDone { get; private set; }
	public bool WasCharacterPlaced { get; set; }
	public Character CharacterThatTookActionInTurn { get; set; }

	public Turn(Game game)
	{
		Game = game;
		IsDone = false;
		WasCharacterPlaced = false;
		CharacterThatTookActionInTurn = null;
		TurnFinished += () =>
		{
			if (CharacterThatTookActionInTurn != null)
			{
				CharacterThatTookActionInTurn.TookActionInPhaseBefore = true;
			}
			IsDone = true;
			WasCharacterPlaced = false;
			CharacterThatTookActionInTurn = null;
			Game.Active.Reset();
			Game.HexMapDrawer.RemoveAllHighlights();
		};
	}
	public void Start(GamePlayer gamePlayer)
	{
		Game.Active.GamePlayer = gamePlayer;
		Game.Active.Turn.IsDone = false;
		Game.Active.Reset();
		UIManager.Instance.UpdateActivePlayerUI();
		TurnStarted?.Invoke(gamePlayer);
		
		
		if (Game.Active.Phase.Number != 0) return;
		if (Game.Active.GamePlayer.Characters.Any(c => !c.IsOnMap))
		{
			UIManager.Instance.ForcePlacingChampions = true;
		}
	}

	public delegate void OnTurnFinish();
	public delegate void PlayerDelegate(GamePlayer player);
	public event OnTurnFinish TurnFinished;
	public event PlayerDelegate TurnStarted;

	public void Finish()
	{
		TurnFinished?.Invoke();
	}
}
