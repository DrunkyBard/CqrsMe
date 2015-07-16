using AutoMapper;

namespace Commands.Core
{
    internal class AutomapperObjectMapper : IObjectMapper
    {
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            var destination = Mapper.Map<TSource, TDestination>(source);

            return destination;
        }
    }
}
