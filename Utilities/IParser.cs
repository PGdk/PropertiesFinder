namespace Utilities
{
    public interface IParser<in TSource, out TDestination>
    {
        TDestination Parse(TSource source);
    }
}