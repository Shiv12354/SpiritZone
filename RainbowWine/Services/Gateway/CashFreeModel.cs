using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.Gateway
{
    public class CashFreeModel: IPaymentModel
    {
        public string OrderId { get; set; }
        public string OrderAmount { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string OrderCurrency { get; set; }
        public string CustomerName { get; set; }
    }
    //public interface IOutputCashFree
    //{
    //    CashFreePaymentOutput PaymentOutput { get; set; };
    //    bool Status { get; set; }
    //    string ErrorMessage { get; set; }
    //    string Message { get; set; }

    //}
    public class OutputCashFree: IPaymentResponseModel
    {
        public CashFreePaymentOutput PaymentOutput { get; set; }
        public bool Status { get; set; }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }

    }
}