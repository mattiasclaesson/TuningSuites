using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

// allows autotuning Ignition map (Ign_map_0!)
// we need a locking map (which cells are locked)
// we need a change counter map (which cells are changed and by how much, maybe combine with locking map)

namespace Trionic5Tools
{
    public enum IgnitionMapType : int
    {
        FeedbackIgnitionMap
    }

    public class IgnitionMaps
    {
        public delegate void IgnitionmapCellChanged(object sender, IgnitionmapChangedEventArgs e);
        public event IgnitionMaps.IgnitionmapCellChanged onIgnitionmapCellChanged;

        public delegate void CellLocked(object sender, IgnitionmapChangedEventArgs e);
        public event IgnitionMaps.CellLocked onCellLocked;

        public class IgnitionmapChangedEventArgs : System.EventArgs
        {
            private int _mapindex;

            public int Mapindex
            {
                get { return _mapindex; }
                set { _mapindex = value; }
            }

            private int _cellvalue;

            public int Cellvalue
            {
                get { return _cellvalue; }
                set { _cellvalue = value; }
            }

            public IgnitionmapChangedEventArgs(int mapindex, int cellvalue)
            {
                this._mapindex = mapindex;
                this._cellvalue = cellvalue;
            }
        }


        private IECUFile m_TrionicFile;

        private int[] ignitionmap;
        private int[] originalignitionmap;
        private int[] ignitioncorrectioncountermap;
        private int[] ignitionlockedmap;
        private float[] IgnitionMapInMemory;
        private int[] IgnitionMapCounterInMemory;

        private int[] knockPressTab;

        private int _CellStableTime_ms;

        private double m_IgnitionAdvancePerCycle = 0.1;

        public double IgnitionAdvancePerCycle
        {
            get { return m_IgnitionAdvancePerCycle; }
            set { m_IgnitionAdvancePerCycle = value; }
        }
        private double m_IgnitionRetardFirstKnock = 1.0;

        public double IgnitionRetardFirstKnock
        {
            get { return m_IgnitionRetardFirstKnock; }
            set { m_IgnitionRetardFirstKnock = value; }
        }
        private double m_IgnitionRetardFurtherKnocks = 0.5;

        public double IgnitionRetardFurtherKnocks
        {
            get { return m_IgnitionRetardFurtherKnocks; }
            set { m_IgnitionRetardFurtherKnocks = value; }
        }
        private double m_GlobalMaximumIgnitionAdvance = 35.0;

        public double GlobalMaximumIgnitionAdvance
        {
            get { return m_GlobalMaximumIgnitionAdvance; }
            set { m_GlobalMaximumIgnitionAdvance = value; }
        }


        private double m_MaxumimIgnitionAdvancePerSession = 2;

        public double MaxumimIgnitionAdvancePerSession
        {
            get { return m_MaxumimIgnitionAdvancePerSession; }
            set { m_MaxumimIgnitionAdvancePerSession = value; }
        }

        private int _MinimumEngineSpeedForIgnitionTuning = 1200;

        public int MinimumEngineSpeedForIgnitionTuning
        {
            get { return _MinimumEngineSpeedForIgnitionTuning; }
            set { _MinimumEngineSpeedForIgnitionTuning = value; }
        }

        public int CellStableTime_ms
        {
            get { return _CellStableTime_ms; }
            set { _CellStableTime_ms = value; }
        }

        private bool _isAutoMappingActive = false;

        public bool IsAutoMappingActive
        {
            get { return _isAutoMappingActive; }
            set
            {
                if (_isAutoMappingActive != value)
                {
                    _isAutoMappingActive = value;
                    if (_isAutoMappingActive)
                    {
                        _currentEngineSpeed = 0;
                        _currentBoostLevel = 0;
                        _currentThrottlePosition = 0;
                        _currentAdvance = 0;
                        _currentKnockCondition = false;
                        _cellDurationMonitor.Stop();
                        _cellDurationMonitor.Reset();
                    }
                }
            }
        }

        private double mapsensorfactor = 1;

        public IECUFile TrionicFile
        {
            get { return m_TrionicFile; }
            set
            {
                m_TrionicFile = value;
                if (m_TrionicFile != null)
                {
                    if (m_TrionicFile.Exists())
                    {
                        mapsensorfactor = CalculateConversionFactor(MapSensorType.MapSensor25, m_TrionicFile.GetMapSensorType(false));
                    }
                }
            }
        }

