using MediatR.DependencyInjection.SourceGenerator.Registrations;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace MediatR.DependencyInjection.SourceGenerator.Discovery
{
    class RegistrationDiscovererSymbolVisitor : SymbolVisitor
    {
        private readonly HashSet<IRegistration> _registrations = new HashSet<IRegistration>();
        private readonly Dictionary<INamedTypeSymbol, Lifetime> _wellknownInterfaces;
        private static readonly SymbolDisplayFormat symbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        public RegistrationDiscovererSymbolVisitor(Dictionary<INamedTypeSymbol, Lifetime> wellknownInterfaces)
        {
            _wellknownInterfaces = wellknownInterfaces;
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

                if (symbol.Name == MediatRSourceGenerator.ClassName)
                {
                    RegistrationClassNamespace = symbol.ContainingNamespace.ToDisplayString(symbolDisplayFormat);
                }
            }
        }

        private bool IsMediatRInterface(INamedTypeSymbol implementedInterface, out Lifetime lifetime)
        {
            if (implementedInterface.IsGenericType)
            {
                return _wellknownInterfaces.TryGetValue(implementedInterface.ConstructedFrom, out lifetime);
            }

            lifetime = default;
            return false;
        }

        private GenericRegistration ConstructRegistration(INamedTypeSymbol type, INamedTypeSymbol implementedInterface, Lifetime lifetime) =>
            new GenericRegistration(implementedInterface.ToString(), type.ToString(), lifetime);

        public IEnumerable<IRegistration> Registrations => _registrations;

        public string RegistrationClassNamespace { get; private set; }
    }
}
