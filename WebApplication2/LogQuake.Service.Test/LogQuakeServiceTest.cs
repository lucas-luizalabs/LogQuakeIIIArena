using LogQuake.Service.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogQuake.Service.Test
{
    [TestClass]
    public class LogQuakeServiceTest
    {
        private readonly LogQuakeService _LogQuakeService;

        public LogQuakeServiceTest()
        {
            _LogQuakeService = new LogQuakeService();
        }

        [TestMethod]
        public void BuscaPaginada()
        {
            var result = _LogQuakeService.GetAll(null);

            Assert.AreEqual(result, null);

            //Assert.IsFalse(result == null, "1 should not be prime");
        }
    }
}
