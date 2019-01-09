using System.Linq;
using NKMObjects.Templates;
using UI;

public class Turn
{
	private readonly Game _game;
	private Console Console => _game.Console;
	public bool IsDone { get; private set; }
	public bool WasCharacterPlaced { get; set; }
	public Character CharacterThatTookActionInTurn { get; set; }

	public Turn(Game game)
	{
		_game = game;
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
			_game.Active.Reset();
			_game.HexMapDrawer.RemoveHighlights();
		};
		TurnFinished += c => Console.GameLog("TURN FINISHED");
	}
	public void Start(GamePlayer gamePlayer)
	{
		_game.Active.GamePlayer = gamePlayer;
		_game.Active.Turn.IsDone = false;
		_game.Active.Reset();
		UIManager.Instance.UpdateActivePlayerUI();
		TurnStarted?.Invoke(gamePlayer);
		
		
		if (_game.Active.Phase.Number != 0) return;
		if (_game.Active.GamePlayer.Characters.Any(c => !c.IsOnMap) && !_game.IsReplay)
		{
			UIManager.Instance.ForcePlacingChampions = true;
		}
	}

	public delegate void CharacterDelegate(Character character);
	public delegate void PlayerDelegate(GamePlayer player);
	public event CharacterDelegate TurnFinished;
	public event PlayerDelegate TurnStarted;

	public void Finish() => TurnFinished?.Invoke(CharacterThatTookActionInTurn);
}
