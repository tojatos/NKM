using NKMCore.Templates;

namespace NKMCore
{
	public class Turn
	{
		private readonly Game _game;
		private Console Console => _game.Console;
		public bool IsDone { get; private set; }
		public Character CharacterThatTookActionInTurn { get; set; }

		public Turn(Game game)
		{
			_game = game;
			IsDone = false;
			CharacterThatTookActionInTurn = null;
			TurnFinished += character =>
			{
				if (CharacterThatTookActionInTurn != null)
				{
					CharacterThatTookActionInTurn.TookActionInPhaseBefore = true;
				}
				IsDone = true;
				CharacterThatTookActionInTurn = null;
				_game.Active.Reset();
			};
		}
		public void Start(GamePlayer gamePlayer)
		{
			_game.Active.GamePlayer = gamePlayer;
			_game.Active.Turn.IsDone = false;
			_game.Active.Reset();
			TurnStarted?.Invoke(gamePlayer);
		}

		public delegate void CharacterDelegate(Character character);
		public delegate void PlayerDelegate(GamePlayer player);
		public event CharacterDelegate TurnFinished;
		public event PlayerDelegate TurnStarted;

		public void Finish() => TurnFinished?.Invoke(CharacterThatTookActionInTurn);
	}
}
