using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RainbowWine.Services.Email
{
    public class EmailSelectTemplate
    {
        public readonly static string SendPaymentlink = @"templates\html\paymentlink.html";
        public readonly static string SupplierOrderInfo = @"templates\html\supplierorderinfo.html";
    }
}
