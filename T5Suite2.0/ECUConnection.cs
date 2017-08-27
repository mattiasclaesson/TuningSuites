using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Data;
using System.IO;
using System.Globalization;
using Trionic5Tools;
using CommonSuite;

namespace T5Suite2
{
    public class ECUConnection
    {
        public delegate void SymbolDataReceived(object sender, RealtimeDataEventArgs e);
        public event ECUConnection.SymbolDataReceived onSymbolDataReceived;

        public delegate void CycleCompleted(object sender, EventArgs e);
        public event ECUConnection.CycleCompleted onCycleCompleted;


        public delegate void WriteDataToECU(object sender, ProgressEventArgs e);
        public event ECUConnection.WriteDataToECU onWriteDataToECU;

        public delegate void ReadDataFromECU(object sender, ProgressEventArgs e);
        public event ECUConnection.ReadDataFromECU onReadDataFromECU;

        public delegate void CanBusInfo(object sender, CanInfoEventArgs e);
        public event ECUConnection.CanBusInfo onCanBusInfo;

        private Random ran = new Random(DateTime.Now.Millisecond);

        private bool m_RunInEmulationMode = false;

        public bool RunInEmulationMode
        {
            get { return m_RunInEmulationMode; }
            set { m_RunInEmulationMode = value; }
        }

        private bool _stallReading = false;

        public bool StallReading
        {
            get { return _stallReading; }
            set { _stallReading = value; }
        }

        private bool _isT52 = false;

        public bool IsT52
        {
            get { return _isT52; }
            set { _isT52 = value; }
        }

        private Engine m_Engine = new Engine();

        private Trionic5Tools.MapSensorType _mapSensorType = MapSensorType.MapSensor25;

        public Trionic5Tools.MapSensorType MapSensorType
        {
            get { return _mapSensorType; }
            set { _mapSensorType = value; }
        }

        private T5CANLib.T5CAN _tcan = null;
        private T5CANLib.CAN.ICANDevice _usbcandevice = null;
        private string _swversion = string.Empty;
        private System.Timers.Timer m_MonitorECUTimer = new System.Timers.Timer();
        private bool _prohibitRead = false;
        private Trionic5SymbolConverter _symConverter = new Trionic5SymbolConverter();

        private bool _engineRunning = false;

        public bool EngineRunning
        {
            get { return _engineRunning; }
            set { _engineRunning = value; }
        }

        private string _sramDumpFile = string.Empty;

        public string SramDumpFile
        {
            get { return _sramDumpFile; }
            set { _sramDumpFile = value; }
        }

        public bool ProhibitRead
        {
            get { return _prohibitRead; }
            set { _prohibitRead = value; }
        }


        private SymbolCollection m_SymbolsToMonitor;

        public SymbolCollection SymbolsToMonitor
        {
            get { return m_SymbolsToMonitor; }
            set { m_SymbolsToMonitor = value; }
        }

        public void StartECUMonitoring()
        {
            //_prohibitRead = false;
            _stallReading = false;
        }

        public void StopECUMonitoring()
        {
            //_prohibitRead = true;
            _stallReading = true;
        }


        public void AddSymbolToWatchlist(string symbolname, Int32 sramaddress, Int32 length, bool systemSymbol)
        {
            _stallReading = true;
            if (symbolname != "")
            {
                SymbolHelper sh = new SymbolHelper();
                sh.Varname = symbolname;
                sh.Start_address = sramaddress;
                sh.Length = length;
                sh.IsSystemSymbol = systemSymbol;
                if (!CollectionContains(symbolname)) // maybe use systemSymbol as well <GS-11042011>
                {
                    m_SymbolsToMonitor.Add(sh);
                }
            }
            _stallReading = false;
        }

        public void AddSymbolToWatchlist(SymbolHelper shuser, bool systemSymbol)
        {
            _stallReading = true;
            if (shuser.Varname != "")
            {
                SymbolHelper sh = new SymbolHelper();
                sh.Varname = shuser.Varname;
                sh.Start_address = shuser.Start_address;
                sh.Length = shuser.Length;
                sh.UserCorrectionFactor = shuser.UserCorrectionFactor;
                sh.UserCorrectionOffset = shuser.UserCorrectionOffset;
                sh.UseUserCorrection = shuser.UseUserCorrection;
                sh.IsSystemSymbol = systemSymbol;
                if (!CollectionContains(shuser.Varname)) // maybe use systemSymbol as well <GS-11042011>
                {
                    m_SymbolsToMonitor.Add(sh);
                }
            }
            _stallReading = false;
        }

        private bool CollectionContains(string symbolname)
        {
            foreach (SymbolHelper sh in m_SymbolsToMonitor)
            {
                if (sh.Varname == symbolname) return true;
            }
            return false;
        }

        private bool CollectionContains(string symbolname, bool systemSymbol)
        {
            foreach (SymbolHelper sh in m_SymbolsToMonitor)
            {
                if (sh.Varname == symbolname && sh.IsSystemSymbol == systemSymbol) return true;
            }
            return false;
        }

