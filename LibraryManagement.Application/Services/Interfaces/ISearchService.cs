using System.Linq.Expressions;

namespace LibraryManagement.Application.Services.Interaces;

public interface ISearchService<T>
{
    Expression<Func<T, bool>> BuildExpression<TDto>(TDto searchDto);
}
