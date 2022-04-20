using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.Gateway
{
    public abstract class PaymentService<TModel> : IPaymentService
    where TModel : IPaymentModel

    {
        public virtual bool AppliesTo(Type provider)
        {
            return typeof(TModel).Equals(provider);
        }

        public object MakePayment<T>(T model) where T : IPaymentModel
        {
            return MakePayment((TModel)(object)model);
        }

        protected abstract object MakePayment(TModel model);
    }
}