using System.Reflection;
using Autofac;
using AutoMapper;
using Xunit;

namespace Commands.Core.Test
{
    public class TestContinuationCommand
    {
        private readonly ICommandComposition _commandComposition;

        public TestContinuationCommand()
        {
            Mapper.CreateMap<IncrementResult, IncrementCommand>()
                .ConvertUsing(result => new IncrementCommand(result.Increment, result.Result));
            var cBuilder = new ContainerBuilder();
            cBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(
                    x => x.IsClosedTypeOf(typeof(ICommandHandler<>)) || x.IsClosedTypeOf(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .InstancePerDependency();
            cBuilder.RegisterModule<TypeRegistrationModule>();
            var container = cBuilder.Build();
            _commandComposition = container.Resolve<ICommandComposition>();
        }

        [Fact]
        public void TestIncrementCommand()
        {
            var continuationCommandChain = _commandComposition.StartWith<InitializeCommand, IncrementResult>(new InitializeCommand(0, 1));

            for (int i = 0; i < 6; i++)
            {
                continuationCommandChain = continuationCommandChain
                    .ContinueWith<IncrementCommand, IncrementResult>();
            }

            var result = continuationCommandChain.Run();

            Assert.Equal(result.Result, 6);
        }
    }

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

    public class InitializeCommandHandler : ICommandHandler<InitializeCommand, IncrementResult>
    {
        public IncrementResult Execute(InitializeCommand command)
        {
            return new IncrementResult
            {
                Result = command.InitValue,
                Increment = command.Increment
            };
        }
    }

    public class IncrementResult
    {
        public int Result { get; set; }

        public int Increment { get; set; }
    }

    public class IncrementCommandHandler : ICommandHandler<IncrementCommand, IncrementResult>
    {
        public IncrementResult Execute(IncrementCommand command)
        {
            var result = new IncrementResult
            {
                Result = command.CurrentState + command.Increment,
                Increment = command.Increment
            };

            return result;
        }
    }
}
