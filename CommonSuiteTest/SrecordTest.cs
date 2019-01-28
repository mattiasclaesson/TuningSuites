using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonSuite;
using TrionicCANLib.Firmware;
using System.IO;

namespace CommonSuiteTest
{
    /// <summary>
    ///This is a test class for SrecordTest and is intended
    ///to contain all SrecordTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SrecordTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Convert AppTool T7 s19 file to bin
        ///</summary>
        [TestMethod(), DeploymentItem("Resources//t7.s19")]
        public void ConvertAppToolT7s19()
        {
            const string inputS19Filename = "t7.s19";
            string actualBinFilename;
            Srecord srecord = new Srecord();
            srecord.ConvertSrecToBin(inputS19Filename, FileT7.Length, out actualBinFilename);

            Assert.AreEqual("t7.bin", actualBinFilename);
            Assert.IsTrue(File.Exists(actualBinFilename));
            FileInfo info = new FileInfo(actualBinFilename);
            Assert.AreEqual(FileT7.Length, info.Length);
        }

        /// <summary>
        ///A test for Convert AppTool T8 s19 file to bin
        ///</summary>
        [TestMethod(), DeploymentItem("Resources//t8_application.s19")]
        public void ConvertAppToolT8s19()
        {
            const string inputS19Filename = "t8_application.s19";
            string actualBinFilename;
            Srecord srecord = new Srecord();
            srecord.ConvertSrecToBin(inputS19Filename, FileT8.Length, out actualBinFilename);

            Assert.AreEqual("t8_application.bin", actualBinFilename);
            Assert.IsTrue(File.Exists(actualBinFilename));
            FileInfo info = new FileInfo(actualBinFilename);
            Assert.AreEqual(FileT8.Length, info.Length);
        }
    }
}
