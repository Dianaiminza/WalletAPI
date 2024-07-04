using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public class TransactionEnums
    {
        public enum Categories
        {
            None=0,
            Transport = 1,
            Food=2,
            Other=3
        }
        public enum ModeOfPayments
        {
            None = 0,
            Mpesa = 1,
            Bank=2,
            Cash=3
        }
    }
}
