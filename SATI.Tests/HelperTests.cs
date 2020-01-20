using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace SATI.Tests
{
    [TestClass]
    public class HelperTests
    {
        [TestMethod]
        public void TestPropertyName()
        {

            var displayName = Helpers.ReflectionExtensions.GetPropertyDisplayName<Test>(p => p.Name);

            Assert.AreEqual("Test Name", displayName);
        }
    }

    public class Test
    {
        [DisplayName("Test Name")]
        public string Name { get; set; }
    }
}
