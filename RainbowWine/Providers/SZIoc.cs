using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SZData;
using SZData.Interfaces;
using SZData.Repo;
using SZInfrastructure;

namespace RainbowWine.Providers
{
    public class SZIoc
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient(typeof(IConfigService), typeof(ConfigService));
            services.AddTransient(typeof(IConfigRepo), typeof(ConfigRepo));
            services.AddTransient(typeof(ISZConfiguration), typeof(SZConfiguration));
            services.AddTransient(typeof(IPageService), typeof(PageService));
            services.AddTransient(typeof(IPageRepo), typeof(PageRepo));
            services.AddTransient(typeof(ISZPromoCode), typeof(SZPromoCode));
            services.AddTransient(typeof(IPromoCodeService), typeof(PromoCodeService));
            services.AddTransient(typeof(IPromoCodeRepo), typeof(PromoCodeRepo));
            services.AddTransient(typeof(IDeliveryEarningRepo), typeof(DeliveryEarningRepo));
            services.AddTransient(typeof(IDeliveryEarningService), typeof(DeliveryEarningService));
            services.AddTransient(typeof(IProductRepo), typeof(ProductRepo));
            services.AddTransient(typeof(IProductService), typeof(ProductService));
            services.AddTransient(typeof(ICartRepo), typeof(CartRepo));
            services.AddTransient(typeof(ICartService), typeof(CartService));

        }

        public static T GetSerivce<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

    }
}