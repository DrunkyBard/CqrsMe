namespace Commands.Core.Test.TestFixture
{
    public class IncrementCommand : ICommand
    {
        public int Increment;
        public int CurrentState;

        public IncrementCommand(int increment, int currentState)
        {
            Increment = increment;
            CurrentState = currentState;
        }
    }
}
