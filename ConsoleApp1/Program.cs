using MediatR;
using MediatR.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }



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
}