        private double _currentEngineSpeed = 0;
        private double _currentBoostLevel = 0;
        private double _currentThrottlePosition = 0;
        private double _currentAdvance = 0;
        private bool _currentKnockCondition = false;
        private Stopwatch _cellDurationMonitor = new Stopwatch();
        private int _monitoringCellRPMIndex = -1;
        private int _monitoringCellMAPIndex = -1;
        private IgnitionMeasurementCollection _ignitionMeasurements = new IgnitionMeasurementCollection();

        public void InitAutoTuneVars(bool startAutoTune)
        {
            _monitoringCellRPMIndex = -1;
            _monitoringCellMAPIndex = -1;
            _ignitionMeasurements.Clear();
            _cellDurationMonitor.Stop();
            _cellDurationMonitor.Reset();
            // reset the feedback ignition map!
            if (startAutoTune)
            {
                // means we're entering autotune, in that case clear the feedback map
                InitIgnitionFeedbackMaps();
            }

        }

        private void InitIgnitionFeedbackMaps()
        {
            int columns = 18;
            int rows = 16;

            m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetIgnitionMap(), out columns, out rows);
            if (columns != 0)
            {
                IgnitionMapInMemory = new float[rows * columns];
                IgnitionMapCounterInMemory = new int[rows * columns];
                SaveIgnitionAndCounterMaps();
            }
        }

        public double GetAverageFromMeasurements()
        {
            double average = 0;
            for (int i = 0; i < _ignitionMeasurements.Count; i++)
            {
                average += _ignitionMeasurements[i].IgnitionValue;
            }
            average /= _ignitionMeasurements.Count;
            return average;
        }

        private void CastCellLockedEvent(int mapindex)
        {
            if (onCellLocked != null)
            {
                onCellLocked(this, new IgnitionmapChangedEventArgs(mapindex, 1));
            }
        }

        private void CastIgnitionmapCellChangedEvent(int mapindex, int cellvalue)
        {
            if (onIgnitionmapCellChanged != null)
            {
                onIgnitionmapCellChanged(this, new IgnitionmapChangedEventArgs(mapindex, cellvalue));
            }
        }


        /// <summary>
        /// clears data that might cause problems when switching from idle to normal ignition map or v.v.
        /// </summary>
        public void AutoTuneSwitchMap()
        {
            _monitoringCellRPMIndex = -1;
            _monitoringCellMAPIndex = -1;
            _ignitionMeasurements.Clear();
        }

        private bool _holdUntilKnockIsGone = false;
        
