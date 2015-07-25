namespace Commands.Core.Test.TestFixture
{
    public class InitializeCommand : ICommand
    {
        public int InitValue;

        public int Increment;

        public InitializeCommand(int initValue, int increment)
        {
            InitValue = initValue;
            Increment = increment;
        }
    }
}
