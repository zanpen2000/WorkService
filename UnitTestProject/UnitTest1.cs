using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkService;


namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DocumentService ds = new DocumentService();
            ds.__getNewFilename("", DateTime.Now.Date);
        }
    }
}
