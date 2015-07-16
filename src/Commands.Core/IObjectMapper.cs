namespace Commands.Core
{
    public interface IObjectMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);
    }
}
