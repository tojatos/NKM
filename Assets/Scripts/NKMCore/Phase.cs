namespace NKMCore
{
    public class Phase
    {
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
                PhaseChanged?.Invoke();
            }
        }

        public event Delegates.Void PhaseChanged;
        public event Delegates.Void PhaseFinished;

        public void Finish()
        {
            Number++;
            PhaseFinished?.Invoke();
        }
    }
}
