using Managers;
using UI;

public class Phase
{
	private static Game Game => GameStarter.Instance.Game;

	public int Number { get; set; }

//	public Phase(Game game)
//	{
//		Number = 0;
//	}

	public delegate void VoidDelegate();
	public event VoidDelegate PhaseFinished;
	
	public void Finish()
	{
		Game.Players.ForEach(p => p.Characters.ForEach(c => c.OnPhaseFinish()));
//		Game.HexMapDrawer.Cells.ForEach(c=> c.Effects.ForEach(e=>e.OnPhaseFinish()));
		Number++;
		UIManager.Instance.UpdateActivePhaseText();
		PhaseFinished?.Invoke();
	}
}
