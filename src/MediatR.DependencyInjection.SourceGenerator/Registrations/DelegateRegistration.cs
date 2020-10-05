using System;
using System.Diagnostics;

namespace MediatR.DependencyInjection.SourceGenerator.Registrations
{
    [DebuggerDisplay("{ServiceType} -> {ImplementationFactory} ({Lifetime})nq")]
    class DelegateRegistration : IRegistration, IEquatable<DelegateRegistration>
    {
        public DelegateRegistration(string serviceType, string implementationFactory, Lifetime lifetime)
        {
            if (string.IsNullOrEmpty(serviceType))
            {
                throw new ArgumentException($"'{nameof(serviceType)}' cannot be null or empty", nameof(serviceType));
            }

            if (string.IsNullOrEmpty(implementationFactory))
            {
                throw new ArgumentException($"'{nameof(implementationFactory)}' cannot be null or empty", nameof(implementationFactory));
            }

            ServiceType = serviceType;
            ImplementationFactory = implementationFactory;
            Lifetime = lifetime;
        }

        public string ServiceType { get; }

        public string ImplementationFactory { get; }

        public Lifetime Lifetime { get; }

        public override int GetHashCode() => (ServiceType, ImplementationFactory).GetHashCode();

        public override bool Equals(object obj) => Equals(obj as DelegateRegistration);

        public bool Equals(DelegateRegistration other)
        {
            return other != null &&
                   ServiceType == other.ServiceType &&
                   ImplementationFactory == other.ImplementationFactory;
        }

        public string ToRegistration() => Lifetime switch
        {
            Lifetime.Transient => $"AddTransient<{ServiceType}>({ImplementationFactory});",
            Lifetime.Scoped => $"AddScoped<{ServiceType}>({ImplementationFactory});",
            Lifetime.Singleton => $"AddSingleton<{ServiceType}>({ImplementationFactory});",
            _ => throw new ArgumentOutOfRangeException(nameof(Lifetime)),
        };
    }
}