        public void RemoveSymbolFromWatchlist(string symbolname)
        {
            _stallReading = true;
            foreach (SymbolHelper sh in m_SymbolsToMonitor)
            {
                if (sh.Varname == symbolname && !sh.IsSystemSymbol)
                {
                    m_SymbolsToMonitor.Remove(sh);
                    return;
                }
            }
            _stallReading = false;
        }

        public string Swversion
        {
            get { return _swversion; }
            set { _swversion = value; }
        }
        private bool _opened = false;

        public bool Opened
        {
            get { return _opened; }
            set { _opened = value; }
        }

        public void DumpSRAM(string filename)
        {
            if(_tcan != null)
            {
                _prohibitRead = true;
                Thread.Sleep(100);
                _tcan.DumpSRAMContent(filename);
                _prohibitRead = false;
            }
        }

        public void SetWidebandvalues(double lowvoltage, double highvoltage, double lowafr, double highafr)
        {
            _symConverter.WidebandHighAFR = highafr;
            _symConverter.WidebandLowAFR = lowafr;
            _symConverter.WidebandHighVoltage = highvoltage;
            _symConverter.WidebandLowVoltage = lowvoltage;
        }

        public ECUConnection()
        {
            _sramDumpFile = string.Empty;
            m_Engine.onEngineRunning += new Engine.NotifyEngineState(m_Engine_onEngineRunning);
            m_MonitorECUTimer.AutoReset = true;
            m_MonitorECUTimer.Interval = 10;
            m_MonitorECUTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_MonitorECUTimer_Elapsed);
            m_MonitorECUTimer.Start();
            m_SymbolsToMonitor = new SymbolCollection();
            /*if (_tcan == null)
            {
                _tcan = new T5CANLib.T5CAN();
                _usbcandevice = new T5CANLib.CAN.CANUSBDevice();
                _tcan.onWriteProgress += new T5CANLib.T5CAN.WriteProgress(OnWriteProgress);
                _tcan.onCanInfo +=new T5CANLib.T5CAN.CanInfo(_tcan_onCanInfo);
                _tcan.onReadProgress += new T5CANLib.T5CAN.ReadProgress(_tcan_onReadProgress);
                _tcan.setCANDevice(_usbcandevice);
                _opened = false;
                Thread.Sleep(500);
            }*/
        }

        void m_Engine_onEngineRunning(object sender, Engine.EngineStateEventArgs e)
        {
            if (m_RunInEmulationMode)
            {
                // convert value and handle result
                //e.EngineLoad
                //onSymbolDataReceived(this, new RealtimeDataEventArgs("P_medel", (double)e.EngineLoad));
                onSymbolDataReceived(this, new RealtimeDataEventArgs("AD_EGR", (double)e.AirFuelRatio));
                onSymbolDataReceived(this, new RealtimeDataEventArgs("Kyl_temp", (double)e.CoolantTemperature));
                onSymbolDataReceived(this, new RealtimeDataEventArgs("Insptid_ms10", (double)e.InjectionTime));
                onSymbolDataReceived(this, new RealtimeDataEventArgs("Lufttemp", (double)e.IntakeAitTemperature));
                onSymbolDataReceived(this, new RealtimeDataEventArgs("Rpm", (double)e.RPM));
                onSymbolDataReceived(this, new RealtimeDataEventArgs("Bil_hast", (double)e.Speed));
                onSymbolDataReceived(this, new RealtimeDataEventArgs("Medeltrot", (double)e.ThrottlePosition));
                onSymbolDataReceived(this, new RealtimeDataEventArgs("P_medel", (double)e.TurboPressure));
                onSymbolDataReceived(this, new RealtimeDataEventArgs("Pgm_status", (double)0));
                onCycleCompleted(this, EventArgs.Empty);
                /*if (onSymbolDataReceived != null)
                {
                    foreach (SymbolHelper sh in m_SymbolsToMonitor)
                    {
                        if (sh.Varname == "Rpm")
                        {
                            int symbolvalue = 900 + ran.Next(6000);
                            onSymbolDataReceived(this, new RealtimeDataEventArgs(sh.Varname, symbolvalue));
                        }
                        else if (sh.Varname == "P_medel")
                        {
                            double symbolvalue = (ran.NextDouble() - 0.5) * 2;
                            onSymbolDataReceived(this, new RealtimeDataEventArgs(sh.Varname, symbolvalue));
                        }
                        else if (sh.Varname == "AD_EGR")
                        {
                            int symbolvalue = 10 + ran.Next(10);
                            onSymbolDataReceived(this, new RealtimeDataEventArgs(sh.Varname, symbolvalue));
                        }
                        else if (sh.Length == 1)
                        {
                            int symbolvalue = ran.Next(255);
                            onSymbolDataReceived(this, new RealtimeDataEventArgs(sh.Varname, symbolvalue));
                        }
                        else if (sh.Length == 2)
                        {
                            int symbolvalue = ran.Next(65535);
                            onSymbolDataReceived(this, new RealtimeDataEventArgs(sh.Varname, symbolvalue));
                        }
                    }

                    
                    //m_Engine.Throttleposition = 40;
                    onCycleCompleted(this, EventArgs.Empty);
                }*/
            }
        }

