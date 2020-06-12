using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MediatR.DependencyInjection.SourceGenerator
{
    [Generator]
    public class MySourceGenerator : ISourceGenerator
    {
        private static Dictionary<INamedTypeSymbol, Lifetime> _wellKnownInterfacesLifetime = new Dictionary<INamedTypeSymbol, Lifetime>();

        public void Execute(SourceGeneratorContext context)
        {
            // Debugger.Launch(); Uncomment this to debug
            InitializeWllKnownInterfaces(context.Compilation, _wellKnownInterfacesLifetime);
            GetAllSymbolsVisitor visitor = new GetAllSymbolsVisitor(_wellKnownInterfacesLifetime);
            visitor.Visit(context.Compilation.Assembly.GlobalNamespace);
            WriteRegistrations(visitor.Registrations, context);
        }

        private void WriteRegistrations(IEnumerable<Registration> registrations, SourceGeneratorContext context)
        {
            var sourceBuilder = new StringBuilder($@"
using System;
using Microsoft.Extensions.DependencyInjection;

namespace {context.Compilation.AssemblyName}");

            sourceBuilder.AppendLine(
@"
{
    public static class Registrations
    {
        public static void RegisterMediatR(this IServiceCollection services) 
        {
");
            // add the filepath of each tree to the class we're building
            foreach (var registration in registrations)
            {
                sourceBuilder.AppendLine($"services.{registration.ToRegistration()}");
            }

            // finish creating the source to inject
            sourceBuilder.Append(@"
        }
    }
}");
            var source = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
            context.AddSource("MediatRRegistration", source);

            // TODO Add support for writing this one with either environment variable or csproj file option
            // TODO Add discovery support for classes outside this compilation (i.e. in another project)
            // TODO Add support for configure the class name + the namespace
            File.WriteAllText("MediatRRegistration.cs", source.ToString());
        }

        private void InitializeWllKnownInterfaces(Compilation compilation, Dictionary<INamedTypeSymbol, Lifetime> wellKnownInterfacesLifetime)
        {
            Add(compilation, "MediatR.IRequestHandler`2", Lifetime.Scoped, wellKnownInterfacesLifetime);
            Add(compilation, "MediatR.IRequestHandler`1", Lifetime.Scoped, wellKnownInterfacesLifetime);
            Add(compilation, "MediatR.INotificationHandler`1", Lifetime.Scoped, wellKnownInterfacesLifetime);
            Add(compilation, "MediatR.Pipeline.IRequestPreProcessor`1", Lifetime.Scoped, wellKnownInterfacesLifetime);
            Add(compilation, "MediatR.Pipeline.IRequestPostProcessor`2", Lifetime.Scoped, wellKnownInterfacesLifetime);
            Add(compilation, "MediatR.Pipeline.IRequestExceptionHandler`3", Lifetime.Scoped, wellKnownInterfacesLifetime);
            Add(compilation, "MediatR.Pipeline.IRequestExceptionHandler`2", Lifetime.Scoped, wellKnownInterfacesLifetime);
            Add(compilation, "MediatR.Pipeline.IRequestExceptionAction`2", Lifetime.Scoped, wellKnownInterfacesLifetime);
            Add(compilation, "MediatR.Pipeline.IRequestExceptionAction`1", Lifetime.Scoped, wellKnownInterfacesLifetime);

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
    }
}
