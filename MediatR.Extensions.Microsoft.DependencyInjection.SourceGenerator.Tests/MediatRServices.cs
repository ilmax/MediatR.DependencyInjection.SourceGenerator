using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;

namespace MediatR.Extensions.Microsoft.DependencyInjection.SourceGenerator.Tests
{
    public class AResponse { }
    public class ARequestWithResponse : IRequest<AResponse> { }
    public class ARequest : IRequest { }
    public class ARequest2 : IRequest { }
    public class AHandler : IRequestHandler<ARequest>, IRequestHandler<ARequest2>
    {
        public Task<Unit> Handle(ARequest request, CancellationToken cancellationToken) => throw new NotImplementedException();

        public Task<Unit> Handle(ARequest2 request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
    public class AGenericHandler<TParam> : IRequestHandler<ARequestWithResponse, AResponse> where TParam : AResponse
    {
        public Task<AResponse> Handle(ARequestWithResponse request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
    public abstract class AbstractHandler : IRequestHandler<ARequestWithResponse, AResponse>
    {
        public Task<AResponse> Handle(ARequestWithResponse request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
    class ARequestPreProcessor : IRequestPreProcessor<ARequest>
    {
        public Task Process(ARequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
    class ARequestPostProcessor : IRequestPostProcessor<ARequest, Unit>
    {
        public Task Process(ARequest request, Unit response, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
    class AExceptionHandler : IRequestExceptionHandler<ARequest, Unit>
    {
        public Task Handle(ARequest request, Exception exception, RequestExceptionHandlerState<Unit> state, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
    class AExceptionAction : IRequestExceptionAction<ARequest>
    {
        public Task Execute(ARequest request, Exception exception, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}