
using System;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1
{
    public static class Registrations
    {
        public static void RegisterMediatR(this IServiceCollection services) 
        {

services.AddScoped<MediatR.IRequestHandler<ConsoleApp1.ARequest>, ConsoleApp1.AHandler>();
services.AddScoped<MediatR.IRequestHandler<ConsoleApp1.ARequest, MediatR.Unit>, ConsoleApp1.AHandler>();
services.AddScoped<MediatR.IRequestHandler<ConsoleApp1.ARequest2>, ConsoleApp1.AHandler>();
services.AddScoped<MediatR.IRequestHandler<ConsoleApp1.ARequest2, MediatR.Unit>, ConsoleApp1.AHandler>();
services.AddScoped<MediatR.Pipeline.IRequestPreProcessor<ConsoleApp1.ARequest>, ConsoleApp1.ARequestPreProcessor>();
services.AddScoped<MediatR.Pipeline.IRequestPostProcessor<ConsoleApp1.ARequest, MediatR.Unit>, ConsoleApp1.ARequestPostProcessor>();
services.AddScoped<MediatR.Pipeline.IRequestExceptionHandler<ConsoleApp1.ARequest, MediatR.Unit>, ConsoleApp1.AExceptionHandler>();
services.AddScoped<MediatR.Pipeline.IRequestExceptionHandler<ConsoleApp1.ARequest, MediatR.Unit, System.Exception>, ConsoleApp1.AExceptionHandler>();
services.AddScoped<MediatR.Pipeline.IRequestExceptionAction<ConsoleApp1.ARequest>, ConsoleApp1.AExceptionAction>();
services.AddScoped<MediatR.Pipeline.IRequestExceptionAction<ConsoleApp1.ARequest, System.Exception>, ConsoleApp1.AExceptionAction>();

        }
    }
}