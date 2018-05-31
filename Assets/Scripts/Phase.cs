using UIManagers;

public class Phase
{
	private readonly Game Game;

	public int Number { get; set; }

	public Phase(Game game)
	{
		Game = game;
		Number = 0;
	}

	public delegate void OnPhaseFinish();
	public event OnPhaseFinish PhaseFinished;
	
	public void Finish()
	{
		Game.Players.ForEach(p => p.Characters.ForEach(c => c.OnPhaseFinish()));
//		Game.HexMapDrawer.Cells.ForEach(c=> c.Effects.ForEach(e=>e.OnPhaseFinish()));
		Number++;
		UIManager.Instance.UpdateActivePhaseText();
		PhaseFinished?.Invoke();
	}
}
