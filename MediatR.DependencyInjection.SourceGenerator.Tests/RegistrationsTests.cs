using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MediatR.Extensions.Microsoft.DependencyInjection.SourceGenerator.Tests
{
    public class RegistrationsTests
    {
        [Fact]
        public void RegisterServices_registers_same_number_of_services()
        {
            // Arrange
            var sc1 = new ServiceCollection();
            var sc2 = new ServiceCollection();

            sc1.AddMediatR(typeof(RegistrationsTests));

            // Act
            sc2.AddMediatRSourceGeneration();

            // Assert
            Assert.Equal(sc1.Count, sc2.Count);
        }

        [Fact]
        public void RegisterServices_registers_same_key_services()
        {
            // Arrange
            var sc1 = new ServiceCollection();
            var sc2 = new ServiceCollection();

            sc1.AddMediatR(typeof(RegistrationsTests));

            // Act
            sc2.AddMediatRSourceGeneration();

            // Assert
            Assert.All(sc1, descriptor =>
            {
                Assert.Contains(sc2, d => d.ServiceType == descriptor.ServiceType && d.Lifetime == descriptor.Lifetime);
            });
        }
        
        [Fact]
        public void RegisterServices_registers_same_services()
        {
            // Arrange
            var sc1 = new ServiceCollection();
            var sc2 = new ServiceCollection();

            sc1.AddMediatR(typeof(RegistrationsTests));
            var comparer = new ServiceDescriptorEqualityComparer();

            // Act
            sc2.AddMediatRSourceGeneration();

            // Assert
            Assert.All(sc1, descriptor =>
            {
                Assert.Contains(sc2, serviceDescriptor => comparer.Equals(descriptor, serviceDescriptor));
            });
        }

        class ServiceDescriptorEqualityComparer : IEqualityComparer<ServiceDescriptor>
        {
            public bool Equals(ServiceDescriptor x, ServiceDescriptor y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;

                return x.Lifetime == y.Lifetime && x.ServiceType == y.ServiceType && x.ImplementationType == y.ImplementationType && Equals(x.ImplementationInstance, y.ImplementationInstance) && Equals(x.ImplementationFactory, y.ImplementationFactory);
            }

            public int GetHashCode(ServiceDescriptor obj)
            {
                return HashCode.Combine((int) obj.Lifetime, obj.ServiceType, obj.ImplementationType, obj.ImplementationInstance, obj.ImplementationFactory);
            }
        }
    }

    static partial class MediatRServiceExtension
    {
        public static partial void AddMediatRSourceGeneration(this IServiceCollection services);
    }
}
