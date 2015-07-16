using System;

namespace Cqrs.ObjectMapping
{
    public interface IObjectMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);

        object Map<TSource>(TSource source);

        Type GetMappedType(Type sourceType);
    }
}
