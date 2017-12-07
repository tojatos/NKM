using Managers;
using UIManagers;

public class Phase
{
	public int Number { get; set; }

	public Phase()
	{
		Number = 0;
	}

	public void Finish()
	{
		GameManager.Players.ForEach(p => p.Characters.ForEach(c => c.OnPhaseFinish()));
		Number++;
		UIManager.Instance.UpdateActivePhaseText();
	}
}
