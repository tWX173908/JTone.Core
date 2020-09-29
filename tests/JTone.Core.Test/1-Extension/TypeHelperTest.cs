using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace JTone.Core.Test
{
    [TestClass]
    public class TypeHelperTest
    {
        [TestMethod]
        public void GetTypeProperties()
        {
            var properties = typeof(TestAttribute).GetTypeProperties();
        }
    }


    internal class TestAttribute
    {
        public string A { get; set; }

        public int B { get; set; }
    }
}
