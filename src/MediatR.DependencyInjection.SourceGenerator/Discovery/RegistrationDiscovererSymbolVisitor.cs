using System;
using MediatR.DependencyInjection.SourceGenerator.Registrations;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using MediatR.DependencyInjection.SourceGenerator.Writer;

namespace MediatR.DependencyInjection.SourceGenerator.Discovery
{
    class RegistrationDiscovererSymbolVisitor : SymbolVisitor
    {
        private static readonly SymbolDisplayFormat SymbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
        private readonly HashSet<IRegistration> _registrations = new HashSet<IRegistration>();
        private readonly Dictionary<INamedTypeSymbol, Lifetime> _wellKnownInterfaces;

        private readonly string _sourceGeneratorClassName;
        private readonly ITypeSymbol _serviceCollectionTypeSymbol;

        public RegistrationDiscovererSymbolVisitor(Dictionary<INamedTypeSymbol, Lifetime> wellKnownInterfaces, string sourceGeneratorClassName, ITypeSymbol serviceCollectionTypeSymbol)
        {
            _wellKnownInterfaces = wellKnownInterfaces;
            _sourceGeneratorClassName = sourceGeneratorClassName;
            _serviceCollectionTypeSymbol = serviceCollectionTypeSymbol;
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            // TODO Use parallel foreach
            foreach (var member in symbol.GetMembers())
            {
                member.Accept(this);
            }

            //Parallel.ForEach(symbol.GetMembers(), s => s.Accept(this));
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            if (symbol.TypeKind == TypeKind.Class && !symbol.IsGenericType && !symbol.IsAbstract)
            {
                foreach (var implementedInterface in symbol.AllInterfaces)
                {
                    if (IsMediatRInterface(implementedInterface, out var lifetime))
                    {
                        if (!_registrations.Add(ConstructRegistration(symbol, implementedInterface, lifetime)))
                        {
                            // LOG something here
                        }
                    }
                }

                if (symbol.Name == _sourceGeneratorClassName)
                {
                    WriterConfig = InitializeWriterConfiguration(symbol);
                }
            }
        }

        private RegistrationWriterConfiguration InitializeWriterConfiguration(INamedTypeSymbol symbol)
        {
            foreach (var member in symbol.GetMembers())
            {
                if (member.Kind == SymbolKind.Method && member.IsStatic && member is IMethodSymbol methodSymbol && methodSymbol.IsExtensionMethod)
                {
                    foreach (var methodSymbolParameter in methodSymbol.Parameters)
                    {
                        if (SymbolEqualityComparer.Default.Equals(methodSymbolParameter.Type, _serviceCollectionTypeSymbol))
                        {
                            return new RegistrationWriterConfiguration(
                                symbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat),
                                _sourceGeneratorClassName,
                                ToStringAccessibility(methodSymbol.DeclaredAccessibility),
                                methodSymbol.Name,
                                methodSymbolParameter.Name);
                        }
                    }
                }
            }

            return null;
        }

        private static string ToStringAccessibility(Accessibility methodSymbolDeclaredAccessibility)
        {
            return methodSymbolDeclaredAccessibility switch
            {
                Accessibility.Internal => "internal",
                Accessibility.Private => "internal",
                Accessibility.Public => "public",
                _ => throw new ArgumentOutOfRangeException(nameof(methodSymbolDeclaredAccessibility), methodSymbolDeclaredAccessibility, null)
            };
        }

        private bool IsMediatRInterface(INamedTypeSymbol implementedInterface, out Lifetime lifetime)
        {
            if (implementedInterface.IsGenericType)
            {
                return _wellKnownInterfaces.TryGetValue(implementedInterface.ConstructedFrom, out lifetime);
            }

            lifetime = default;
            return false;
        }

        private GenericRegistration ConstructRegistration(INamedTypeSymbol type, INamedTypeSymbol implementedInterface, Lifetime lifetime) =>
            new GenericRegistration(implementedInterface.ToString(), type.ToString(), lifetime);

        public IEnumerable<IRegistration> Registrations => _registrations;

        public RegistrationWriterConfiguration WriterConfig { get; private set; }
    }
}
