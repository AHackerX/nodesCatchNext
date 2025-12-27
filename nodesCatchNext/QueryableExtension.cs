using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace nodesCatchNext;

public static class QueryableExtension
{
	public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
	{
		return _OrderBy(query, propertyName, isDesc: false);
	}

	public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
	{
		return _OrderBy(query, propertyName, isDesc: true);
	}

	private static IOrderedQueryable<T> _OrderBy<T>(IQueryable<T> query, string propertyName, bool isDesc)
	{
		string name = (isDesc ? "OrderByDescendingInternal" : "OrderByInternal");
		PropertyInfo property = typeof(T).GetProperty(propertyName);
		return (IOrderedQueryable<T>)typeof(QueryableExtension).GetMethod(name).MakeGenericMethod(typeof(T), property.PropertyType).Invoke(null, new object[2] { query, property });
	}

	public static IOrderedQueryable<T> OrderByInternal<T, TProp>(IQueryable<T> query, PropertyInfo memberProperty)
	{
		return query.OrderBy(_GetLamba<T, TProp>(memberProperty));
	}

	public static IOrderedQueryable<T> OrderByDescendingInternal<T, TProp>(IQueryable<T> query, PropertyInfo memberProperty)
	{
		return query.OrderByDescending(_GetLamba<T, TProp>(memberProperty));
	}

	private static Expression<Func<T, TProp>> _GetLamba<T, TProp>(PropertyInfo memberProperty)
	{
		if (memberProperty.PropertyType != typeof(TProp))
		{
			throw new Exception();
		}
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
		return Expression.Lambda<Func<T, TProp>>(Expression.Property(parameterExpression, memberProperty), new ParameterExpression[1] { parameterExpression });
	}
}
