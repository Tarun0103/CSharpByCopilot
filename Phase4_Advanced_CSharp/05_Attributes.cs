using System;
using System.Linq;

namespace Phase4_Advanced_CSharp
{
    internal class AttributesExamples
    {
        private static void Main()
        {
            // Built-in attribute example
            LegacyMethod();

            // Read custom attributes via reflection
            var type = typeof(SampleService);
            var attribute = type.GetCustomAttributes(typeof(ServiceInfoAttribute), inherit: false)
                .Cast<ServiceInfoAttribute>()
                .FirstOrDefault();

            if (attribute != null)
            {
                Console.WriteLine($"Service: {attribute.Name}, Version: {attribute.Version}");
            }
            else
            {
                Console.WriteLine("No ServiceInfoAttribute found.");
            }
        }

        [Obsolete("Use NewMethod instead.")]
        private static void LegacyMethod()
        {
            Console.WriteLine("Legacy method called.");
        }

        [ServiceInfo("ExampleService", "1.0")]
        private class SampleService
        {
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
        private class ServiceInfoAttribute : Attribute
        {
            public string Name { get; }
            public string Version { get; }

            public ServiceInfoAttribute(string name, string version)
            {
                Name = name;
                Version = version;
            }
        }
    }
}