        public DateTime GetMemorySyncDate()
        {
            DateTime dt_sync = new DateTime(2000, 1, 1, 0, 0, 0); // testvalue
            int year = 2000;
            int month = 1;
            int day = 1;
            int hour = 0;
            int minute = 0;
            int second = 0;
            if (m_RunInEmulationMode) return DateTime.Now;

            //if (_sramDumpFile == string.Empty)
            //{
                byte[] result = _tcan.readRAM((ushort)0x7FC0, 1);
                year = (Int32)result[0] * 256;
                result = _tcan.readRAM((ushort)0x7FC1, 1);
                year += (Int32)result[0];
                result = _tcan.readRAM((ushort)0x7FC2, 1);
                month = (Int32)result[0];
                result = _tcan.readRAM((ushort)0x7FC3, 1);
                day = (Int32)result[0];
                result = _tcan.readRAM((ushort)0x7FC4, 1);
                hour = (Int32)result[0];
                result = _tcan.readRAM((ushort)0x7FC5, 1);
                minute = (Int32)result[0];
                result = _tcan.readRAM((ushort)0x7FC6, 1);
                second = (Int32)result[0];
                if (year != -1)
                {
                    try
                    {
                        dt_sync = new DateTime(year, month, day, hour, minute, second);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                        foreach (byte b in result)
                        {
                            Console.Write(b.ToString("X2") + " ");
                        }
                        Console.WriteLine();

                        result = _tcan.readRAM((ushort)0x7FC0, 1);
                        year = (Int32)result[0] * 256;
                        result = _tcan.readRAM((ushort)0x7FC1, 1);
                        year += (Int32)result[0];
                        result = _tcan.readRAM((ushort)0x7FC2, 1);
                        month = (Int32)result[0];
                        result = _tcan.readRAM((ushort)0x7FC3, 1);
                        day = (Int32)result[0];
                        result = _tcan.readRAM((ushort)0x7FC4, 1);
                        hour = (Int32)result[0];
                        result = _tcan.readRAM((ushort)0x7FC5, 1);
                        minute = (Int32)result[0];
                        result = _tcan.readRAM((ushort)0x7FC6, 1);
                        second = (Int32)result[0];
                        foreach (byte b in result)
                        {
                            Console.Write(b.ToString("X2") + " ");
                        }
                        Console.WriteLine();
                        try
                        {
                            dt_sync = new DateTime(year, month, day, hour, minute, second);
                        }
                        catch (Exception E2)
                        {
                            Console.WriteLine(E2.Message);
                        }
                    }
                }

           /* }
            else
            {
                byte[] result = ReadData((uint)0x7FC0, 8);
                year = (Int32)result[0] * 256;
                year += (Int32)result[1];
                month = (Int32)result[2];
                day = (Int32)result[3];
                hour = (Int32)result[4];
                minute = (Int32)result[5];
                second = (Int32)result[6];
                if (year != -1)
                {
                    try
                    {
                        dt_sync = new DateTime(year, month, day, hour, minute, second);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }
            }*/
            Console.WriteLine("ECU: " + dt_sync.ToString("dd/MM/yyyy HH:mm:ss"));
            return dt_sync;
        }

        public void SetMemorySyncDate(DateTime syncdt)
        {
            byte[] buf = new byte[7];
            _prohibitRead = true;
            buf[0] = Convert.ToByte((syncdt.Year >> 8) & 0x0000000000000000FF);
            buf[1] = Convert.ToByte((syncdt.Year) & 0x0000000000000000FF);
            buf[2] = Convert.ToByte((syncdt.Month) & 0x0000000000000000FF);
            buf[3] = Convert.ToByte((syncdt.Day) & 0x0000000000000000FF);
            buf[4] = Convert.ToByte((syncdt.Hour) & 0x0000000000000000FF);
            buf[5] = Convert.ToByte((syncdt.Minute) & 0x0000000000000000FF);
            buf[6] = Convert.ToByte((syncdt.Second) & 0x0000000000000000FF);
           // if (_sramDumpFile == string.Empty)
            //{
                _tcan.writeRam(0x7FC0, buf);
            /*}
            else
            {
                WriteData(buf, 0x7FC0);
            }*/
            _prohibitRead = false;
            //WriteDataNoCounterIncrease(buf, (uint)(m_fileInfo.Filelength - 0x1E0));
        }

