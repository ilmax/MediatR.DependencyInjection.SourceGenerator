using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace MediatR.DependencyInjection.SourceGenerator
{
    public class GetAllSymbolsVisitor : SymbolVisitor
    {
        private readonly HashSet<Registration> _registrations = new HashSet<Registration>();
        private readonly Dictionary<INamedTypeSymbol, Lifetime> _wellknownInterfaces;

        public GetAllSymbolsVisitor(Dictionary<INamedTypeSymbol, Lifetime> wellknownInterfaces)
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
            if (symbol.TypeKind == TypeKind.Class && !symbol.IsUnboundGenericType)
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

        private Registration ConstructRegistration(INamedTypeSymbol type, INamedTypeSymbol implementedInterface, Lifetime lifetime) =>
            new Registration(implementedInterface.ToString(), type.ToString(), lifetime);

        public IEnumerable<Registration> Registrations => _registrations;
    }
}
