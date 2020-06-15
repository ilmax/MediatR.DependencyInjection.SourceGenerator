using MediatR.DependencyInjection.SourceGenerator.Registrations;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace MediatR.DependencyInjection.SourceGenerator.Writer
{
    class RegistrationWriter
    {
        private static readonly IRegistration[] _requiredRegistrations = new IRegistration[]
        {
            new DelegateRegistration("MediatR.ServiceFactory", "p => p.GetService", Lifetime.Transient),
            new NonGenericRegistration("MediatR.IPipelineBehavior<,>", "MediatR.Pipeline.RequestPreProcessorBehavior<,>", Lifetime.Transient),
            new NonGenericRegistration("MediatR.IPipelineBehavior<,>", "MediatR.Pipeline.RequestPostProcessorBehavior<,>", Lifetime.Transient),
            new NonGenericRegistration("MediatR.IPipelineBehavior<,>", "MediatR.Pipeline.RequestExceptionActionProcessorBehavior<,>", Lifetime.Transient),
            new NonGenericRegistration("MediatR.IPipelineBehavior<,>", "MediatR.Pipeline.RequestExceptionProcessorBehavior<,>", Lifetime.Transient),
            new NonGenericRegistration("MediatR.IMediator", "MediatR.Pipeline.RequestExceptionProcessorBehavior<,>", Lifetime.Transient),
        };

        public SourceText WriteRegistrations(IEnumerable<IRegistration> discoveredRegistrations, string namespaceName, bool isPartial)
        {
            var partialModifier = isPartial ? "partial ": "";
            var sourceBuilder = new StringBuilder($@"
using System;
using Microsoft.Extensions.DependencyInjection;

namespace {namespaceName}
{{
    static {partialModifier}class MediatRServiceExtension
    {{
        internal static {partialModifier}void AddMediatR2(this IServiceCollection services) 
        {{
");

            // add the required registrations for the library itself
            foreach (var registration in _requiredRegistrations)
            {
                sourceBuilder.AppendLine($"services.{registration.ToRegistration()}");
            }

            // add the filepath of each tree to the class we're building
            foreach (var registration in discoveredRegistrations)
            {
                sourceBuilder.AppendLine($"services.{registration.ToRegistration()}");
            }

            sourceBuilder.AppendLine($@"
        }}
    }}
}}");
            return SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
        }
    }
}
