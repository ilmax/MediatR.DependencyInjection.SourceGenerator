using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleTables;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var collection1 = new ServiceCollection();
            collection1.AddMediatR(typeof(Program).Assembly);
            DumpRegistrations(collection1);
            InstantiateAllDependencies(collection1.BuildServiceProvider(), collection1);

            var collection2 = new ServiceCollection();
            collection2.AddMediatRRegistrations();
            DumpRegistrations(collection2);
            InstantiateAllDependencies(collection2.BuildServiceProvider(), collection2);

        }

        private static void InstantiateAllDependencies(IServiceProvider serviceProvider, ServiceCollection serviceCollection)
        {
            using var scope = serviceProvider.CreateScope();
            foreach (var serviceDescriptor in serviceCollection)
            {
                if (!serviceDescriptor.ServiceType.IsGenericTypeDefinition)
                {
                    scope.ServiceProvider.GetService(serviceDescriptor.ServiceType);
                }
            }
        }

        private static void DumpRegistrations(ServiceCollection collection)
        {
            var table = new ConsoleTable("Scope", "Service", "Implementation", "Factory");

            foreach (var registration in collection)
            {
                table.AddRow(registration.Lifetime, registration.ServiceType, registration.ImplementationType, registration.ImplementationFactory);
            }

            table.Write();
        }
    }

    public class AResponse { }
    public class ARequestWithResponse : IRequest<AResponse> { }
    public class ARequest : IRequest { }
    public class ARequest2 : IRequest { }
    public class AHandler : IRequestHandler<ARequest>, IRequestHandler<ARequest2>
    {
        public Task<Unit> Handle(ARequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Unit> Handle(ARequest2 request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    public class AGenericHandler<TParam> : IRequestHandler<ARequestWithResponse, AResponse> where TParam : AResponse
    {
        public Task<AResponse> Handle(ARequestWithResponse request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    public abstract class AbstractHandler : IRequestHandler<ARequestWithResponse, AResponse>
    {
        public Task<AResponse> Handle(ARequestWithResponse request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    class ARequestPreProcessor : IRequestPreProcessor<ARequest>
    {
        public Task Process(ARequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    class ARequestPostProcessor : IRequestPostProcessor<ARequest, Unit>
    {
        public Task Process(ARequest request, Unit response, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    class AExceptionHandler : IRequestExceptionHandler<ARequest, Unit>
    {
        public Task Handle(ARequest request, Exception exception, RequestExceptionHandlerState<Unit> state, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    class AExceptionAction : IRequestExceptionAction<ARequest>
    {
        public Task Execute(ARequest request, Exception exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }


    //static partial class MediatRServiceExtension
    //{
    //    public static partial void AddMediatR2(this IServiceCollection services);
    //}
}
