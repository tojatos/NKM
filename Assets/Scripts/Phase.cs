using UI;

public class Phase
{
//    private static Game Game => GameStarter.Instance.Game;

    private int _number;
    public int Number
    {
        get
        {
            return _number;
        }
        set
        {
            _number = value;
            UIManager.Instance.UpdateActivePhaseText();
        }
    }

    public delegate void VoidDelegate();
    public event VoidDelegate PhaseFinished;

    public void Finish()
    {
//        Game.Players.ForEach(p => p.Characters.ForEach(c => c.OnPhaseFinish()));
        Number++;
        PhaseFinished?.Invoke();
    }
}
