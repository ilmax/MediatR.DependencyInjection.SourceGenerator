using System;
using System.Diagnostics;

namespace MediatR.DependencyInjection.SourceGenerator.Registrations
{
    [DebuggerDisplay("{ServiceType} -> {Implementation} ({Lifetime})nq")]
    class GenericRegistration : IEquatable<GenericRegistration>, IRegistration
    {
        public GenericRegistration(string contractType, string implementation, Lifetime lifetime)
        {
            if (string.IsNullOrEmpty(contractType))
            {
                throw new ArgumentException($"'{nameof(contractType)}' cannot be null or empty", nameof(contractType));
            }

            if (string.IsNullOrEmpty(implementation))
            {
                throw new ArgumentException($"'{nameof(implementation)}' cannot be null or empty", nameof(implementation));
            }

            ServiceType = contractType;
            Implementation = implementation;
            Lifetime = lifetime;
        }

        public string ServiceType { get; }

        public string Implementation { get; }

        public Lifetime Lifetime { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as GenericRegistration);
        }

        public override int GetHashCode() => (ServiceType, Implementation).GetHashCode();

        public bool Equals(GenericRegistration other)
        {
            return other != null &&
                   ServiceType == other.ServiceType &&
                   Implementation == other.Implementation;
        }

        public string ToRegistration() => Lifetime switch
        {
            Lifetime.Transient => $"AddTransient<{ServiceType}, {Implementation}>();",
            Lifetime.Scoped => $"AddScoped<{ServiceType}, {Implementation}>();",
            Lifetime.Singleton => $"AddSingleton<{ServiceType}, {Implementation}>();",
            _ => throw new ArgumentOutOfRangeException(nameof(Lifetime)),
        };
    }
}
