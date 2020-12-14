using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FsChat.Utilities.Common.Test
{
    [TestClass]
    public class FileOpTests
    {
        [TestMethod]
        public void TestValidFileExtension()
        {
            var filePath = @"C:\temp\file.pdf";
            var isValid = FileOp.IsFileExtensionValid(filePath, "xls", "xlsx", "pdf");
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void TestInvalidFileExtension()
        {
            var filePath = @"C:\temp\file.bin";
            var isValid = FileOp.IsFileExtensionValid(filePath, "xls", "xlsx", "pdf");
            Assert.IsFalse(isValid);
        }
    }
}
