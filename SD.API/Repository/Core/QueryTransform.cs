using System.Linq.Expressions;

namespace SD.API.Repository.Core;

internal static class TransformExtensions
{
    internal static Func<IQueryable<T>, IQueryable<T>> CreateTransform<T>() => query => query;

    internal static Func<IQueryable<T>, IQueryable<T>> Skip<T>(this Func<IQueryable<T>, IQueryable<T>> transform, int? skip)
    {
        if (skip is not > 0) return transform;

        return query => transform(query).Skip(skip.Value);
    }

    internal static Func<IQueryable<T>, IQueryable<T>> Take<T>(this Func<IQueryable<T>, IQueryable<T>> transform, int? take)
    {
        if (take is not > 0) return transform;

        return query => transform(query).Take(take.Value);
    }

    internal static Func<IQueryable<T>, IQueryable<T>> OrderBy<T, TKey>(this Func<IQueryable<T>, IQueryable<T>> transform, Expression<Func<T, TKey>> expression)
    {
        return query => transform(query).OrderBy(expression);
    }

    internal static Func<IQueryable<T>, IQueryable<T>> OrderByDescending<T, TKey>(this Func<IQueryable<T>, IQueryable<T>> transform, Expression<Func<T, TKey>> expression)
    {
        return query => transform(query).OrderByDescending(expression);
    }

    internal static Func<IQueryable<T>, IQueryable<T>> ThenBy<T, TKey>(this Func<IQueryable<T>, IQueryable<T>> transform, Expression<Func<T, TKey>> expression)
    {
        return query => ((IOrderedQueryable<T>)transform(query)).ThenBy(expression);
    }

    internal static Func<IQueryable<T>, IQueryable<T>> ThenByDescending<T, TKey>(this Func<IQueryable<T>, IQueryable<T>> transform, Expression<Func<T, TKey>> expression)
    {
        return query => ((IOrderedQueryable<T>)transform(query)).ThenByDescending(expression);
    }
}