using NUnit.Framework;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class AssemblyFoundationTests
    {
        [Test]
        public void EditModeTestAssembly_LoadsWithExpectedName()
        {
            string assemblyName = typeof(AssemblyFoundationTests).Assembly.GetName().Name;

            Assert.That(assemblyName, Is.EqualTo("Tanvir.SolarSystem.Tests.EditMode"));
        }
    }
}
