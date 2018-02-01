using System.Linq;
using MyGameObjects.MyGameObject_templates;
using UIManagers;

public class Turn
{
	private Game Game;
	public bool IsDone { get; private set; }
	//private int Number;
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
		if (Game.Active.Phase.Number == 0)
		{
		 if (Game.Active.GamePlayer.Characters.Any(c => !c.IsOnMap))
			{
				UIManager.Instance.ForcePlacingChampions = true;
			}
		}
	}

	public delegate void OnTurnFinish();
	public event OnTurnFinish TurnFinished;

	public void Finish()
	{
		TurnFinished?.Invoke();
	}
	//public void TurnFinished()
	//{
	//	if (CharacterThatTookActionInTurn != null)
	//	{
	//		CharacterThatTookActionInTurn.TookActionInPhaseBefore = true;
	//	}
	//	IsDone = true;
	//	//Number++;
	//	WasCharacterPlaced = false;
	//	CharacterThatTookActionInTurn = null;
	//	Game.Active.Reset();
	//	HexMapDrawer.RemoveAllHighlights();
	//}
}