        public void HandleRealtimeData(double rpm, double tps, double boost, double ignitionadvance, bool isKnocking)
        {
            if (_isAutoMappingActive && !_holdUntilKnockIsGone)  // _holdUuntilKnockIsGone indicates we should ignore measurements until knock is over
            {
                if (rpm > _MinimumEngineSpeedForIgnitionTuning) 
                {
                    // only handle this stuff if automapping is active
                    // first off, get the index in the ignition map for the given engine speed and load
                    double currentLoad = boost;
                    currentLoad += 1;
                    currentLoad /= 0.01;
                    int rpmindex = LookUpIndexAxisRPMMap(rpm, "Ign_map_0_y_axis!", 1);
                    int mapindex = LookUpIndexAxisMap(currentLoad, "Ign_map_0_x_axis!");

                    // we need to be higher in boost than what knock_press_tab tells us, because below these pressure, knock will not
                    // be detected
                    int boostLimit = knockPressTab[rpmindex];
                    double dLimit = (double)boostLimit;
                    dLimit *= mapsensorfactor;

                    //EDIT: We made a slight mistake here... we need to start altering one column HIGHER in boost because
                    // if we change the column directly when boost > knockPressTab[..] we will still influence ignition advance
                    // because of T5 table interpolation. So, we need to skip ONE extra column
                    // how can we do this? ... get the index for boostLimit as well
                    int mapMinimumIndex = LookUpIndexAxisMap(boostLimit, "Ign_map_0_x_axis!");

                    if (dLimit > currentLoad) return;
                    if (mapindex <= mapMinimumIndex + 1) return; // we're not allowed to alter this column because it will cause interpolation issues

                    //so we now know in what cell we are
                    // if the cell changed, we need to restart the stopwatch
                    // if the cell is unchanged we need to check the stopwatchduration against the settings
                    if (rpmindex != _monitoringCellRPMIndex || mapindex != _monitoringCellMAPIndex)
                    {
                        // set both values
                        _ignitionMeasurements.Clear(); // clear average collection
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
                            IgnitionMeasurement _measurement = new IgnitionMeasurement();
                            _measurement.IgnitionValue = ignitionadvance;
                            _ignitionMeasurements.Add(_measurement);
                            long elapsed_ms = _cellDurationMonitor.ElapsedMilliseconds;

                            if (elapsed_ms > _CellStableTime_ms)
                            {
                                //Console.WriteLine("Stable in cell: " + rpmindex.ToString() + " " + mapindex.ToString());
                                float average_ignition_in_cell = (float)GetAverageFromMeasurements();
                                int ignitionAdvanceValueFromMap = ignitionmap[_monitoringCellRPMIndex * 18 + _monitoringCellMAPIndex];

                                // TODO: if knocking, retard ignition by 1 full degree and lock the cell.
                                // TODO: if NOT knocking, advance ignition by m_IgnitionAdvancePerCycle (0.1 by default) degrees but ONLY if the boost is higher than the knock detection limit AND knock detection is turned on
                                // If we make a change we need to reset the monitor time and the measurements
                                if (isKnocking)
                                {
                                    _holdUntilKnockIsGone = true;
                                    if (IsCellLocked(_monitoringCellMAPIndex, _monitoringCellRPMIndex))
                                    {
                                        ignitionAdvanceValueFromMap -= Convert.ToInt32(m_IgnitionRetardFurtherKnocks * 10); // might be knocking based on previous cell.. now what
                                        // only update stuff AFTER knock has disappeared again
                                        AddLogEntry("Decreasing by " + m_IgnitionRetardFurtherKnocks.ToString() + " degrees because locked cell still knocked rpm: " + rpm.ToString() + " boost: " + boost.ToString() + " adjusted to " + ignitionAdvanceValueFromMap.ToString());
                                        if (ignitionAdvanceValueFromMap <= 0) ignitionAdvanceValueFromMap = 0;
                                        ignitionmap[_monitoringCellRPMIndex * 18 + _monitoringCellMAPIndex] = ignitionAdvanceValueFromMap;
                                        ignitioncorrectioncountermap[_monitoringCellRPMIndex * 18 + _monitoringCellMAPIndex]++;
                                        CastIgnitionmapCellChangedEvent((rpmindex * 18) + mapindex, ignitionAdvanceValueFromMap);
                                    }
                                    else
                                    {
                                        ignitionAdvanceValueFromMap -= Convert.ToInt32(m_IgnitionRetardFirstKnock * 10);
                                        if (ignitionAdvanceValueFromMap <= 0) ignitionAdvanceValueFromMap = 0;
                                        AddLogEntry("Decreasing by " + m_IgnitionRetardFirstKnock.ToString() + " degrees because non-locked cell knocked rpm: " + rpm.ToString() + " boost: " + boost.ToString() + " adjusted to " + ignitionAdvanceValueFromMap.ToString());
                                        ignitionmap[_monitoringCellRPMIndex * 18 + _monitoringCellMAPIndex] = ignitionAdvanceValueFromMap;
                                        ignitioncorrectioncountermap[_monitoringCellRPMIndex * 18 + _monitoringCellMAPIndex]++;
                                        CastIgnitionmapCellChangedEvent((rpmindex * 18) + mapindex, ignitionAdvanceValueFromMap);
                                        LockCell(_monitoringCellMAPIndex, _monitoringCellRPMIndex);
                                    }
                                    _ignitionMeasurements.Clear(); // clear average collection
                                    _cellDurationMonitor.Stop();
                                    _cellDurationMonitor.Reset();
                                    _cellDurationMonitor.Start();

                                }
                                else
                                {
                                    _holdUntilKnockIsGone = false;
                                    if (!IsCellLocked(_monitoringCellMAPIndex, _monitoringCellRPMIndex))
                                    {
                                        //increase ignition by m_IgnitionAdvancePerCycle (0.1 by default) degrees IF the cell is not locked, otherwise leave it be
                                        //Cast event to increase ignition

                                        // check against m_MaxumimIgnitionAdvancePerSession
                                        int originalAdvance = originalignitionmap[_monitoringCellRPMIndex * 18 + _monitoringCellMAPIndex];
                                        double advanceThisSession = Convert.ToDouble(ignitionAdvanceValueFromMap - originalAdvance);
                                        advanceThisSession /= 10;
                                        if (advanceThisSession >= m_MaxumimIgnitionAdvancePerSession)
                                        {
                                            AddLogEntry("Maximum advance increase reached for rpm: " + rpm.ToString() + " boost: " + boost.ToString() + " adjusted to " + ignitionAdvanceValueFromMap.ToString());
                                            if (ignitionAdvanceValueFromMap > Convert.ToInt32(m_GlobalMaximumIgnitionAdvance * 10))
                                            {
                                                ignitionmap[_monitoringCellRPMIndex * 18 + _monitoringCellMAPIndex] = Convert.ToInt32(m_GlobalMaximumIgnitionAdvance * 10); // cap
                                                CastIgnitionmapCellChangedEvent((rpmindex * 18) + mapindex, Convert.ToInt32(m_GlobalMaximumIgnitionAdvance * 10));
                                            }
                                        }
                                        else
                                        {
                                            ignitionAdvanceValueFromMap += Convert.ToInt32(m_IgnitionAdvancePerCycle * 10);
                                            if (ignitionAdvanceValueFromMap >= Convert.ToInt32(m_GlobalMaximumIgnitionAdvance * 10)) ignitionAdvanceValueFromMap = Convert.ToInt32(m_GlobalMaximumIgnitionAdvance * 10); // max @ 40 degrees advance
                                            AddLogEntry("Increasing by " + m_IgnitionAdvancePerCycle.ToString() + " degrees because non-locked cell did not knock rpm: " + rpm.ToString() + " boost: " + boost.ToString() + " adjusted to " + ignitionAdvanceValueFromMap.ToString());
                                            ignitionmap[_monitoringCellRPMIndex * 18 + _monitoringCellMAPIndex] = ignitionAdvanceValueFromMap;
                                            ignitioncorrectioncountermap[_monitoringCellRPMIndex * 18 + _monitoringCellMAPIndex]++;
                                            CastIgnitionmapCellChangedEvent((rpmindex * 18) + mapindex, ignitionAdvanceValueFromMap);
                                        }
                                    }
                                    _ignitionMeasurements.Clear(); // clear average collection
                                    _cellDurationMonitor.Stop();
                                    _cellDurationMonitor.Reset();
                                    _cellDurationMonitor.Start();
                                }

                            }
                        }
                        else
                        {
                            // nothing, invalid index from map
                            _ignitionMeasurements.Clear(); // clear average collection
                            _cellDurationMonitor.Stop();
                            _cellDurationMonitor.Reset();
                            _cellDurationMonitor.Start();
                        }
                    }
                }
            }
            else if (!isKnocking)
            {
                _holdUntilKnockIsGone = false;
            }
        }

        public void LockCell(int _monitoringCellMAPIndex, int _monitoringCellRPMIndex)
        {
            ignitionlockedmap.SetValue(1, (_monitoringCellRPMIndex * 18) + _monitoringCellMAPIndex);
            SaveIgnitionAndCounterMaps();
        }

        public void UnlockCell(int _monitoringCellMAPIndex, int _monitoringCellRPMIndex)
        {
            ignitionlockedmap.SetValue(0, (_monitoringCellRPMIndex * 18) + _monitoringCellMAPIndex);
            SaveIgnitionAndCounterMaps();
        }

        /// <summary>
        /// Checks if the current cell is locked (previously detected knock
        /// </summary>
        /// <param name="_monitoringCellMAPIndex"></param>
        /// <param name="_monitoringCellRPMIndex"></param>
        /// <returns></returns>
        private bool IsCellLocked(int _monitoringCellMAPIndex, int _monitoringCellRPMIndex)
        {
            //TODO: Implement this, should check whether a CELL is locked
            // This LOCK table should only be cleared by the USER not automatically by software when starting autotune or something
            if ((int)ignitionlockedmap.GetValue((_monitoringCellRPMIndex * 18) + _monitoringCellMAPIndex) > 0)
            {
                return true;
            }
            return false;
        }


        public int[] GetCurrentlyMutatedIgnitionMap()
        {
            return ignitionmap;
        }

        public void ClearIgnitionLockedMap()
        {
            for (int i = 0; i < ignitionlockedmap.Length; i++) ignitionlockedmap.SetValue(0, i);
            SaveIgnitionAndCounterMaps();
        }


        public int[] GetIgnitionLockedMap()
        {
            return ignitionlockedmap;
        }


        public int[] GetCurrentlyMutatedIgnitionMapCounter()
        {
            return ignitioncorrectioncountermap;
        }


        public int[] GetOriginalIgnitionmap()
        {
            return originalignitionmap;
        }



        public void SetOriginalIgnitionMap(int[] ignitionmapdata)
        {

            originalignitionmap = new int[ignitionmapdata.Length];
            for (int t = 0; t < ignitionmapdata.Length; t++)
            {
                originalignitionmap[t] = ignitionmapdata[t];
            }
            
        }

        public void SetKnockPressTab(int[] knockpressuremap)
        {
            knockPressTab = new int[knockpressuremap.Length];
            for (int t = 0; t < knockpressuremap.Length; t++)
            {
                knockPressTab[t] = knockpressuremap[t];
            }
        }

        public void SetCurrentIgnitionMap(int[] ignitionmapdata)
        {
            
            // copy the data
            ignitionmap = new int[ignitionmapdata.Length];
            for (int t = 0; t < ignitionmapdata.Length; t++)
            {
                ignitionmap[t] = ignitionmapdata[t];
            }
            // and create a correction counter map with all 0's
            ignitioncorrectioncountermap = new int[ignitionmapdata.Length];
            for (int i = 0; i < ignitioncorrectioncountermap.Length; i++)
            {
                ignitioncorrectioncountermap[i] = 0; // initialize
            }
        }


        public void InitializeMaps()
        {
            // first get the data length we should use (default is 18x16 Ign_map_0)
            string injection_map_name = m_TrionicFile.GetFileInfo().GetIgnitionMap();
            int map_length = m_TrionicFile.GetFileInfo().GetSymbolLength(injection_map_name);
            if (map_length == 0) map_length = 18 * 16; // fail safe for bins without a correct symbol table, e.g. maptun/nordic files etc
            try
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "IgnitionMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }

                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-IgnitionFeedbackmap.ign")))
                {
                    IgnitionMapInMemory = LoadTargetIgnitionMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-IgnitionFeedbackmap.ign");
                }
                else
                {
                    int columns = 18;
                    int rows = 16;

                    //m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetIgnitionMap(), out columns, out rows);
                    if (columns != 0)
                    {
                        IgnitionMapInMemory = new float[rows * columns];
                        IgnitionMapCounterInMemory = new int[rows * columns];
                        ignitionlockedmap = new int[rows * columns];
                        SaveIgnitionAndCounterMaps();
                    }
                }
                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-IgnitionFeedbackCountermap.ign")))
                {
                    IgnitionMapCounterInMemory = LoadCounterMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-IgnitionFeedbackCountermap.ign"));
                }
                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-IgnitionLockedmap.ign")))
                {
                    ignitionlockedmap = LoadCounterMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-IgnitionLockedmap.ign"));
                }

            }
            catch (Exception tafrE)
            {
                Console.WriteLine(tafrE.Message);
            }

        }

        private int LookUpIndexAxisRPMMap(double value, string symbolname, int multiplywith)
        {
            int return_index = -1;
            double min_difference = 10000000;
            byte[] axisvalues = m_TrionicFile.ReadData((uint)m_TrionicFile.GetFileInfo().GetSymbolAddressFlash(symbolname), (uint)m_TrionicFile.GetFileInfo().GetSymbolLength(symbolname));
            if (m_TrionicFile.GetFileInfo().isSixteenBitTable(symbolname))
            {
                for (int t = 0; t < axisvalues.Length; t += 2)
                {
                    int b = (int)(byte)axisvalues.GetValue(t) * 256;
                    b += (int)(byte)axisvalues.GetValue(t + 1);
                    b *= multiplywith;
                    double diff = Math.Abs((double)b - value);
                    if (min_difference > diff)
                    {
                        min_difference = diff;
                        // this is our index
                        return_index = t / 2;
                        // Console.WriteLine("Difference was: " + diff.ToString() + " at index " + return_index.ToString());

                    }
                }
            }
            else
            {
                for (int t = 0; t < axisvalues.Length; t++)
                {
                    int b = (int)(byte)axisvalues.GetValue(t);
                    b *= multiplywith;
                    double diff = Math.Abs((double)b - value);
                    if (min_difference > diff)
                    {
                        min_difference = diff;
                        // this is our index
                        return_index = t;
                    }
                }
            }
            return return_index;
        }

        private double CalculateConversionFactor(MapSensorType fromSensorType, MapSensorType toSensorType)
        {
            double factor = 1.2;
            switch (fromSensorType)
            {
                case MapSensorType.MapSensor25:
                    switch (toSensorType)
                    {
                        case MapSensorType.MapSensor25:
                            factor = 1.0;           // from 2.5 to 2.5 mapsensor
                            break;
                        case MapSensorType.MapSensor30:
                            factor = 1.2;           // from 2.5 to 3.0 mapsensor
                            break;
                        case MapSensorType.MapSensor35:
                            factor = 1.4;           // from 2.5 to 3.5 mapsensor
                            break;
                        case MapSensorType.MapSensor40:
                            factor = 1.6;           // from 2.5 to 4.0 mapsensor
                            break;
                        case MapSensorType.MapSensor50:
                            factor = 2.0;           // from 2.5 to 5.0 mapsensor
                            break;
                    }
                    break;
                case MapSensorType.MapSensor30:
                    switch (toSensorType)
                    {
                        case MapSensorType.MapSensor25:
                            factor = 0.8333;           // from 3.0 to 2.5 mapsensor
                            break;
                        case MapSensorType.MapSensor30:
                            factor = 1.0;           // from 3.0 to 3.0 mapsensor
                            break;
                        case MapSensorType.MapSensor35:
                            factor = 1.1666;           // from 3.0 to 3.5 mapsensor
                            break;
                        case MapSensorType.MapSensor40:
                            factor = 1.3333;           // from 3.0 to 4.0 mapsensor
                            break;
                        case MapSensorType.MapSensor50:
                            factor = 1.6667;           // from 3.0 to 5.0 mapsensor
                            break;
                    }
                    break;
                case MapSensorType.MapSensor35:
                    switch (toSensorType)
                    {
                        case MapSensorType.MapSensor25:
                            factor = 0.7143;           // from 3.5 to 2.5 mapsensor
                            break;
                        case MapSensorType.MapSensor30:
                            factor = 0.8571;           // from 3.5 to 3.0 mapsensor
                            break;
                        case MapSensorType.MapSensor35:
                            factor = 1.0;           // from 3.5 to 3.5 mapsensor
                            break;
                        case MapSensorType.MapSensor40:
                            factor = 1.1429;           // from 3.5 to 4.0 mapsensor
                            break;
                        case MapSensorType.MapSensor50:
                            factor = 1.4285;           // from 3.5 to 5.0 mapsensor
                            break;
                    }
                    break;
                case MapSensorType.MapSensor40:
                    switch (toSensorType)
                    {
                        case MapSensorType.MapSensor25:
                            factor = 0.6250;           // from 4.0 to 2.5 mapsensor
                            break;
                        case MapSensorType.MapSensor30:
                            factor = 0.75;           // from 4.0 to 3.0 mapsensor
                            break;
                        case MapSensorType.MapSensor35:
                            factor = 0.875;           // from 4.0 to 3.5 mapsensor
                            break;
                        case MapSensorType.MapSensor40:
                            factor = 1.0;           // from 4.0 to 4.0 mapsensor
                            break;
                        case MapSensorType.MapSensor50:
                            factor = 1.25;           // from 4.0 to 5.0 mapsensor
                            break;
                    }
                    break;
                case MapSensorType.MapSensor50:
                    switch (toSensorType)
                    {
                        case MapSensorType.MapSensor25:
                            factor = 0.5;           // from 5.0 to 2.5 mapsensor
                            break;
                        case MapSensorType.MapSensor30:
                            factor = 0.6;           // from 5.0 to 3.0 mapsensor
                            break;
                        case MapSensorType.MapSensor35:
                            factor = 0.7;           // from 5.0 to 3.5 mapsensor
                            break;
                        case MapSensorType.MapSensor40:
                            factor = 0.8;           // from 5.0 to 4.0 mapsensor
                            break;
                        case MapSensorType.MapSensor50:
                            factor = 1.0;           // from 5.0 to 5.0 mapsensor
                            break;
                    }
                    break;
            }
            return factor;
        }

        private int LookUpIndexAxisMap(double value, string symbolname)
        {
            int return_index = -1;
            double min_difference = 10000000;

            double factor = mapsensorfactor;//CalculateConversionFactor(MapSensorType.MapSensor25, m_TrionicFile.GetMapSensorType(false));

            byte[] axisvalues = m_TrionicFile.ReadData((uint)m_TrionicFile.GetFileInfo().GetSymbolAddressFlash(symbolname), (uint)m_TrionicFile.GetFileInfo().GetSymbolLength(symbolname));

            if (m_TrionicFile.GetFileInfo().isSixteenBitTable(symbolname))
            {
                for (int t = 0; t < axisvalues.Length; t += 2)
                {
                    int b = (int)(byte)axisvalues.GetValue(t) * 256;
                    b += (int)(byte)axisvalues.GetValue(t + 1);

                    /*if (threebarsensor)*/
                    b = (int)((double)b * /*1.2*/ factor);
                    double diff = Math.Abs((double)b - value);

                    if (min_difference > diff)
                    {
                        min_difference = diff;
                        // this is our index
                        return_index = t / 2;
                        // Console.WriteLine("Difference was: " + diff.ToString() + " at index " + return_index.ToString());

                    }
                }
            }
            else
            {

                for (int t = 0; t < axisvalues.Length; t++)
                {
                    int b = (int)(byte)axisvalues.GetValue(t);

                    /*if (threebarsensor)*/
                    b = (int)((double)b * /*1.2*/ factor);

                    double diff = Math.Abs((double)b - value);
                    if (min_difference > diff)
                    {
                        min_difference = diff;
                        // this is our index
                        return_index = t;
                    }
                }
            }
            //Console.WriteLine("Index found = " + return_index.ToString());
            return return_index;
        }

        public int[] GetIgnitionCountermap()
        {
            return IgnitionMapCounterInMemory;
        }

        public float[] GetFeedbackIgnitionMap()
        {
            return IgnitionMapInMemory;
        }

        public int[] GetFeedbackIgnitionMapinIntegers()
        {
            return LoadTargetIgnitionMapInIntegers(IgnitionMapInMemory);
        }

        public int[] GetFeedBackmapInIntegers()
        {
            return LoadTargetIgnitionMapInIntegers(IgnitionMapInMemory);
        }

        private void UpdateFeedbackMaps()
        {
            // cast an event to the main application to refresh the Ignition maps if visible
        }

        public void SaveMaps()
        {
            SaveIgnitionAndCounterMaps();
        }

        public string GetFeedbackIgnitionMapname()
        {
            string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "IgnitionMaps");
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }

            return Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-feedbackignitiontab.ign");
        }


        private void SaveIgnitionAndCounterMaps()
        {
            try
            {
                if (IgnitionMapInMemory != null)
                {
                    // save it to the correct filename
                    SaveTargetIgnitionMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-IgnitionFeedbackmap.ign", IgnitionMapInMemory);
                }
                if (IgnitionMapCounterInMemory != null)
                {
                    // save it
                    SaveTargetIgnitionCounterMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-IgnitionFeedbackCountermap.ign", IgnitionMapCounterInMemory);
                }
                if (ignitionlockedmap != null)
                {
                    SaveTargetIgnitionCounterMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-IgnitionLockedmap.ign", ignitionlockedmap);
                }
            }
            catch (Exception stargetE)
            {
                Console.WriteLine(stargetE.Message);
            }
        }




        
        byte[] open_loop=null;
        private bool IsCellClosedLoop(int mapvalue, int rpmindex)
        {
            if (open_loop == null)
            {
                open_loop = m_TrionicFile.ReadData((uint)m_TrionicFile.GetFileInfo().GetSymbolAddressFlash("Open_loop!"), (uint)m_TrionicFile.GetFileInfo().GetSymbolLength("Open_loop!"));
            }
            // look up the value in open_loop! map
            if (open_loop.Length > 1)
            {
                int closed_loop_limit = Convert.ToInt32(open_loop.GetValue(rpmindex));
                if (closed_loop_limit < mapvalue) return false;
            }
            return true;
        }

        private void SaveTargetIgnitionMap(string filename, float[] map)
        {
            int columns = 18;
            int rows = 16;
            

            if (m_TrionicFile.Exists())
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "IgnitionMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                filename = Path.Combine(foldername, filename);
                //m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetIgnitionMap(), out columns, out rows);
                if (map.Length == columns * rows)
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
                else
                {
                    //  MessageBox.Show("Target IGN map has different length than main VE map");
                }
            }
        }

        private void AddLogEntry(string entry)
        {
            if (m_TrionicFile.Exists())
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "IgnitionMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }

                string filename = Path.Combine(foldername, DateTime.Now.Date.ToString("yyyyMMdd") + "-IgnitionTuning.log");
                using (StreamWriter sw = new StreamWriter(filename, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + entry);
                }
            }
        }

        private void SaveTargetIgnitionCounterMap(string filename, int[] map)
        {
            int columns = 18;
            int rows = 16;
            if (m_TrionicFile.Exists())
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "IgnitionMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                filename = Path.Combine(foldername, filename);

                //m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetIgnitionMap(), out columns, out rows);
                if (map.Length == columns * rows)
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
                else
                {
                    //   MessageBox.Show("Target IGN counter map has different length than main VE map");
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

        public int[] LoadTargetIgnitionMapInIntegers()
        {
            string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "IgnitionMaps");
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }

            string filename = Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetignition.ign");
            int[] map = new int[m_TrionicFile.GetFileInfo().GetSymbolLength(m_TrionicFile.GetFileInfo().GetIgnitionMap())/2];
            map.Initialize();
            if (m_TrionicFile.Exists())
            {
                if (File.Exists(filename))
                {
                    try
                    {
                        // read all lines
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
                                        int val = (int)(Math.Ceiling(ConvertToDouble(value) * 10));
                                        byte b1 = (byte)(val / 256);
                                        byte b2 = (byte)(val - (int)b1 * 256);
                                        map.SetValue(b1, idx++);
                                        map.SetValue(b2, idx++);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        //  MessageBox.Show("Failed to load target IGN map: " + E.Message);
                        Console.WriteLine(E.Message);

                    }
                }
            }
            else
            {
                map = new int[0x120];
                map.Initialize();
            }
            return map;

        }


        

        private int[] LoadTargetIgnitionMapInIntegers(float[] orginalmap)
        {

            int[] map = new int[/*m_TrionicFile.GetFileInfo().GetSymbolLength(m_TrionicFile.GetFileInfo().GetIgnitionMap())*/ orginalmap.Length];
            map.Initialize();
            if (m_TrionicFile.Exists())
            {
                try
                {
                    int idx = 0;
                    foreach (float f in orginalmap)
                    {
                        int val = (int)(Math.Ceiling(f * 10));
                        map.SetValue(val, idx++);
                    }
                }
                catch (Exception E)
                {
                    //   MessageBox.Show("Failed to load target IGN map: " + E.Message);
                    Console.WriteLine(E.Message);
                }
            }
            else
            {
                map = new int[orginalmap.Length];
                map.Initialize();
            }
            return map;
        }

        

        private float[] LoadTargetIgnitionMap(string filename)
        {
            // load the target IGN map into memory
            float[] map = new float[m_TrionicFile.GetFileInfo().GetSymbolLength(m_TrionicFile.GetFileInfo().GetIgnitionMap())/2];
            map.Initialize();
            if (m_TrionicFile.Exists())
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "IgnitionMaps");
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
                        //      MessageBox.Show("Failed to load target IGN map: " + E.Message);
                        // something went wrong, try to reinitialize the map
                        Console.WriteLine(E.Message);

                    }
                }
            }
            else
            {
                map = new float[0x120];
                map.Initialize();
            }
            return map;
        }



        private int[] LoadCounterMap(string filename)
        {
            // load the target IGN map into memory
            int[] map = new int[m_TrionicFile.GetFileInfo().GetSymbolLength(m_TrionicFile.GetFileInfo().GetIgnitionMap()) / 2 ];
            map.Initialize();
            if (m_TrionicFile.Exists())
            {
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
                    catch (Exception E)
                    {
                        //  MessageBox.Show("Failed to load target IGN counter map: " + E.Message);
                        Console.WriteLine(E.Message);
                    }
                }
            }
            else
            {
                map = new int[0x120];
                map.Initialize();

            }
            return map;
        }


        private bool IsAllZero(float[] values)
        {
            for (int t = 0; t < values.Length; t++)
            {
                if (values[t] != 0) return false;
            }
            return true;
        }


        public void ClearIgnitionFeedbackMapCell(int mapindex)
        {
            try
            {
                if (IgnitionMapInMemory != null)
                {
                    IgnitionMapInMemory[mapindex] = 0;
                }
                if (IgnitionMapCounterInMemory != null)
                {
                    IgnitionMapCounterInMemory[mapindex] = 0;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }
    }
}
