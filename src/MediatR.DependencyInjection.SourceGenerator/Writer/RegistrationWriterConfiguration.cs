namespace MediatR.DependencyInjection.SourceGenerator.Writer
{
    public class RegistrationWriterConfiguration
    {
        public RegistrationWriterConfiguration(string ns, string className, string methodModifier, string methodName, string servicesParameterName)
        {
            Namespace = ns;
            ClassName = className;
            MethodModifier = methodModifier;
            MethodName = methodName;
            ServicesParameterName = servicesParameterName;
        }

        public string Namespace { get; }
        public string ClassName { get; }
        public string MethodModifier { get; }
        public string MethodName { get; }
        public string ServicesParameterName { get; }
    }
}