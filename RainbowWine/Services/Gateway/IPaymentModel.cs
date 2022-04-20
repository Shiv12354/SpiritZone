using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.Gateway
{
    // Empty interface just to ensure that we get a compile
    // error if we pass a model that does not belong to our
    // payment system.
    public interface IPaymentModel
    {
    }
    public interface IPaymentResponseModel
    {
    }
    public interface IPaymentService
    {
        object MakePayment<T>(T model) where T : IPaymentModel;
        bool AppliesTo(Type provider);
    }

    public interface IPaymentStrategy
    {
        object MakePayment<T>(T model) where T : IPaymentModel;
    }
}