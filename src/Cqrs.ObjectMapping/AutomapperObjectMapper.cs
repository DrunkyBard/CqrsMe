using System;
using System.Linq;
using AutoMapper;

namespace Cqrs.ObjectMapping
{
    internal sealed class AutomapperObjectMapper : IObjectMapper
    {
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            var destination = Mapper.Map<TSource, TDestination>(source);

            return destination;
        }

        public object Map<TSource>(TSource source)
        {
            var destinationType = GetMappedType(source.GetType());
            var destination = Mapper.Map(source, source.GetType(), destinationType);

            return destination;
        }

        public Type GetMappedType(Type sourceType)
        {
            var destinationType = Mapper.GetAllTypeMaps()
                .Single(x => x.SourceType == sourceType)
                .DestinationType;

            return destinationType;
        }
    }
}
