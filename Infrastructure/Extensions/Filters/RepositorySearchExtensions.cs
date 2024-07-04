
using Domain.Entities;
using Infrastructure.Shared.Extensions;
using static Domain.Enums.TransactionEnums;

namespace CoreApi.Infrastructure.Extensions.Filters;

public static class RepositorySearchExtensions
{
  public static IQueryable<DailyExpense> SearchPaymentServices(this IQueryable<DailyExpense> dailyExpenses,
    string searchTerm = "")
  {
    if (string.IsNullOrEmpty(searchTerm))
    {
      return dailyExpenses;
    }

    var matchingPaymentMode = searchTerm.GetEnumMatchingValues<ModeOfPayments>();
    var matchingCategories = searchTerm.GetEnumMatchingValues<Categories>();

    return dailyExpenses.Where(ps =>
      matchingPaymentMode.Contains(ps.ModeOfPayment) ||
      matchingCategories.Contains(ps.Category));
  }

  
}
