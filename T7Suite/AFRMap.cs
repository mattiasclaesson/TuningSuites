using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using NLog;

namespace T7
{
    public class AFRMap
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public delegate void FuelmapCellChanged(object sender, FuelmapChangedEventArgs e);
        public event AFRMap.FuelmapCellChanged onFuelmapCellChanged;
        public delegate void CellLocked(object sender, EventArgs e);
        public event AFRMap.CellLocked onCellLocked;

        public class FuelmapChangedEventArgs : System.EventArgs
        {
            private int _mapindex;

            public int Mapindex
            {
                get { return _mapindex; }
                set { _mapindex = value; }
            }

            private byte _cellvalue;

            public byte Cellvalue
            {
                get { return _cellvalue; }
                set { _cellvalue = value; }
            }

            public FuelmapChangedEventArgs(int mapindex, byte cellvalue)
            {
                this._mapindex = mapindex;
                this._cellvalue = cellvalue;
            }
        }

        private float[] m_FeedbackMap;
        private int[] m_FeedbackCounterMap;
        private string m_filename = string.Empty;
        string foldername = string.Empty;

        public void InitializeMaps(int size, string filename)
        {
            m_filename = filename;
            logger.Debug("Init AFR map: " + size.ToString() + " " + filename);
            foldername = Path.Combine(Path.GetDirectoryName(filename), "AFRMaps");
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            m_FeedbackMap = new float[size];
            m_FeedbackMap.Initialize();
            m_FeedbackCounterMap = new int[size];
            m_FeedbackCounterMap.Initialize();
            if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(filename) + "-targetafr.afr")))
            {
                targetmap = LoadTargetAFRMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(filename) + "-targetafr.afr"));
            }
            else
            {
                //targetmap = null;
                CreateTargetMap(filename);
            }

            LoadMap(filename, size);

        }

        public byte[] GetTargetAFRMapinBytes(int length, string filename)
        {
            return LoadTargetAFRMapInBytes(targetmap, filename);
        }

        private byte[] LoadTargetAFRMapInBytes(float[] orginalmap, string filename)
        {

            byte[] map = new byte[/*m_TrionicFile.GetFileInfo().GetSymbolLength(m_TrionicFile.GetFileInfo().GetInjectionMap())*/ orginalmap.Length * 2];
            map.Initialize();
            if (File.Exists(filename))
            {
                try
                {
                    int idx = 0;
                    foreach (float f in orginalmap)
                    {
                        int val = (int)(Math.Ceiling(f * 10));
                        byte b1 = (byte)(val / 256);
                        byte b2 = (byte)(val - (int)b1 * 256);
                        map.SetValue(b1, idx++);
                        map.SetValue(b2, idx++);
                    }
                }
                catch (Exception E)
                {
                    //   MessageBox.Show("Failed to load target AFR map: " + E.Message);
                    logger.Debug(E.Message);
                }
            }
            else
            {
                map = new byte[orginalmap.Length * 2];
                map.Initialize();
            }
            return map;
        }


        public void CreateTargetMap(string filename)
        {
            targetmap = CreateDefaultTargetAFRMap();
            foldername = Path.Combine(Path.GetDirectoryName(filename), "AFRMaps");
            SaveTargetAFRMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(filename) + "-targetafr.afr"), targetmap, 18, 16);
        }

        private float[] CreateDefaultTargetAFRMap()
        {
            int columns = 18;
            int rows = 16;
            if (columns != 0)
            {
                float[] map = new float[rows * columns];

                for (int rpmtel = 0; rpmtel < rpmYSP.Length; rpmtel++)
                {

                    for (int maptel = 0; maptel < airXSP.Length; maptel++)
                    {
                        int mapvalue = (int)airXSP.GetValue(maptel);
                        int rpmvalue = (int)rpmYSP.GetValue(rpmtel);
                        logger.Debug("Calculating afr target for " + rpmvalue.ToString() + " " + mapvalue.ToString());

                        float afrtarget = 14.7F;
                        // now, decrease as mapvalue increases
                        // top MAP should be ~12.0
                        if (mapvalue > 600)
                        {
                            // positive boost
                            afrtarget -= 3.5F * (float)maptel / (float)columns;
                            if (rpmvalue > 4000) rpmvalue = 4000 - rpmvalue % 4000;
                            if (rpmvalue < 0) rpmvalue = 0;
                            afrtarget += Math.Abs((4000 - (float)rpmvalue) / 4000);
                            logger.Debug("Calculated afr target for " + rpmvalue.ToString() + " " + mapvalue.ToString() + " = " + afrtarget.ToString());
                        }
                        map.SetValue(afrtarget, rpmtel * columns + maptel);
                    }
                }
                return map;
            }
            float[] map2 = new float[1];
            map2.SetValue(0F, 0);
            return map2;
        }


        private float[] LoadTargetAFRMap(string filename)
        {
            // load the target AFR map into memory
            float[] map = new float[18 * 16];
            map.Initialize();
            if (File.Exists(filename))
            {
                string foldername = Path.Combine(Path.GetDirectoryName(filename), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                filename = Path.Combine(foldername, filename);

                if (File.Exists(filename))
                {
                    try
                    {
                        string line = string.Empty;
                        char[] sep = new char[1];
                        sep.SetValue(';', 0);
                        int idx = 0;
                        using (StreamReader sr = new StreamReader(filename))
                        {
                            while ((line = sr.ReadLine()) != null)
                            {
                                string[] values = line.Split(sep);
                                foreach (string value in values)
                                {
                                    if (value.Length > 0)
                                    {
                                        float val = (float)(ConvertToDouble(value));
                                        map.SetValue(val, idx++);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        //      MessageBox.Show("Failed to load target AFR map: " + E.Message);
                        // something went wrong, try to reinitialize the map
                        logger.Debug(E.Message);

                    }
                }
            }
            else
            {
                map = new float[18 * 16];
                map.Initialize();
            }
            return map;
        }


        private Stopwatch _cellDurationMonitor = new Stopwatch();
        private int _monitoringCellRPMIndex = -1;
        private int _monitoringCellMAPIndex = -1;
        private AFRMeasurementCollection _afrMeasurements = new AFRMeasurementCollection();
        FuelMapInformation m_FuelMapInformation;

        public FuelMapInformation FuelMapInformation
        {
            get { return m_FuelMapInformation; }
            set { m_FuelMapInformation = value; }
        }

        public byte[] GetOriginalFuelmap()
        {
            return originalfuelmap;
        }

        public void InitAutoTuneVars(bool ClosedLoopOn, int width, int height)
        {
            _monitoringCellRPMIndex = -1;
            _monitoringCellMAPIndex = -1;
            _afrMeasurements.Clear();
            _cellDurationMonitor.Stop();
            _cellDurationMonitor.Reset();
            // reset the feedback AFR map!
            if (!ClosedLoopOn)
            {
                // means we're entering autotune, in that case clear the feedback map
                InitAFRFeedbackMaps(width, height);
                m_FuelMapInformation = new FuelMapInformation();
                // set default values
                // m_FuelMapInformation.SetOriginalFuelMap(fuelmap);
            }

        }

        public double[] GetPercentualDifferences()
        {
            double[] retval = m_FuelMapInformation.GetDifferencesInPercentages();
            for (int t = 0; t < retval.Length; t++)
            {
                if (retval[t] > _MaximumAdjustmentPerCyclePercentage) retval[t] = _MaximumAdjustmentPerCyclePercentage;
            }
            return retval;
        }

        private float[] AFRMapInMemory;
        private int[] AFRMapCounterInMemory;


        private void InitAFRFeedbackMaps(int columns, int rows)
        {
            if (columns != 0)
            {
                AFRMapInMemory = new float[rows * columns];
                AFRMapCounterInMemory = new int[rows * columns];
                SaveAFRAndCounterMaps(columns, rows);
            }
        }

        public void SaveAFRAndCounterMaps(int columns, int rows)
        {
            try
            {
                if (AFRMapInMemory != null)
                {
                    // save it to the correct filename
                    SaveTargetAFRMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_filename) + "-AFRFeedbackmap.afr"), AFRMapInMemory, columns, rows);
                }
                if (AFRMapCounterInMemory != null)
                {
                    // save it
                    SaveTargetAFRCounterMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_filename) + "-AFRFeedbackCountermap.afr"), AFRMapCounterInMemory, columns, rows);
                }

            }
            catch (Exception stargetE)
            {
                logger.Debug(stargetE.Message);
            }
        }

        

        public int[] GetAFRCountermap()
        {
            return m_FeedbackCounterMap;
        }

        public void ClearMaps(int columns, int rows, string filename)
        {
            int size = columns * rows;
            logger.Debug("Clear AFR map: " + size.ToString() + " " + filename);
            m_FeedbackMap = new float[size];
            m_FeedbackMap.Initialize();
            m_FeedbackCounterMap = new int[size];
            m_FeedbackCounterMap.Initialize();
            SaveMap(filename, columns, rows);

        }

        public void SaveMap(string filename, int columns, int rows)
        {
            logger.Debug("Save AFR map: " + filename);
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }

            if (m_FeedbackMap != null && m_FeedbackCounterMap != null)
            {
                // save it to the correct filename
                SaveTargetAFRMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(filename) + "-AFRFeedbackmap.afr"), m_FeedbackMap, columns, rows);
                SaveTargetAFRCounterMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(filename) + "-AFRFeedbackCountermap.afr"), m_FeedbackCounterMap, columns, rows);
            }
        }

        public void LoadMap(string filename, int size)
        {
            // check if the map exists
            logger.Debug("Load AFR map: " + size.ToString() + " " + filename);
            string foldername = Path.Combine(Path.GetDirectoryName(filename), "AFRMaps");
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            string filenameafr = Path.Combine(foldername, Path.GetFileNameWithoutExtension(filename) + "-AFRFeedbackmap.afr");
            string filenamecount = Path.Combine(foldername, Path.GetFileNameWithoutExtension(filename) + "-AFRFeedbackCountermap.afr");
            if (File.Exists(filenameafr) && File.Exists(filenamecount))
            {
                // load both files
                if (File.Exists(filenameafr))
                {
                    m_FeedbackMap = LoadTargetAFRMap(filenameafr, size);
                    m_FeedbackCounterMap = LoadCounterMap(filenamecount, size);
                }
            }
        }

        private double ConvertToDouble(string v)
        {
            double d = 0;
            if (v == "") return d;
            string vs = "";
            vs = v.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Double.TryParse(vs, out d);
            return d;
        }

        private float[] LoadTargetAFRMap(string filename, int size)
        {
            // load the target AFR map into memory
            float[] map = new float[size];
            map.Initialize();
            if (File.Exists(filename))
            {
                try
                {
                    string line = string.Empty;
                    char[] sep = new char[1];
                    sep.SetValue(';', 0);
                    int idx = 0;
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] values = line.Split(sep);
                            foreach (string value in values)
                            {
                                if (value.Length > 0)
                                {
                                    float val = (float)(ConvertToDouble(value));
                                    map.SetValue(val, idx++);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //      MessageBox.Show("Failed to load target AFR map: " + E.Message);
                    // something went wrong, try to reinitialize the map

                }
            }
            return map;
        }

        private int[] LoadCounterMap(string filename, int size)
        {
            // load the target AFR map into memory
            int[] map = new int[size];
            map.Initialize();
            if (File.Exists(filename))
            {
                try
                {
                    string line = string.Empty;
                    char[] sep = new char[1];
                    sep.SetValue(';', 0);
                    int idx = 0;
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] values = line.Split(sep);
                            foreach (string value in values)
                            {
                                if (value.Length > 0)
                                {
                                    int val = Convert.ToInt32(value);
                                    map.SetValue(val, idx++);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //  MessageBox.Show("Failed to load target AFR counter map: " + E.Message);
                }
            }
            return map;
        }

        public void SaveTargetAFRMap(string filename, float[] map, int columns, int rows)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                for (int rt = 0; rt < rows; rt++)
                {
                    for (int ct = 0; ct < columns; ct++)
                    {
                        float value = (float)map.GetValue((rt * columns) + ct);
                        sw.Write(value.ToString("F2") + ";");
                    }
                    sw.Write(Environment.NewLine);
                }
            }
        }

        private void SaveTargetAFRCounterMap(string filename, int[] map, int columns, int rows)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                for (int rt = 0; rt < rows; rt++)
                {
                    for (int ct = 0; ct < columns; ct++)
                    {
                        int value = (int)map.GetValue((rt * columns) + ct);
                        sw.Write(value.ToString() + ";");
                    }
                    sw.Write(Environment.NewLine);
                }
            }
        }

        internal void AddMeasurement(float AFRValue, int RpmIndex, int AirmassIndex, int columns, int rows)
        {
            // we need to update the correct AFR feedback cell now
            int current_count = Convert.ToInt32(m_FeedbackCounterMap.GetValue((RpmIndex * columns) + AirmassIndex));
            float newvalue = (float)m_FeedbackMap.GetValue((RpmIndex * columns) + AirmassIndex) * current_count;
            newvalue += (float)AFRValue;
            current_count++;
            newvalue /= current_count;
            m_FeedbackMap.SetValue(newvalue, (RpmIndex * columns) + AirmassIndex);
            m_FeedbackCounterMap.SetValue(current_count, (RpmIndex * columns) + AirmassIndex);
        }

        internal byte[] GetFeedbackMapInBytes(int size)
        {
            return LoadTargetAFRMapInBytes(m_FeedbackMap, size);
        }

        internal byte[] GetFeedbackCounterMapInBytes(int size)
        {
            return LoadTargetAFRMapInBytes(m_FeedbackCounterMap, size);
        }

        private byte[] LoadTargetAFRMapInBytes(int[] orginalmap, int size)
        {
            byte[] map = new byte[size * 2];
            map.Initialize();
            try
            {
                int idx = 0;
                foreach (int val in orginalmap)
                {
                    byte b1 = (byte)(val / 256);
                    byte b2 = (byte)(val - (int)b1 * 256);
                    map.SetValue(b1, idx++);
                    map.SetValue(b2, idx++);
                }
            }
            catch (Exception)
            {
                //   MessageBox.Show("Failed to load target AFR map: " + E.Message);
            }

            return map;
        }

        private byte[] LoadTargetAFRMapInBytes(float[] orginalmap, int size)
        {
            byte[] map = new byte[size * 2];
            map.Initialize();
            try
            {
                int idx = 0;
                foreach (float f in orginalmap)
                {
                    int val = (int)(Math.Ceiling(f * 10));
                    byte b1 = (byte)(val / 256);
                    byte b2 = (byte)(val - (int)b1 * 256);
                    map.SetValue(b1, idx++);
                    map.SetValue(b2, idx++);
                }
            }
            catch (Exception)
            {
                //   MessageBox.Show("Failed to load target AFR map: " + E.Message);
            }

            return map;
        }

        private byte[] fuelmap;
        private byte[] originalfuelmap;
        private int[] fuelcorrectioncountermap;
        private float[] targetmap;

        //private bool _HasValidFuelmap = false;

        public void SetCurrentFuelMap(byte[] fuelmapdata)
        {
            // copy the data
            fuelmap = new byte[fuelmapdata.Length];
            for (int t = 0; t < fuelmapdata.Length; t++)
            {
                fuelmap[t] = fuelmapdata[t];
            }
            // and create a correction counter map with all 0's
            fuelcorrectioncountermap = new int[fuelmapdata.Length];
            for (int i = 0; i < fuelcorrectioncountermap.Length; i++)
            {
                fuelcorrectioncountermap[i] = 0; // initialize
            }
            m_FuelMapInformation.SetOriginalFuelMap(fuelmapdata);
            //_HasValidFuelmap = true;
        }

        public void SetOriginalFuelMap(byte[] fuelmapdata)
        {
            originalfuelmap = new byte[fuelmapdata.Length];
            for (int t = 0; t < fuelmapdata.Length; t++)
            {
                originalfuelmap[t] = fuelmapdata[t];
            }
            m_FuelMapInformation.SetOriginalFuelMap(fuelmapdata);
        }


        private bool _AutoUpdateFuelMap = true;

        public bool AutoUpdateFuelMap
        {
            get { return _AutoUpdateFuelMap; }
            set { _AutoUpdateFuelMap = value; }
        }

        private int _CellStableTime_ms;

        public int CellStableTime_ms
        {
            get { return _CellStableTime_ms; }
            set { _CellStableTime_ms = value; }
        }
        private int _AcceptableTargetErrorPercentage;
        public int AcceptableTargetErrorPercentage
        {
            get { return _AcceptableTargetErrorPercentage; }
            set { _AcceptableTargetErrorPercentage = value; }
        }

        private int _CorrectionPercentage;

        public int CorrectionPercentage
        {
            get { return _CorrectionPercentage; }
            set { _CorrectionPercentage = value; }
        }

        private int _MaximumAdjustmentPerCyclePercentage;

        public int MaximumAdjustmentPerCyclePercentage
        {
            get { return _MaximumAdjustmentPerCyclePercentage; }
            set { _MaximumAdjustmentPerCyclePercentage = value; }
        }

        public double GetAverageFromMeasurements()
        {
            double average = 0;
            for (int i = 0; i < _afrMeasurements.Count; i++)
            {
                average += _afrMeasurements[i].AfrValue;
            }
            average /= _afrMeasurements.Count;
            return average;
        }

        int[] rpmYSP = new int[16];

        public int[] RpmYSP
        {
            get { return rpmYSP; }
            set { rpmYSP = value; }
        }
        int[] airXSP = new int[18];

        public int[] AirXSP
        {
            get { return airXSP; }
            set { airXSP = value; }
        }

        private int LookUpIndexAxisRPMMap(double value)
        {
            //TODO: uses fixed axis from BFuelCal.Map (BFuelCal.RpmYSP)
            int return_index = -1;
            double min_difference = 10000000;


            for (int t = 0; t < rpmYSP.Length; t++)
            {
                int b = (int)rpmYSP.GetValue(t);
                double diff = Math.Abs((double)b - value);
                if (min_difference > diff)
                {
                    min_difference = diff;
                    // this is our index
                    return_index = t;
                }
            }

            return return_index;
        }

        private int LookUpIndexAxisMap(double value)
        {
            //TODO: uses fixed axis from BFuelCal.Map (BFuelCal.AirXSP)
            int return_index = -1;
            double min_difference = 10000000;
            //logger.Debug("looking up: " + value.ToString());
            for (int t = 0; t < airXSP.Length; t++)
            {
                int b = (int)airXSP.GetValue(t);
                //logger.Debug("checking against: " + b.ToString());
                double diff = Math.Abs((double)b - value);
                if (min_difference > diff)
                {
                    //logger.Debug("diff: " + diff.ToString());
                    min_difference = diff;
                    // this is our index
                    return_index = t;
                    //logger.Debug("idx: " + return_index.ToString());
                }
            }
            //logger.Debug("Index found = " + return_index.ToString());
            return return_index;
        }


        private void CastCellLockedEvent()
        {
            if (onCellLocked != null)
            {
                onCellLocked(this, EventArgs.Empty);
            }
        }

        private void CastFuelmapCellChangedEvent(int mapindex, byte cellvalue)
        {
            if (onFuelmapCellChanged != null)
            {
                onFuelmapCellChanged(this, new FuelmapChangedEventArgs(mapindex, cellvalue));
            }
        }

        internal void HandleRealtimeData(float afr, float rpm, float airmass)
        {
            double currentLoad = airmass;
            //currentLoad += 1;
            //currentLoad /= 0.01;
            int rpmindex = LookUpIndexAxisRPMMap(rpm);
            int mapindex = LookUpIndexAxisMap(currentLoad);
            //logger.Debug("rpm index: " + rpmindex.ToString() + " loadindex: " + mapindex.ToString());
            //so we now know in what cell we are
            // if the cell changed, we need to restart the stopwatch
            // if the cell is unchanged we need to check the stopwatchduration against the settings
            if (rpmindex != _monitoringCellRPMIndex || mapindex != _monitoringCellMAPIndex)
            {
                // set both values
                _afrMeasurements.Clear(); // clear average collection
                _monitoringCellRPMIndex = rpmindex;
                _monitoringCellMAPIndex = mapindex;
                // reset stopwatch
                _cellDurationMonitor.Stop();
                _cellDurationMonitor.Reset();
                _cellDurationMonitor.Start();
            }
            else
            {
                // appearantly we are in the same cell
                if (_monitoringCellMAPIndex >= 0 && _monitoringCellRPMIndex >= 0)
                {
                    AFRMeasurement _measurement = new AFRMeasurement();
                    _measurement.AfrValue = afr;
                    _afrMeasurements.Add(_measurement);
                    long elapsed_ms = _cellDurationMonitor.ElapsedMilliseconds;

                    if (elapsed_ms > _CellStableTime_ms)
                    {
                        
                        // check afr against target afr
                        // get the average AFR from the collection
                        float average_afr_in_cell = (float)GetAverageFromMeasurements();
                        float targetafr_currentcell = (float)targetmap.GetValue((rpmindex * 18) + mapindex);
                        // calculate the difference in percentage
                        float _afr_diff_percentage = Math.Abs(((targetafr_currentcell - average_afr_in_cell) / targetafr_currentcell) * 100);
                        if (_afr_diff_percentage > _AcceptableTargetErrorPercentage)
                        {
                            
                            // afr is outside of limits (difference > set percentage).. now what?
                            // is the afr lower or higher than the target?
                            // we should correct with the set percentage _CorrectionPercentage
                            // but no more than _MaximumAdjustmentPerCyclePercentage of the original fuelmap value
                            float afr_diff_to_correct = Math.Abs(_afr_diff_percentage);
                            afr_diff_to_correct *= _CorrectionPercentage;
                            afr_diff_to_correct /= 100;
                            // cap it to _MaximumAdjustmentPerCyclePercentage
                            if (afr_diff_to_correct > _MaximumAdjustmentPerCyclePercentage)
                            {
                                afr_diff_to_correct = _MaximumAdjustmentPerCyclePercentage;
                            }
                            logger.Debug("Stable in cell: " + rpmindex.ToString() + " " + mapindex.ToString() + " " + _afr_diff_percentage.ToString() + " " + afr_diff_to_correct.ToString());
                            // get the current fuel map value for the given cell
                            // get the number of changes that have been made already to this cell
                            // if the number of changes > x then don't do changes anymore and notify the user
                            int _fuelcorrectionvalue = (int)fuelmap[(rpmindex * 18) + mapindex];

                            if (average_afr_in_cell > targetafr_currentcell)
                            {
                                // we're running too lean, so we need to increase the fuelmap value by afr_diff_to_correct %
                                // get the current fuelmap value
                                // correct it with the percentage
                                logger.Debug("running lean");
                                _fuelcorrectionvalue *= (int)(100 + afr_diff_to_correct);
                                _fuelcorrectionvalue /= 100;
                                if (_fuelcorrectionvalue > 254) _fuelcorrectionvalue = 254;
                                // save it to the map

                            }
                            else
                            {
                                // we're running too rich, so we need to decrease the fuelmap value by afr_diff_to_correct %
                                // correct it with the percentage
                                logger.Debug("running rich");
                                _fuelcorrectionvalue *= (int)(100 - afr_diff_to_correct);
                                _fuelcorrectionvalue /= 100;
                                // don't go under 25, seems to be some kind of boundary!
                                // <GS-28102010> changed for testing purposes, if problems occur, revert back to 25 as boundary
                                if (_fuelcorrectionvalue < 1) _fuelcorrectionvalue = 1;

                                // save it to the map
                            }
                            if (fuelmap[(rpmindex * 18) + mapindex] != (byte)_fuelcorrectionvalue)
                            {
                                // if autowrite to ECU
                                if (_AutoUpdateFuelMap)
                                {
                                    // if the user should be notified, do so now, ask permission to alter the fuel map
                                    logger.Debug("Altering rpmidx: " + rpmindex.ToString() + " mapidx: " + mapindex.ToString() + " from value: " + fuelmap[(rpmindex * 18) + mapindex].ToString() + " to value: " + _fuelcorrectionvalue.ToString());
                                    fuelmap[(rpmindex * 18) + mapindex] = (byte)_fuelcorrectionvalue;
                                    // increase counter
                                    fuelcorrectioncountermap[(rpmindex * 18) + mapindex]++;
                                    // cast an event that will write the data into the ECU (?)
                                    CastFuelmapCellChangedEvent((rpmindex * 18) + mapindex, (byte)_fuelcorrectionvalue);
                                }
                                else
                                {
                                    // keep track of the changes in the fuelmapinfo
                                    m_FuelMapInformation.UpdateFuelMap(mapindex, rpmindex, _fuelcorrectionvalue);
                                }
                                //<GS-09082010> play an optional PING sound here!
                                CastCellLockedEvent();
                            }
                            //now we reset the stopwatch and wait for another event to occur
                            _afrMeasurements.Clear(); // clear average collection
                            _cellDurationMonitor.Stop();
                            _cellDurationMonitor.Reset();
                            _cellDurationMonitor.Start();

                        }
                        else
                        {
                            //what to do if AFR is correct (within limits), should we reset the stopwatch?
                            _afrMeasurements.Clear(); // clear average collection
                            _cellDurationMonitor.Stop();
                            _cellDurationMonitor.Reset();
                            _cellDurationMonitor.Start();
                        }
                    }
                }
                else
                {
                    // nothing, invalid index from map
                    _afrMeasurements.Clear(); // clear average collection
                    _cellDurationMonitor.Stop();
                    _cellDurationMonitor.Reset();
                    _cellDurationMonitor.Start();
                }
            }
        }

        internal float[] GetFeedbackMap()
        {
            return m_FeedbackMap;
        }

        internal float[] GetTargetMap()
        {
            return targetmap;
        }

    }
}