        public Int64 GetMemorySyncCounter()
        {
            // read a specific portion of the sram memory for this
            // 0x7FD0 maybe (doing for test)
            Int64 countervalue = 0;
            //if (_sramDumpFile == string.Empty)
            //{
                byte[] result = _tcan.readRAM((ushort)0x7FD0, 8);
                countervalue = (Int64)result[0] * 256 * 256 * 256 * 256 * 256 * 256 * 256;
                countervalue += (Int64)result[1] * 256 * 256 * 256 * 256 * 256 * 256;
                countervalue += (Int64)result[2] * 256 * 256 * 256 * 256 * 256;
                countervalue += (Int64)result[3] * 256 * 256 * 256 * 256;
                countervalue += (Int64)result[4] * 256 * 256 * 256;
                countervalue += (Int64)result[5] * 256 * 256;
                countervalue += (Int64)result[6] * 256;
                countervalue += (Int64)result[7];
                
            /*}
            else
            {
                byte[] result = ReadData((uint)0x7FD0, 8);
                countervalue = (Int64)result[0] * 256 * 256 * 256 * 256 * 256 * 256 * 256;
                countervalue += (Int64)result[1] * 256 * 256 * 256 * 256 * 256 * 256;
                countervalue += (Int64)result[2] * 256 * 256 * 256 * 256 * 256;
                countervalue += (Int64)result[3] * 256 * 256 * 256 * 256;
                countervalue += (Int64)result[4] * 256 * 256 * 256;
                countervalue += (Int64)result[5] * 256 * 256;
                countervalue += (Int64)result[6] * 256;
                countervalue += (Int64)result[7];
            }*/
            return countervalue;
        }

        public void SetMemorySyncCounter(Int64 countervalue)
        {
            byte[] buf = new byte[8];
            _prohibitRead = true;
            buf[0] = Convert.ToByte((countervalue >> 56) & 0x0000000000000000FF);
            buf[1] = Convert.ToByte((countervalue >> 48) & 0x0000000000000000FF);
            buf[2] = Convert.ToByte((countervalue >> 40) & 0x0000000000000000FF);
            buf[3] = Convert.ToByte((countervalue >> 32) & 0x0000000000000000FF);
            buf[4] = Convert.ToByte((countervalue >> 24) & 0x0000000000000000FF);
            buf[5] = Convert.ToByte((countervalue >> 16) & 0x0000000000000000FF);
            buf[6] = Convert.ToByte((countervalue >> 8) & 0x0000000000000000FF);
            buf[7] = Convert.ToByte((countervalue) & 0x0000000000000000FF);
            //if (_sramDumpFile == string.Empty)
            //{
                _tcan.writeRam(0x7FD0, buf);
            /*}
            else
            {
                WriteData(buf, 0x7FD0);
            }*/
            _prohibitRead = false;
        }

        void _tcan_onReadProgress(object sender, T5CANLib.ReadProgressEventArgs e)
        {
            // cast read progress event
            if (onReadDataFromECU != null)
            {
                onReadDataFromECU(this, new ProgressEventArgs(e.Percentage, 0, 0));
            }
        }

        private T5AppSettings m_appSettings;

        public T5AppSettings AppSettings
        {
            get { return m_appSettings; }
            set { m_appSettings = value; }
        }

        private double ConvertADCValue(int channel, float value)
        {
            double retval = value;
            double m_HighVoltage = 5;
            double m_LowVoltage = 0;
            double m_HighValue = 1;
            double m_LowValue = 0;
            switch (channel)
            {
                case 0:
                    m_HighVoltage = m_appSettings.Adc1highvoltage;
                    m_LowVoltage = m_appSettings.Adc1lowvoltage;
                    m_LowValue = m_appSettings.Adc1lowvalue;
                    m_HighValue = m_appSettings.Adc1highvalue;
                    //Console.WriteLine("highV = " + m_HighVoltage.ToString() + " lowV = " + m_LowVoltage.ToString() + " highVal = " + m_HighValue.ToString() + " lowVal = " + m_LowValue.ToString());
                    break;
                case 1:
                    m_HighVoltage = m_appSettings.Adc2highvoltage;
                    m_LowVoltage = m_appSettings.Adc2lowvoltage;
                    m_LowValue = m_appSettings.Adc2lowvalue;
                    m_HighValue = m_appSettings.Adc2highvalue;
                    break;
                case 2:
                    m_HighVoltage = m_appSettings.Adc3highvoltage;
                    m_LowVoltage = m_appSettings.Adc3lowvoltage;
                    m_LowValue = m_appSettings.Adc3lowvalue;
                    m_HighValue = m_appSettings.Adc3highvalue;
                    break;
                case 3:
                    m_HighVoltage = m_appSettings.Adc4highvoltage;
                    m_LowVoltage = m_appSettings.Adc4lowvoltage;
                    m_LowValue = m_appSettings.Adc4lowvalue;
                    m_HighValue = m_appSettings.Adc4highvalue;
                    break;
                case 4:
                    m_HighVoltage = m_appSettings.Adc5highvoltage;
                    m_LowVoltage = m_appSettings.Adc5lowvoltage;
                    m_LowValue = m_appSettings.Adc5lowvalue;
                    m_HighValue = m_appSettings.Adc5highvalue;
                    break;
                default:
                    break;
            }
            // convert using the known math
            // convert to AFR value using wideband lambda sensor settings
            // ranges 0 - 255 will be default for 0-5 volt
            double voltage = value;//<GS-12042011> Combiadapter seems to generate voltage in stead of 0-255 values ((value) / 255) * (m_HighVoltage / 1000 - m_LowVoltage / 1000);
            //Console.WriteLine("Voltage: " + voltage.ToString());
            // now convert to AFR using user settings
            if (voltage < m_LowVoltage / 1000) voltage = m_LowVoltage / 1000;
            if (voltage > m_HighVoltage / 1000) voltage = m_HighVoltage / 1000;
            //Console.WriteLine("Voltage (after clipping): " + voltage.ToString());
            double steepness = ((m_HighValue / 1000) - (m_LowValue / 1000)) / ((m_HighVoltage / 1000) - (m_LowVoltage / 1000));
            //Console.WriteLine("Steepness: " + steepness.ToString());
            retval = (m_LowValue / 1000) + (steepness * (voltage - (m_LowVoltage / 1000)));
            //Console.WriteLine("retval: " + retval.ToString());
            return retval;

        }

