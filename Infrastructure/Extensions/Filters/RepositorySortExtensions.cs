
using Domain.Entities;
using System.Linq.Dynamic.Core;

namespace CoreApi.Infrastructure.Extensions.Filters;

public static class RepositorySortExtensions
{
  public static IQueryable<DailyExpense> SortNotifications(this IQueryable<DailyExpense> notifications,
      string orderByQueryString)
  {
    if (string.IsNullOrWhiteSpace(orderByQueryString))
      return notifications.OrderByDescending(e => e.CreatedOn);

    var orderQuery = OrderQueryBuilder.CreateOrderQuery<DailyExpense>(orderByQueryString);

    return string.IsNullOrWhiteSpace(orderQuery)
        ? notifications.OrderByDescending(e => e.CreatedOn)
        : notifications.OrderBy(orderQuery);
  }
}
