using Mapster;

namespace PANDACLINIC.Application.Mapping
{
    /// <summary>
    /// Simple Mapster-backed mapper implementation.
    /// </summary>
    public class ServiceMapper : IMapper
    {
        public TDestination Map<TDestination>(object source)
            => source.Adapt<TDestination>();

        public TDestination Map<TSource, TDestination>(TSource source)
            => source.Adapt<TSource, TDestination>();

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
            => source.Adapt(destination);
    }
}