        void m_MonitorECUTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_stallReading) return;

            m_MonitorECUTimer.Stop();

            if (_opened && !_prohibitRead && !m_RunInEmulationMode)
            {
                if (_tcan != null)
                {
                    try
                    {
                        foreach (SymbolHelper sh in m_SymbolsToMonitor)
                        {
                            if (_prohibitRead || _stallReading) break;
                            //if (_sramDumpFile == string.Empty)
                            //{
                                if (sh.Varname.StartsWith("Knock_count_cyl")) sh.Length = 2;
                                byte[] result = _tcan.readRAM((ushort)sh.Start_address, (uint)sh.Length);
                                // convert resultvalue to a usable format (doubles)
                                HandleResult(result, sh.Varname, sh.UserCorrectionFactor, sh.UserCorrectionOffset, sh.UseUserCorrection);
                            /*}
                            else
                            {
                                if (sh.Varname.StartsWith("Knock_count_cyl")) sh.Length = 2;
                                byte[] result = ReadData((uint)sh.Start_address, (uint)sh.Length);
                                HandleResult(result, sh.Varname);
                            }*/
                        }
                        if (_canusbDevice == "Multiadapter" || _canusbDevice == "CombiAdapter")
                        {
                            if (m_appSettings.Useadc1)
                            {
                                //_tcan.geta
                                float adc = _usbcandevice.GetADCValue(0);
                                //Console.WriteLine("ADC1: " + adc.ToString());
                                double convertedADvalue = ConvertADCValue(0, adc);
                                //Console.WriteLine("ADC1 converted: " + convertedADvalue.ToString());
                                string channelName = m_appSettings.Adc1channelname;
                                if (onSymbolDataReceived != null)
                                {
                                    onSymbolDataReceived(this, new RealtimeDataEventArgs(channelName, convertedADvalue));
                                }
                            }
                            if (m_appSettings.Useadc2)
                            {
                                float adc = _usbcandevice.GetADCValue(1);
                                double convertedADvalue = ConvertADCValue(1, adc);
                                string channelName = m_appSettings.Adc2channelname;
                                if (onSymbolDataReceived != null)
                                {
                                    onSymbolDataReceived(this, new RealtimeDataEventArgs(channelName, convertedADvalue));
                                }
                                
                            }
                            if (m_appSettings.Useadc3)
                            {
                                float adc = _usbcandevice.GetADCValue(2);
                                double convertedADvalue = ConvertADCValue(2, adc);
                                string channelName = m_appSettings.Adc3channelname;
                                if (onSymbolDataReceived != null)
                                {
                                    onSymbolDataReceived(this, new RealtimeDataEventArgs(channelName, convertedADvalue));
                                }
                            }
                            if (m_appSettings.Useadc4)
                            {
                                float adc = _usbcandevice.GetADCValue(3);
                                double convertedADvalue = ConvertADCValue(3, adc);
                                string channelName = m_appSettings.Adc4channelname;
                                if (onSymbolDataReceived != null)
                                {
                                    onSymbolDataReceived(this, new RealtimeDataEventArgs(channelName, convertedADvalue));
                                }
                            }
                            if (m_appSettings.Useadc5)
                            {
                                float adc = _usbcandevice.GetADCValue(4);
                                double convertedADvalue = ConvertADCValue(4, adc);
                                string channelName = m_appSettings.Adc5channelname;
                                if (onSymbolDataReceived != null)
                                {
                                    onSymbolDataReceived(this, new RealtimeDataEventArgs(channelName, convertedADvalue));
                                }
                            }
                            if (m_appSettings.Usethermo)
                            {
                                float temperature = _usbcandevice.GetThermoValue();
                                string channelName = m_appSettings.Thermochannelname;
                                Console.WriteLine(channelName + " = " + temperature.ToString());
                                if (onSymbolDataReceived != null)
                                {
                                    onSymbolDataReceived(this, new RealtimeDataEventArgs(channelName, temperature));
                                }
                            }
                        }
                        // cast cycle completed event
                        if (onCycleCompleted != null)
                        {
                            onCycleCompleted(this, EventArgs.Empty);
                        }

                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("m_MonitorECUTimer_Elapsed: "+ E.Message);
                    }
                }
            }
            m_MonitorECUTimer.Start();
        }

        private byte[] ReadData(uint offset, uint length)
        {
            if (File.Exists(_sramDumpFile))
            {
                return readdatafromfile(_sramDumpFile, (int)offset, (int)length);
            }
            byte[] b = new byte[1];
            b.SetValue((byte)0x00, 0);
            return b;
        }

        public bool WriteData(byte[] data, uint offset)
        {
            if (File.Exists(_sramDumpFile))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    writebyteinfile(_sramDumpFile, (int)offset + i, (byte)data.GetValue(i));
                }
                return true;
            }
            return false;
        }

        public void writebyteinfile(string filename, int address, byte value)
        {
            if (address <= 0) return;

            FileStream fsi1 = File.OpenWrite(filename);
            while (address > fsi1.Length) address -= (int)fsi1.Length;
            BinaryWriter br1 = new BinaryWriter(fsi1);
            fsi1.Position = address;
            br1.Write(value);
            fsi1.Flush();
            br1.Close();
            fsi1.Close();
            fsi1.Dispose();
        }


        private byte[] readdatafromfile(string filename, int address, int length)
        {
            byte[] retval = new byte[length];
            FileStream fsi1 = File.OpenRead(filename);
            while (address > fsi1.Length) address -= (int)fsi1.Length;
            BinaryReader br1 = new BinaryReader(fsi1);
            fsi1.Position = address;
            string temp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                retval.SetValue(br1.ReadByte(), i);
            }
            fsi1.Flush();
            br1.Close();
            fsi1.Close();
            fsi1.Dispose();
            return retval;
        }

        private void HandleResult(byte[] ECUData, string symbolname, double _userCorrectionFactor, double _userCorrectionOffset, bool UseUserCorrection)
        {
            // let a subclass convert the data to useable stuff
            
            double symbolvalue = _symConverter.ConvertSymbol(symbolname, ECUData, _mapSensorType, _userCorrectionFactor, _userCorrectionOffset, UseUserCorrection);
            //Console.WriteLine("Handling: " + symbolname + " " + symbolvalue.ToString());
            if (symbolname == "Rpm")
            {
                if (symbolvalue > 0) _engineRunning = true;
                else _engineRunning = false;
            }
            // cast a delegate to toplevel
            if (onSymbolDataReceived != null)
            {

                //<GS-09082010>
                /*if (m_appSettings.DebugMode)
                {
                    if (symbolname == "Rpm") symbolvalue = 2000;
                    else if (symbolname == "Kyl_temp") symbolvalue = 80;
                    else if (symbolname == "AD_EGR") symbolvalue = 17;
                    else if (symbolname == "P_Medel") symbolvalue = 1.2F;
                    else if (symbolname == "P_medel") symbolvalue = 1.2F;
                    else if (symbolname == "P_Manifold10") symbolvalue = 1.2F;
                }*/

                onSymbolDataReceived(this, new RealtimeDataEventArgs(symbolname, symbolvalue));
                //Console.WriteLine(symbolname + " " + symbolvalue.ToString());
            }
        }

        private string _canusbDevice = "Lawicel";

        public string CanusbDevice
        {
            get { return _canusbDevice; }
            set { _canusbDevice = value; }
        }

        public void OpenECUConnection()
        {
            if (m_RunInEmulationMode)
            {
                m_Engine.InitializeEngine();
                m_Engine.StartEngine();
                _opened = true;
            }
            if (!_opened)
            {
                _opened = true;
                //if (_sramDumpFile == string.Empty)
                //{
                    if (_tcan == null)
                    {
                        _tcan = new T5CANLib.T5CAN();
                        if (_canusbDevice == "Multiadapter" || _canusbDevice == "CombiAdapter")
                        {
                            _usbcandevice = new T5CANLib.CAN.LPCCANDevice_T5();
                        }
                        else if (_canusbDevice == "DIY")
                        {
                            _usbcandevice = new T5CANLib.CAN.MctCanDevice();
                        }
                        else if (_canusbDevice == "Just4Trionic")
                        {
                            _usbcandevice = new T5CANLib.CAN.Just4TrionicDevice();
                        }
                        else // default = Lawicel
                        {
                            _usbcandevice = new T5CANLib.CAN.CANUSBDevice();
                        }

                        _tcan.onWriteProgress += new T5CANLib.T5CAN.WriteProgress(OnWriteProgress);
                        _tcan.onCanInfo += new T5CANLib.T5CAN.CanInfo(_tcan_onCanInfo);
                        _tcan.onReadProgress +=new T5CANLib.T5CAN.ReadProgress(_tcan_onReadProgress);
                        _tcan.setCANDevice(_usbcandevice);
                    }
                    if (!_tcan.openDevice(out _swversion))
                    {
                        _opened = false;
                    }
                    /*if (_opened) // if can adapter opened successfully, try to contact the ECU
                    {
                        byte[] testbyte = _tcan.readRAM(0x1000, 2);
                        if (testbyte.Length != 2) _opened = false;
                        Console.WriteLine(testbyte.Length.ToString() + " as result");
                    }*/
                    Console.WriteLine("OpenECUConnection: " + _swversion);
                //}
            }
            _stallReading = false;
        }

        void _tcan_onCanInfo(object sender, T5CANLib.CanInfoEventArgs e)
        {
            if (onCanBusInfo != null)
            {
                onCanBusInfo(this, new CanInfoEventArgs(e.Info));
            }
        }

        public string GetSoftwareVersion()
        {
            string swversion = string.Empty;
            if (m_RunInEmulationMode) return "DEBUG.MODE";
            if (_opened)
            {
                //if (_sramDumpFile == string.Empty)
                //{
                if (_isT52)
                {
                    swversion = _tcan.getSWVersionT52(false);
                }
                else
                {
                    swversion = _tcan.getSWVersion(false);
                }
                /*}
                else
                {
                    swversion = "DUMMY.SRAM";
                }*/
            }
            return swversion;
        }

        public void CloseECUConnection(bool forceClose)
        {
            if (m_RunInEmulationMode)
            {
                m_Engine.StopEngine();
            }

            if (_opened && forceClose)
            {
                _opened = false;
                //if (_sramDumpFile == string.Empty)
                {
                  if(_tcan != null)  _tcan.Cleanup();
                }
            } // <GS-22032010> never close the connection
            else if (_opened)
            {
                // should we slow down on the connection here? 
                //explore options <GS-22032010>
                _stallReading = true;
            }
        }

        public bool IsConnectedECUT52()
        {
            bool retval = false;
            if (_opened)
            {
                retval = _tcan.isT52ECU();
            }
            return retval;
        }

        public DataTable GetSymbolTable()
        {

            DataTable _symtable = new DataTable();
            _symtable.Columns.Add("Symbol");
            _symtable.Columns.Add("Address", Type.GetType("System.Int32"));
            _symtable.Columns.Add("Length", Type.GetType("System.Int32"));
            //if (_sramDumpFile != string.Empty) return _symtable;
            if (_opened)
            {
                _prohibitRead = true;
                Thread.Sleep(100);
                if (_swversion == string.Empty)
                {
                    if (m_RunInEmulationMode) _swversion = "DEBUG.MODE";
                    else _swversion = _tcan.getSWVersion(false);
                }
                string symbols = string.Empty;
                if (!m_RunInEmulationMode)
                {
                    if (!File.Exists(System.Windows.Forms.Application.StartupPath + "\\" + _swversion + ".symbollist"))
                    {
                        symbols = string.Join(Environment.NewLine, _tcan.getSymbolTable().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                        try
                        {
                            System.IO.File.WriteAllText(System.Windows.Forms.Application.StartupPath + "\\" + _swversion + ".symbollist", symbols);
                        }
                        catch
                        {
                            System.IO.File.WriteAllText(System.Windows.Forms.Application.StartupPath + "\\" + "bad_name" + ".symbollist", symbols);
                        }
                    }
                    else
                    {
                        symbols = File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\" + _swversion + ".symbollist");
                    }


                    foreach (string symbol in symbols.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!symbol.StartsWith(">") && symbol != "END")
                        {
                            try
                            {
                                ushort address = ushort.Parse(symbol.Substring(0, 4), NumberStyles.AllowHexSpecifier);
                                uint length = uint.Parse(symbol.Substring(4, 4), NumberStyles.AllowHexSpecifier);
                                string symname = symbol.Substring(8);
                                _symtable.Rows.Add(symname, Convert.ToInt32(address), Convert.ToInt32(length));
                            }
                            catch { }
                        }
                    }
                }
                _prohibitRead = false;
            }
            return _symtable;
        }

        void OnWriteProgress(object sender, T5CANLib.WriteProgressEventArgs e)
        {
            if (onWriteDataToECU != null)
            {
                onWriteDataToECU(this, new ProgressEventArgs(e.Percentage, 0, 0));
            }

        }

        public void ProgramFlash(string flashfile, T5CANLib.ECUType type)
        {
            _prohibitRead = true;
            Thread.Sleep(100);
            _tcan.UpgradeECU(flashfile, T5CANLib.FileType.BinaryFile, type);
            _prohibitRead = false;
        }

        


        public void EnableLogging(string path2log)
        {
            if (_usbcandevice != null)
            {
                _usbcandevice.EnableLogging(path2log);
            }
        }

        internal void DisableLogging()
        {
            if (_usbcandevice != null)
            {
                _usbcandevice.DisableLogging();
            }
        }

        public class RealtimeDataEventArgs : System.EventArgs
        {
            private string _symbol;

            public string Symbol
            {
                get { return _symbol; }
                set { _symbol = value; }
            }


            private double _value;

            public double Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public RealtimeDataEventArgs(string symbol, double value)
            {
                this._symbol = symbol;
                this._value = value;
            }
        }


        public class CanInfoEventArgs : System.EventArgs
        {
            private string _info = string.Empty;

            public string Info
            {
                get { return _info; }
                set { _info = value; }
            }

            public CanInfoEventArgs(string info)
            {
                this._info = info;
            }
        }

        public class ProgressEventArgs : System.EventArgs
        {
            private int _percentage;

            public int Percentage
            {
                get { return _percentage; }
                set { _percentage = value; }
            }
            private int _byteswritten;

            public int Byteswritten
            {
                get { return _byteswritten; }
                set { _byteswritten = value; }
            }
            private int _bytestowrite;

            public int Bytestowrite
            {
                get { return _bytestowrite; }
                set { _bytestowrite = value; }
            }

            public ProgressEventArgs(int percentage, int byteswritten, int bytestowrite)
            {
                this._bytestowrite = bytestowrite;
                this._byteswritten = byteswritten;
                this._percentage = percentage;
            }
        }


        internal void RemoveAllSymbolsFromWatchlist()
        {
            _stallReading = true;
            m_SymbolsToMonitor.Clear();
            bool found = true;
            while (found)
            {
                found = false;
                foreach (SymbolHelper sh in m_SymbolsToMonitor)
                {
                    if (!sh.IsSystemSymbol)
                    {
                        m_SymbolsToMonitor.Remove(sh);
                        found = true;
                        break;
                    }
                }
            }
            _stallReading = false;
        }

        internal byte[] ReadSymbolData(string symbolname, uint address, uint length)
        {
            byte[] retval = new byte[1];
            retval.SetValue((byte)0, 0);
            if (_opened && _tcan != null) //<GS-17032011>
            {
                _prohibitRead = true;
                //if (_sramDumpFile == string.Empty)
                //{
                    retval = _tcan.readRAM((ushort)address, length); //<GS-17032011> Should be the only line here
                //}
                //else
                //{
                   //retval = ReadData(address, length);
                //}
                _prohibitRead = false;
            }
            return retval;
        }

        internal byte[] ReadSymbolDataNoProhibitRead(string symbolname, uint address, uint length)
        {
            byte[] retval = new byte[1];
            retval.SetValue((byte)0, 0);
            if (_opened && _tcan != null) //<GS-17032011>
            {
                //if (_sramDumpFile == string.Empty)
                //{
                    retval = _tcan.readRAM((ushort)address, length);//<GS-17032011> Should be the only line here
                //}
                //else
                //{
                    //retval = ReadData(address, length);
                //}
            }
            return retval;
        }

        private bool MoreThanHalfDiffers(byte[] buf1, byte[] buf2)
        {
            int diffcount = 0;
            if (buf1.Length != buf2.Length) return true;
            else
            {
                for (int _bytetel = 0; _bytetel < buf1.Length; _bytetel++)
                {
                    if (buf1[_bytetel] != buf2[_bytetel])
                    {
                        diffcount++;
                    }
                }
            }
            if ((diffcount * 2) > buf1.Length) return true;
            return false;
        }

        private void ConvertRealtimeValuesForUseInMapviewers()
        {
          //  float requested_boost = GetBoostRequest(engine_status.Throttle_position, engine_status.Rpm, out tpsindex, out rpmindex);
         //   UpdateBoostRequestmap(requested_boost, tpsindex, rpmindex);
        }


        internal void WriteSymbolDataForced(int address, int length, byte[] data)
        {
            if (_opened && _tcan != null)   //<GS-17032011>
            {
                // hold reading until writing is done!
                _prohibitRead = true;
                //if (_sramDumpFile == string.Empty)
                //{
                    _tcan.writeRamForced((ushort)address, data);    //<GS-17032011> Should be the only line here
                    Thread.Sleep(20);                               //<GS-17032011> Should be the only line here
                //}
                //else
                //{
                    //WriteData(data, (uint)address);
                //}
                _prohibitRead = false;
            }
        }

        internal void WriteSymbolData(int address, int length, byte[] data)
        {
            if (_opened && _tcan != null)   //<GS-17032011>
            {
                // hold reading until writing is done!
                _prohibitRead = true;
                //if (_sramDumpFile == string.Empty)
                //{
                    _tcan.writeRam((ushort)address, data);  //<GS-17032011> Should be the only line here
                    Thread.Sleep(20);                       //<GS-17032011> Should be the only line here
                //}
                //else
                //{
//                  WriteData(data, (uint)address);
                //}
                //Thread.Sleep(20);
                //SetMemorySyncCounter(GetMemorySyncCounter() + 1);
                SetMemorySyncDate(DateTime.Now);
                Thread.Sleep(20);
                _prohibitRead = false;
            }
        }

        internal void ReadFlash(string filename)
        {
            _prohibitRead = true;
            Thread.Sleep(100);
            _tcan.DumpECU(filename);
            _prohibitRead = false;
        }
    }
}
