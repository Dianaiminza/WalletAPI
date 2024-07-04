using Application.Boundary.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Enums.TransactionEnums;

namespace Application.Validations
{
    public class DailyExpenseValidator : AbstractValidator<CreateWalletRequest>
    {
        public DailyExpenseValidator()
        {
            RuleFor(exp => exp.TransactionName).NotEmpty().MinimumLength(2).MaximumLength(50);
            RuleFor(exp => exp.TransactionCost).NotEmpty().GreaterThan(0);
            RuleFor(exp => exp.VendorName).NotEmpty().MinimumLength(2).MaximumLength(30);
            RuleFor(exp => exp.Category).IsInEnum().NotEqual(Categories.None).NotEmpty();
            RuleFor(exp => exp.ModeOfPayment).IsInEnum().NotEqual(ModeOfPayments.None).NotEmpty();

        }
    }
}
