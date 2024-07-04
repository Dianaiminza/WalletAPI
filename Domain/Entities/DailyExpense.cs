using Infrastructure.Shared.Contracts;

using static Domain.Enums.TransactionEnums;

namespace Domain.Entities
{
    public class DailyExpense : BaseEntity
    {
        public long Id { get; set; }
        public string TransactionName { get; set; } 

        public string VendorName { get; set; } 

        public int TransactionCost { get; set; } 

        public int TransactionCostCharges { get; set; } 

        public ModeOfPayments ModeOfPayment { get; set; }
        public DateTimeOffset  DateOfTransaction { get; set; } 

        public Categories Category { get; set; } 

        public int TotalAmount
        {
            get
            {
                return TransactionCostCharges + TransactionCost;
            }
            set { }
        }

    }
}
