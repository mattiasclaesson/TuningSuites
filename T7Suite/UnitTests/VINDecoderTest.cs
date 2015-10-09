using CommonSuite;

using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for VINDecoderTest and is intended
    ///to contain all VINDecoderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VINDecoderTest
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
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTest()
        {
            VINDecoder target = new VINDecoder();
            string VINNumber = "YS3EB55A143012475";
            VINCarInfo actual;
            actual = target.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95, actual.CarModel);
            Assert.AreEqual(2004, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B235L, actual.EngineType);
            Assert.AreEqual(VINTurboModel.GarrettT17, actual.TurboModel);
            Assert.AreEqual("5 speed manual / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-5)", actual.PlantInfo);
            Assert.AreEqual("Model series I, Driver and passenger airbags", actual.Series);
            // Serialnumber not decoded 012475
        }
    }
}
