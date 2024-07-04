using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Enums.TransactionEnums;

namespace Application.Boundary.Requests
{
    public class CreateWalletRequest
    {
        public string TransactionName { get; set; }

        public string VendorName { get; set; }

        public int TransactionCost { get; set; }

        public int TransactionCostCharges { get; set; }

        public ModeOfPayments ModeOfPayment { get; set; }
        public DateTime DateOfTransaction { get; set; }

        public Categories Category { get; set; }
    }
}
