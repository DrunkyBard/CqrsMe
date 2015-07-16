using System;

namespace Commands.Core
{
    internal sealed class MapCommand<TSource, TDestinationCommand> : ICommand
        where TDestinationCommand : ICommand
    {
        private readonly Func<TSource> _source;
        public Func<TSource> Source
        {
            get { return _source; }
        }
        
        public MapCommand(Func<TSource> source)
        {
            _source = source;
        }
    }
}
