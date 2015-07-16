using System;
using Cqrs.Domain.Core;
using Cqrs.Domain.ProcessManager.Exceptions;
using Xunit;

namespace Cqrs.Domain.ProcessManager.Test
{
    public class ProcessManagerTests
    {
        [Fact]
        public void Ensure_What_Right_Invoke()
        {
            var fakeProcessManager = new FakeProcessManager(States.First);
            fakeProcessManager.Raise(new FirstTestEvent());
            Assert.Equal(fakeProcessManager.CurrentState, States.Second);
            Assert.True(fakeProcessManager.InvokedForFirstEvent);
            Assert.False(fakeProcessManager.InvokedForSecondEvent);

            fakeProcessManager.Raise(new SecondTestEvent());
            Assert.Equal(fakeProcessManager.CurrentState, States.Mixed);
            Assert.True(fakeProcessManager.InvokedForSecondEvent);
            Assert.False(fakeProcessManager.InvokedForFirstEvent);

            fakeProcessManager.Raise(new SecondTestEvent());
            Assert.Equal(fakeProcessManager.CurrentState, States.Mixed);
            Assert.True(fakeProcessManager.InvokedForSecondEvent);
            Assert.False(fakeProcessManager.InvokedForFirstEvent);

            fakeProcessManager.Raise(new FirstTestEvent());
            Assert.Equal(fakeProcessManager.CurrentState, States.Completed);
            Assert.True(fakeProcessManager.InvokedForSecondEvent);
            Assert.True(fakeProcessManager.InvokedForFirstEvent);
        }

        [Fact]
        public void Ensure_What_Thrown_Exception_After_Fire_Not_Configured_EventHandler()
        {
            var fakeProcessManager = new FakeProcessManager(States.First);
            Assert.Throws<EventHandlerNotFoundException>(() => fakeProcessManager.Raise(new UselessTestEvent()));
        }

        //[Fact]
        //public void Ensure_What_ProcessManager_Condition_Configured_Wrong()
        //{
        //    Assert.Throws<ConditionalStateAlreadyRegisteredException>(() => new ConditionExceptionedProcessManager(States.First));
        //}
        
        [Fact]
        public void Ensure_What_ProcessManager_EventHandler_Configured_Wrong()
        {
            Assert.Throws<EventHandlerAlreadyRegisteredException>(() => new HandlerExceptionedProcessManager(States.First));
        }
    }

    [Flags]
    enum States
    {
        First = 1,
        Second = 2,
        Mixed = 3,
        Completed = 4
    }

    class HandlerExceptionedProcessManager : ProcessManager<States>
    {
        public HandlerExceptionedProcessManager(States initialState)
            : base(initialState)
        {
            IfIn(States.First)
                .ThenHandle<FirstTestEvent>(@event => { })
                .AfterTransitionTo(States.Second);
            
            IfIn(States.First)
                .ThenHandle<FirstTestEvent>(@event => { })
                .AfterTransitionTo(States.First);
        }
    }

    class FakeProcessManager : ProcessManager<States>
    {
        public FakeProcessManager(States initialState) : base(initialState)
        {
            IfIn(States.First)
                .ThenHandle<FirstTestEvent>(SomeEventHandler)
                .AfterTransitionTo(States.Second);

            IfIn(States.Second)
                .ThenHandle<SecondTestEvent>(SomeEventHandler)
                .AfterTransitionTo(States.Mixed);

            IfIn(States.Mixed)
                .WaitAll<FirstTestEvent, SecondTestEvent>(SomeEventHandler)
                .AfterTransitionTo(States.Completed);
        }

        public bool InvokedForFirstEvent { get; set; }

        public bool InvokedForSecondEvent { get; set; }

        private void SomeEventHandler(FirstTestEvent @event)
        {
            InvokedForFirstEvent = true;
            InvokedForSecondEvent = false;
        }

        private void SomeEventHandler(SecondTestEvent @event)
        {
            InvokedForFirstEvent = false;
            InvokedForSecondEvent = true;
        }

        private void SomeEventHandler(FirstTestEvent firstTestEvent, SecondTestEvent secondTestEvent)
        {
            InvokedForFirstEvent = true;
            InvokedForSecondEvent = true;
        }
    }

    class FirstTestEvent : IEvent
    {}

    class SecondTestEvent : IEvent
    {}

    class UselessTestEvent : IEvent
    {}
}
