using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonSuite;

namespace CommonSuiteTest
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
        public void DecodeVINNumberTestYS3EB55A143012475()
        {
            
            const string VINNumber = "YS3EB55A143012475";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95, actual.CarModel);
            Assert.AreEqual(2004, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B235L, actual.EngineType);
            Assert.AreEqual(VINTurboModel.MitsubishiTD04HL_15T_5, actual.TurboModel);
            Assert.AreEqual("5 speed manual / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-5)", actual.PlantInfo);
            Assert.AreEqual("Model series I, Driver and passenger airbags", actual.Series);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 012475
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3EH55GX63510826()
        {
            
            const string VINNumber = "YS3EH55GX63510826";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95, actual.CarModel);
            Assert.AreEqual(2006, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B235R, actual.EngineType);
            Assert.AreEqual(VINTurboModel.MitsubishiTD04HL_15T_5, actual.TurboModel);
            Assert.AreEqual("5 speed manual / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-5)", actual.PlantInfo);
            Assert.AreEqual("Model series IV, Driver and passenger airbags", actual.Series);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 510826
        }
        
         /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3FM52RX81130440()
        {
            
            const string VINNumber = "YS3FM52RX81130440";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab93new, actual.CarModel);
            Assert.AreEqual(2008, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B284R, actual.EngineType);
            Assert.AreEqual(VINTurboModel.MitsubishiTD04HL_15TK, actual.TurboModel);
            Assert.AreEqual("6 speed automatic / all wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-3)", actual.PlantInfo);
            Assert.AreEqual("Saab 9-3 TurboX", actual.Series);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 130440
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3EB59A553505541()
        {
            
            const string VINNumber = "YS3EB59A553505541";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95, actual.CarModel);
            Assert.AreEqual(2005, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B235L, actual.EngineType);
            Assert.AreEqual(VINTurboModel.MitsubishiTD04HL_15T_5, actual.TurboModel);
            Assert.AreEqual("5 speed automatic / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-5)", actual.PlantInfo);
            Assert.AreEqual("Model series I, Driver and passenger airbags", actual.Series);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 505541
        }
        
        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3GR4BJ0B4001333()
        {
            
            const string VINNumber = "YS3GR4BJ0B4001333";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95new, actual.CarModel);
            Assert.AreEqual(2011, actual.Makeyear);
            Assert.AreEqual("4 door sedan (SN)", actual.Body);
            Assert.AreEqual(VINEngineType.A28NER_LAU, actual.EngineType);
            Assert.AreEqual(VINTurboModel.MitsubishiTD04HL_19TK3, actual.TurboModel);
            Assert.AreEqual("6 speed automatic / all wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan (9-5)", actual.PlantInfo);
            Assert.AreEqual("Saab 9-5 Aero", actual.Series);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 001333
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3FD5NT8B1306559()
        {
            
            const string VINNumber = "YS3FD5NT8B1306559";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab93new, actual.CarModel);
            Assert.AreEqual(2011, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B207S, actual.EngineType);
            Assert.AreEqual(VINTurboModel.MitsubishiTD04L_14T, actual.TurboModel);
            Assert.AreEqual("6 speed manual / all wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-3)", actual.PlantInfo);
            Assert.AreEqual("Saab 9-3 X", actual.Series);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 306559
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3GP4AR2B4002240()
        {
            
            const string VINNumber = "YS3GP4AR2B4002240";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95new, actual.CarModel);
            Assert.AreEqual(2011, actual.Makeyear);
            Assert.AreEqual("4 door sedan (SN)", actual.Body);
            Assert.AreEqual(VINEngineType.A20NFT_LHU_BP, actual.EngineType);
            Assert.AreEqual(VINTurboModel.BorgWarnerK04_2277DCB, actual.TurboModel);
            Assert.AreEqual("6 speed automatic / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan (9-5)", actual.PlantInfo);
            Assert.AreEqual("Saab 9-5 Vector", actual.Series);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 002240
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYSCFB51W482301688()
        {
            
            const string VINNumber = "YSCFB51W482301688";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.CadillacBTS, actual.CarModel);
            Assert.AreEqual(2008, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.Z19DTH, actual.EngineType);
            Assert.AreEqual(VINTurboModel.Unknown, actual.TurboModel);
            Assert.AreEqual("6 speed automatic / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line B (900 / 9-3)", actual.PlantInfo);
            Assert.AreEqual("Model series I, Driver and passenger airbags", actual.Series);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 301688
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYSCFD56S782300776()
        {
            
            const string VINNumber = "YSCFD56S782300776";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.CadillacBTS, actual.CarModel);
            Assert.AreEqual(2008, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B207L, actual.EngineType);
            Assert.AreEqual(VINTurboModel.Unknown, actual.TurboModel);
            Assert.AreEqual("6 speed manual / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line B (900 / 9-3)", actual.PlantInfo);
            Assert.AreEqual("Model series II, Driver and passenger airbags", actual.Series);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 300776
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestW0L0ZCF3551095720()
        {
            
            const string VINNumber = "W0L0ZCF3551095720";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.OpelVectra, actual.CarModel);
            Assert.AreEqual(2005, actual.Makeyear);
            Assert.AreEqual(VINEngineType.Z20NET, actual.EngineType);
            Assert.AreEqual(VINTurboModel.GarrettGT2052, actual.TurboModel);
            Assert.AreNotEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 095720
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3EE55EX63507433()
        {
            
            const string VINNumber = "YS3EE55EX63507433";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95, actual.CarModel);
            Assert.AreEqual(2006, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B235E, actual.EngineType);
            Assert.AreEqual(VINTurboModel.GarrettGT1752, actual.TurboModel);
            Assert.AreEqual("5 speed manual / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-5)", actual.PlantInfo);
            Assert.AreEqual("Model series III, Driver airbag", actual.Series);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 507433
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3EE55EX63507423()
        {
            
            const string VINNumber = "YS3EE55EX63507423";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95, actual.CarModel);
            Assert.AreEqual(2006, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B235E, actual.EngineType);
            Assert.AreEqual(VINTurboModel.GarrettGT1752, actual.TurboModel);
            Assert.AreEqual("5 speed manual / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-5)", actual.PlantInfo);
            Assert.AreEqual("Model series III, Driver airbag", actual.Series);
            Assert.AreNotEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 507423
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3EE55E263507433()
        {
            
            const string VINNumber = "YS3EE55E263507433";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95, actual.CarModel);
            Assert.AreEqual(2006, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B235E, actual.EngineType);
            Assert.AreEqual(VINTurboModel.GarrettGT1752, actual.TurboModel);
            Assert.AreEqual("5 speed manual / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-5)", actual.PlantInfo);
            Assert.AreEqual("Model series III, Driver airbag", actual.Series);
            Assert.AreNotEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 507433
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3EE55GX63507433()
        {
            
            const string VINNumber = "YS3EE55GX63507433";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95, actual.CarModel);
            Assert.AreEqual(2006, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B235R, actual.EngineType);
            Assert.AreEqual(VINTurboModel.MitsubishiTD04HL_15T_5, actual.TurboModel);
            Assert.AreEqual("5 speed manual / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-5)", actual.PlantInfo);
            Assert.AreEqual("Model series III, Driver airbag", actual.Series);
            Assert.AreNotEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 507433
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3EE55G863507433()
        {
            
            const string VINNumber = "YS3EE55G863507433";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95, actual.CarModel);
            Assert.AreEqual(2006, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B235R, actual.EngineType);
            Assert.AreEqual(VINTurboModel.MitsubishiTD04HL_15T_5, actual.TurboModel);
            Assert.AreEqual("5 speed manual / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-5)", actual.PlantInfo);
            Assert.AreEqual("Model series III, Driver airbag", actual.Series);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 507433
        }

        /// <summary>
        ///A test for DecodeVINNumber
        ///</summary>
        [TestMethod()]
        public void DecodeVINNumberTestYS3EE55G863507633()
        {
            
            const string VINNumber = "YS3EE55G863507633";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(VINCarModel.Saab95, actual.CarModel);
            Assert.AreEqual(2006, actual.Makeyear);
            Assert.AreEqual("5 door combi coupe", actual.Body);
            Assert.AreEqual(VINEngineType.B235R, actual.EngineType);
            Assert.AreEqual(VINTurboModel.MitsubishiTD04HL_15T_5, actual.TurboModel);
            Assert.AreEqual("5 speed manual / front wheel drive", actual.GearboxDescription);
            Assert.AreEqual("Trollhättan line A (9-5)", actual.PlantInfo);
            Assert.AreEqual("Model series III, Driver airbag", actual.Series);
            Assert.AreNotEqual(actual.CalculatedChecksum, VINNumber[8]);
            // Serialnumber not decoded 507633
        }

        /// <summary>
        ///A test for calculate VIN checksum algorithm
        ///</summary>
        [TestMethod()]
        public void CalculateVINchecksum()
        {
            
            string VINNumber = "11111111111111111";
            VINCarInfo actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            VINNumber = "1M8GDM9AXKP042788";
            actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(actual.CalculatedChecksum, VINNumber[8]);
            VINNumber = "1234567890123456"; // Too short
            actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(actual.CalculatedChecksum, '*');
            VINNumber = "1Q8GIM9AXKO042788"; // Invalid characters: I, O, Q
            actual = VINDecoder.DecodeVINNumber(VINNumber);
            Assert.AreEqual(actual.CalculatedChecksum, '*');
        }
    }
}
