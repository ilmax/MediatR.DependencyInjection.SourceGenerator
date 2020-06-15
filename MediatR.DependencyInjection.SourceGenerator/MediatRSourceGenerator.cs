using MediatR.DependencyInjection.SourceGenerator.Discovery;
using MediatR.DependencyInjection.SourceGenerator.Registrations;
using MediatR.DependencyInjection.SourceGenerator.Writer;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MediatR.DependencyInjection.SourceGenerator
{
    // TODO Add support for writing this one with either environment variable or csproj file option (Partially done via the DEBUG_SOURCE_GENERATOR environment variable)
    // TODO Add discovery support for classes outside this compilation (i.e. in another project)
    // TODO Add support for configure the class name + the namespace (Namespace configuration is partially done via a class name lookup using MediatRSourceGenerator.ClassName)
    // TODO De-dup registrations
    [Generator]
    public class MediatRSourceGenerator : ISourceGenerator
    {
        private static Dictionary<INamedTypeSymbol, Lifetime> _wellKnownInterfacesLifetime = new Dictionary<INamedTypeSymbol, Lifetime>();
        public const string ClassName = "MediatRServiceExtension";
        private static string GeneratedClassName = $"{ClassName}.generated.cs";
        private static bool? _isDebugEnabled;

        public void Execute(SourceGeneratorContext context)
        {
            //Debugger.Launch(); //Uncomment this to debug
            InitializeMediatRInterfaces(context.Compilation, _wellKnownInterfacesLifetime);
            RegistrationDiscovererSymbolVisitor visitor = new RegistrationDiscovererSymbolVisitor(_wellKnownInterfacesLifetime);
            visitor.Visit(context.Compilation.Assembly.GlobalNamespace);
            var writer = new RegistrationWriter();
            var discoveredClassNamespace = visitor.RegistrationClassNamespace;

            var source = writer.WriteRegistrations(visitor.Registrations, discoveredClassNamespace ?? context.Compilation.AssemblyName, !string.IsNullOrEmpty(discoveredClassNamespace));

            if (Debug)
            {
                File.WriteAllText(GeneratedClassName, source.ToString());
            }
            else
            {
                context.AddSource(GeneratedClassName, source);
            }
        }

        private void InitializeMediatRInterfaces(Compilation compilation, Dictionary<INamedTypeSymbol, Lifetime> wellKnownInterfacesLifetime)
        {
            if (wellKnownInterfacesLifetime.Count == 0)
            {
                Add(compilation, "MediatR.IRequestHandler`2", Lifetime.Transient, wellKnownInterfacesLifetime);
                Add(compilation, "MediatR.INotificationHandler`1", Lifetime.Transient, wellKnownInterfacesLifetime);
                Add(compilation, "MediatR.Pipeline.IRequestPreProcessor`1", Lifetime.Transient, wellKnownInterfacesLifetime);
                Add(compilation, "MediatR.Pipeline.IRequestPostProcessor`2", Lifetime.Transient, wellKnownInterfacesLifetime);
                Add(compilation, "MediatR.Pipeline.IRequestExceptionHandler`2", Lifetime.Transient, wellKnownInterfacesLifetime);
                Add(compilation, "MediatR.Pipeline.IRequestExceptionAction`2", Lifetime.Transient, wellKnownInterfacesLifetime);
            }

            static void Add(Compilation compilation, string type, Lifetime lifetime, Dictionary<INamedTypeSymbol, Lifetime> wellKnownInterfacesLifetime)
            {
                var namedSymbol = compilation.GetTypeByMetadataName(type);
                if (namedSymbol != null)
                {
                    wellKnownInterfacesLifetime.Add(namedSymbol, lifetime);
                }
            }
        }

        public void Initialize(InitializationContext context)
        {
        }

        public bool Debug => _isDebugEnabled ??= Environment.GetEnvironmentVariable("DEBUG_SOURCE_GENERATOR")?.Trim() == "1";
    }
}
