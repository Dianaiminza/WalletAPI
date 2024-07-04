using Domain.Entities;
using static Domain.Enums.TransactionEnums;

namespace CoreApi.Infrastructure.Extensions.Filters;

public static class RepositoryFilterExtensions
{
  

  public static IQueryable<DailyExpense> FilterDailyExpenses(
    this IQueryable<DailyExpense> dailyExpense,
    ModeOfPayments? modeOfPayments,
    Categories? serviceCategory)
  {
    var filteredServices = dailyExpense
      .Where(dailyExpense =>
        (serviceCategory == null || dailyExpense.Category == serviceCategory) &&
        (modeOfPayments == null || dailyExpense.ModeOfPayment == modeOfPayments)
      );

    return filteredServices;
  }

  public static IQueryable<DailyExpense> SearchDailyExpenses(
    this IQueryable<DailyExpense> notifications,
    string searchTerm)
  {
    if (string.IsNullOrWhiteSpace(searchTerm))
    {
      return notifications;
    }

    var lowerCaseTerm = searchTerm.Trim().ToLower();

        return notifications
          .Where(s =>
            s.TransactionName.Contains(lowerCaseTerm) ||
            s.VendorName.Contains(lowerCaseTerm));
  }
}
