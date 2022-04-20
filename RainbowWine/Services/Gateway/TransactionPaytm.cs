using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.Gateway
{
    public class TransactionPaytm : PaymentService<CashFreeModel>
    {
        protected override object MakePayment(CashFreeModel model)
        {
            throw new NotImplementedException();
        }
    }
}