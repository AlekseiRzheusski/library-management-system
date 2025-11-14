using System.Linq.Expressions;

public interface ISearchService<T>
{
    Expression<Func<T, bool>> BuildExpression<TDto>(TDto searchDto);
}
