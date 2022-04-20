using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.Gateway
{
    public class PaymentStrategy : IPaymentStrategy
    {
        private readonly IPaymentService _paymentServices;

        public PaymentStrategy(IPaymentService paymentServices)
        {
            this._paymentServices = paymentServices ?? throw new ArgumentNullException(nameof(paymentServices));
        }

        public object MakePayment<T>(T model) where T : IPaymentModel => GetPaymentService(model).MakePayment(model);
       
        private IPaymentService GetPaymentService<T>(T model) where T : IPaymentModel
        {
            //var result = _paymentServices.FirstOrDefault(p => p.AppliesTo(model.GetType()));
            //if (result == null)
            var result = _paymentServices;
            if (!result.AppliesTo(model.GetType()))
            {
                throw new InvalidOperationException(
                    $"Payment service for {model.GetType().ToString()} not registered.");
            }
            return result;
        }
    }
}