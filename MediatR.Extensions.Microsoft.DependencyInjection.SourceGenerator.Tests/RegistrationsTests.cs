using System.Linq;
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
                Assert.True(sc2.Any(d => d.ServiceType == descriptor.ServiceType));
            });
        }
    }

    static partial class MediatRServiceExtension
    {
        public static partial void AddMediatRSourceGeneration(this IServiceCollection services);
    }
}
