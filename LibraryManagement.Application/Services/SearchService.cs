using System.Linq.Expressions;

namespace LibraryManagement.Application.Services;

public class SearchService<T> : ISearchService<T>
{
    public Expression<Func<T, bool>> BuildExpression<TDto>(TDto searchDto)
    {
        var dtoType = typeof(TDto);
        var tType = typeof(T);

        var param = Expression.Parameter(typeof(T), "e");
        Expression? finalExpression = null;

        foreach (var prop in dtoType.GetProperties())
        {
            var dtoValue = prop.GetValue(searchDto);
            if (dtoValue == null) continue;

            var tPropType = tType.GetProperty(prop.Name);
            if (tPropType == null) continue;

            Expression? current = null;

            if (tPropType.PropertyType == typeof(string))
            {
                string strDtoValue = dtoValue.ToString()!;
                if (!string.IsNullOrEmpty(strDtoValue))
                {
                    current = Expression.Call(
                        Expression.Property(param, tPropType.Name),
                        typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                        Expression.Constant(strDtoValue)
                    );
                }
            }

            else if ((Nullable.GetUnderlyingType(tPropType.PropertyType) ?? tPropType.PropertyType) == typeof(int) ||
                    (Nullable.GetUnderlyingType(tPropType.PropertyType) ?? tPropType.PropertyType) == typeof(long) ||
                    (Nullable.GetUnderlyingType(tPropType.PropertyType) ?? tPropType.PropertyType) == typeof(bool))
            {
                var propExpr = Expression.Property(param, tPropType.Name);
                current = Expression.Equal(
                    propExpr,
                    Expression.Constant(dtoValue, propExpr.Type)
                );
            }

            else if ((Nullable.GetUnderlyingType(tPropType.PropertyType) ?? tPropType.PropertyType) == typeof(DateTime))
            {
                string strDtoValue = dtoValue.ToString()!;
                var dateTime = DateTime.Parse(strDtoValue);
                var propExpr = Expression.Property(param, tPropType.Name);

                current = Expression.Equal(
                    Expression.Property(param, tPropType.Name),
                    Expression.Constant(dateTime, propExpr.Type)
                );
            }

            if (current != null)
            {
                if (finalExpression is null)
                    finalExpression = current;

                else
                    finalExpression = Expression.AndAlso(finalExpression, current);
            }
        }

        if (finalExpression == null)
        {
            finalExpression = Expression.Constant(true);
        }

        return Expression.Lambda<Func<T, bool>>(finalExpression, param);
    }
}
