using Mapster;

namespace PANDACLINIC.Application.Mapping
{
    /// <summary>
    /// Minimal mapper abstraction backed by Mapster.
    /// </summary>
    public interface IMapper
    {
        TDestination Map<TDestination>(object source);
        TDestination Map<TSource, TDestination>(TSource source);
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}
