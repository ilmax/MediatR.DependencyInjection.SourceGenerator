﻿using MediatR.DependencyInjection.SourceGenerator.Registrations;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace MediatR.DependencyInjection.SourceGenerator.Writer
{
    internal class RegistrationWriter
    {
        private readonly RegistrationWriterConfiguration _configuration;

        private static readonly IRegistration[] RequiredRegistrations =
        {
            new DelegateRegistration("MediatR.ServiceFactory", "sp => sp.GetService", Lifetime.Transient),
            new NonGenericRegistration("MediatR.IMediator", "MediatR.Mediator", Lifetime.Transient),
            new DelegateRegistration("MediatR.ISender", "sp => sp.GetService<MediatR.IMediator>()", Lifetime.Transient, useTryAdd:true),
            new DelegateRegistration("MediatR.IPublisher", "sp => sp.GetService<MediatR.IMediator>()", Lifetime.Transient, useTryAdd:true),
            new NonGenericRegistration("MediatR.IPipelineBehavior<,>", "MediatR.Pipeline.RequestPreProcessorBehavior<,>", Lifetime.Transient),
            new NonGenericRegistration("MediatR.IPipelineBehavior<,>", "MediatR.Pipeline.RequestPostProcessorBehavior<,>", Lifetime.Transient),
            new NonGenericRegistration("MediatR.IPipelineBehavior<,>", "MediatR.Pipeline.RequestExceptionActionProcessorBehavior<,>", Lifetime.Transient),
            new NonGenericRegistration("MediatR.IPipelineBehavior<,>", "MediatR.Pipeline.RequestExceptionProcessorBehavior<,>", Lifetime.Transient),
        };

        public RegistrationWriter(RegistrationWriterConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SourceText WriteRegistrations(IEnumerable<IRegistration> discoveredRegistrations, bool isPartial)
        {
            var partialModifier = isPartial ? "partial " : "";
            var sourceBuilder = new StringBuilder($@"
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace {_configuration.Namespace}
{{
    static {partialModifier}class MediatRServiceExtension
    {{
        {_configuration.MethodModifier} static {partialModifier}void {_configuration.MethodName}(this IServiceCollection {_configuration.ServicesParameterName}) 
        {{
");
            var indent = new string(' ', 3 * 4);
            // add the required registrations for the library itself
            foreach (var registration in RequiredRegistrations)
            {
                sourceBuilder.AppendLine($"{indent}{_configuration.ServicesParameterName}.{registration.ToRegistration()}");
            }

            // add the filepath of each tree to the class we're building
            using var enumerator = discoveredRegistrations.GetEnumerator();
            if (enumerator.MoveNext())
            {
                sourceBuilder.Append($"{indent}{_configuration.ServicesParameterName}.{enumerator.Current.ToRegistration()}");
            }

            while (enumerator.MoveNext())
            {
                sourceBuilder.AppendLine();
                sourceBuilder.Append($"{indent}{_configuration.ServicesParameterName}.{enumerator.Current.ToRegistration()}");
            }

            sourceBuilder.AppendLine(@$"
        }}
    }}
}}");
            return SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
        }
    }
}
