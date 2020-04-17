﻿

using Microsoft.Extensions.DependencyInjection;

namespace MadPay724.AspNetCore.GateWay.Extentions
{
   public static class Common
    {
        public static IServiceCollection AddMadpay724GateWay(IServiceCollection services)
        {
            return services.AddScoped<IMadPayGateWay, MadPayGateWay>();
        }
    }
}
