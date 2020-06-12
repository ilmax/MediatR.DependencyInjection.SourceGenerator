using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MediatR.DependencyInjection.SourceGenerator
{
    [DebuggerDisplay("{ContractType} -> {Implementation} ({Lifetime})nq")]
    public class Registration : IEquatable<Registration>
    {
        public Registration(string contractType, string implementation, Lifetime lifetime)
        {
            if (string.IsNullOrEmpty(contractType))
            {
                throw new ArgumentException($"'{nameof(contractType)}' cannot be null or empty", nameof(contractType));
            }

            if (string.IsNullOrEmpty(implementation))
            {
                throw new ArgumentException($"'{nameof(implementation)}' cannot be null or empty", nameof(implementation));
            }

            ContractType = contractType;
            Implementation = implementation;
            Lifetime = lifetime;
        }

        public string ContractType { get; }

        public string Implementation { get; }

        public Lifetime Lifetime { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Registration);
        }

        public override int GetHashCode()
        {
            return ContractType.GetHashCode() ^ Implementation.GetHashCode();
        }

        public bool Equals(Registration other)
        {
            return other != null &&
                   ContractType == other.ContractType &&
                   Implementation == other.Implementation;
        }

        public static bool operator ==(Registration left, Registration right)
        {
            return EqualityComparer<Registration>.Default.Equals(left, right);
        }

        public static bool operator !=(Registration left, Registration right)
        {
            return !(left == right);
        }

        public string ToRegistration() => Lifetime switch
        {
            Lifetime.Transient => $"AddTransient<{ContractType}, {Implementation}>();",
            Lifetime.Scoped => $"AddScoped<{ContractType}, {Implementation}>();",
            Lifetime.Singleton => $"AddSingleton<{ContractType}, {Implementation}>();",
            _ => throw new ArgumentOutOfRangeException(nameof(Lifetime)),
        };
    }
}
