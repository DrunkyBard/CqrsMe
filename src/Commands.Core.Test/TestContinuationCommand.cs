using System;
using Commands.Core.Test.TestFixture;
using Moq;
using Xunit;

namespace Commands.Core.Test
{
    public class TestContinuationCommand
    {
        private readonly ICommandComposition _commandComposition;
        private readonly Mock<IObjectMapper> _objectMapperMock;
        private readonly Mock<ICommandHandlerFactory> _commandHandlerFactoryMock;

        public TestContinuationCommand()
        {
            var initializeCommandHanler = new Mock<ICommandHandler<InitializeCommand, IncrementResult>>();
            initializeCommandHanler
                .Setup(ch => ch.Execute(It.IsAny<InitializeCommand>()))
                .Returns<InitializeCommand>(cmd => new IncrementResult(cmd.InitValue, cmd.Increment));

            var incrementCommandHanler = new Mock<ICommandHandler<IncrementCommand, IncrementResult>>();
            incrementCommandHanler
                .Setup(ch => ch.Execute(It.IsAny<IncrementCommand>()))
                .Returns<IncrementCommand>(cmd => new IncrementResult(cmd.CurrentState + cmd.Increment, cmd.Increment));

            _objectMapperMock = new Mock<IObjectMapper>();
            _objectMapperMock
                .Setup(om => om.Map<IncrementResult, IncrementCommand>(It.IsAny<IncrementResult>()))
                .Returns<IncrementResult>(res => new IncrementCommand(res.Increment, res.Result));

            var cmdHandlerFactory = Mock.Of<ICommandHandlerFactory>(chf =>
                chf.Create<InitializeCommand, IncrementResult>() == initializeCommandHanler.Object &&
                chf.Create<IncrementCommand, IncrementResult>() == incrementCommandHanler.Object &&
                chf.Create<MapCommand<IncrementResult, IncrementCommand>, IncrementCommand>() == new MapCommandHandler<IncrementResult, IncrementCommand>(_objectMapperMock.Object)
            );
            _commandHandlerFactoryMock = Mock.Get(cmdHandlerFactory);
            
            _commandComposition =  new CommandComposition(cmdHandlerFactory, _objectMapperMock.Object);
        }

        private IncrementResult CallContinueWithNPlusOneTimes(int iterationCount)
        {
            var continuationCommandChain = _commandComposition.StartWith<InitializeCommand, IncrementResult>(new InitializeCommand(0, 1));

            for (int i = 0; i < iterationCount; i++)
            {
                continuationCommandChain = continuationCommandChain
                    .ContinueWith<IncrementCommand, IncrementResult>();
            }

            return continuationCommandChain.Run();
        }

        [Fact]
        public void CommandComposition_WhenCallContinueWithNTimes_ThenShouldCallObjectMapperNTimes()
        {
            var rndIteration = new Random().Next(1, 1000);
            var commandResult = CallContinueWithNPlusOneTimes(rndIteration);

            Assert.Equal(commandResult.Result, rndIteration);
            _objectMapperMock
                .Verify(om => om.Map<IncrementResult, IncrementCommand>(It.IsAny<IncrementResult>()), Times.Exactly(rndIteration));
        }

        [Fact]
        public void CommandComposition_WhenCallContinueWithNTimes_ThenShouldCallCommandFactoryNTimes()
        {
            var rndIteration = new Random().Next(1, 1000);

            var commandResult = CallContinueWithNPlusOneTimes(rndIteration);

            Assert.Equal(commandResult.Result, rndIteration);
            _commandHandlerFactoryMock
                .Verify(chf => chf.Create<InitializeCommand, IncrementResult>(), Times.Once);
            _commandHandlerFactoryMock
                .Verify(chf => chf.Create<IncrementCommand, IncrementResult>(), Times.Exactly(rndIteration));
        }
    }
}
