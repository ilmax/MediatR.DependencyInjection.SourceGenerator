using System.Collections.Generic;
using System.Diagnostics;
using MediatR.DependencyInjection.SourceGenerator.Discovery;
using MediatR.DependencyInjection.SourceGenerator.Registrations;
using MediatR.DependencyInjection.SourceGenerator.Writer;
using Microsoft.CodeAnalysis;

namespace MediatR.DependencyInjection.SourceGenerator
{
    // TODO Verify support for classes outside this compilation (i.e. in another project)
    // TODO Report failures/diagnostics properly
    // TODO De-dup registrations
    // TODO? Use proper Roslyn formatting
    /// <summary>
    /// A source generator used to generate a class a MediatR registration class at compile time, the source file can be inspecting the <see cref="https://github.com/dotnet/roslyn/blob/master/docs/features/source-generators.md#output-files">output files</see>
    /// </summary>
    [Generator]
    public class MediatRSourceGenerator : ISourceGenerator
    {
        private static readonly Dictionary<INamedTypeSymbol, Lifetime> WellKnownInterfacesLifetime = new Dictionary<INamedTypeSymbol, Lifetime>();
        private const string ServiceCollectionTypeName = "Microsoft.Extensions.DependencyInjection.IServiceCollection";
        private string _className = "MediatRServiceExtension";
        private string _methodName = "AddMediatRRegistrations";

        private string _defaultAccessModifier = "internal";
        private string _defaultServicesParameterName = "services";

        private string _classNamespace;
        private string _generatedFileName;

        public void Execute(GeneratorExecutionContext context)
        {
            //Debugger.Launch(); //Uncomment this to debug

            InitializeSourceGenerator(context);

            var visitor = new RegistrationDiscovererSymbolVisitor(WellKnownInterfacesLifetime, _className, context.Compilation.GetTypeByMetadataName(ServiceCollectionTypeName));
            visitor.Visit(context.Compilation.Assembly.GlobalNamespace);

            var writerConfig = visitor.WriterConfig ?? new RegistrationWriterConfiguration(_classNamespace ?? context.Compilation.AssemblyName, _className, _defaultAccessModifier, _methodName, _defaultServicesParameterName);

            var writer = new RegistrationWriter(writerConfig);

            var source = writer.WriteRegistrations(visitor.Registrations, visitor.WriterConfig != null);

            context.AddSource(_generatedFileName, source);
        }

        private void InitializeSourceGenerator(GeneratorExecutionContext context)
        {
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{nameof(MediatRSourceGenerator)}_ClassNamespace", out var classNamespace))
            {
                _classNamespace = classNamespace;
            }

            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{nameof(MediatRSourceGenerator)}_ClassName", out var className))
            {
                _className = className;
            }

            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{nameof(MediatRSourceGenerator)}_MethodName", out var methodName))
            {
                _methodName = methodName;
            }

            _generatedFileName = $"{_className}.g.cs";
            InitializeMediatRInterfaces(context.Compilation, WellKnownInterfacesLifetime);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        private void InitializeMediatRInterfaces(Compilation compilation, Dictionary<INamedTypeSymbol, Lifetime> wellKnownInterfacesLifetime)
        {
            if (wellKnownInterfacesLifetime.Count == 0)
            {
                Add(compilation, "MediatR.IRequestHandler`2", Lifetime.Transient, wellKnownInterfacesLifetime);
                Add(compilation, "MediatR.INotificationHandler`1", Lifetime.Transient, wellKnownInterfacesLifetime);
                Add(compilation, "MediatR.Pipeline.IRequestPreProcessor`1", Lifetime.Transient, wellKnownInterfacesLifetime);
                Add(compilation, "MediatR.Pipeline.IRequestPostProcessor`2", Lifetime.Transient, wellKnownInterfacesLifetime);
                Add(compilation, "MediatR.Pipeline.IRequestExceptionHandler`3", Lifetime.Transient, wellKnownInterfacesLifetime);
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
    }
}
