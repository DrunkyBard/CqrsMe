namespace Commands.Core
{
    public interface ICommandRunner
    {
        void Run();
    }

    public interface ICommandRunner<out TOutput>
    {
        TOutput Run();
    }
}
