using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Trionic5Tools
{
    public enum AFRMapType : int
    {
        TargetAFRMap,
        FeedbackAFRMap,
        DifferenceAFRMap,
        IdleTargetAFRMap,
        IdleFeedbackAFRMap,
        IdleDifferenceAFRMap
    }

    public class AFRMaps
    {
        public delegate void FuelmapCellChanged(object sender, FuelmapChangedEventArgs e);
        public event AFRMaps.FuelmapCellChanged onFuelmapCellChanged;

        public delegate void IdleFuelmapCellChanged(object sender, IdleFuelmapChangedEventArgs e);
        public event AFRMaps.IdleFuelmapCellChanged onIdleFuelmapCellChanged;


        public delegate void CellLocked(object sender, EventArgs e);
        public event AFRMaps.CellLocked onCellLocked;

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

        public class IdleFuelmapChangedEventArgs : System.EventArgs
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

            public IdleFuelmapChangedEventArgs(int mapindex, byte cellvalue)
            {
                this._mapindex = mapindex;
                this._cellvalue = cellvalue;
            }
        }

        private FuelMapInformation m_FuelMapInformation = new FuelMapInformation();

        public FuelMapInformation FuelMapInformation
        {
            get { return m_FuelMapInformation; }
            set { m_FuelMapInformation = value; }
        }

        private IECUFile m_TrionicFile;

        private byte[] fuelmap;
        private byte[] originalfuelmap;
        private int[] fuelcorrectioncountermap;
        private float[] targetmap;
        private float[] AFRMapInMemory;
        private int[] AFRMapCounterInMemory;
        private int[] AFRlockedmap;

        private byte[] idlefuelmap;
        private byte[] idleoriginalfuelmap;
        private int[] idlefuelcorrectioncountermap;
        private float[] idletargetmap;
        private float[] idleAFRMapInMemory;
        private int[] idleAFRMapCounterInMemory;
        private int[] idleAFRlockedmap;


        private string _wideBandAFRSymbol = string.Empty;

        private int _AcceptableTargetErrorPercentage;
        public int AcceptableTargetErrorPercentage
        {
            get { return _AcceptableTargetErrorPercentage; }
            set { _AcceptableTargetErrorPercentage = value; }
        }
        private int _AreaCorrectionPercentage;

        public int AreaCorrectionPercentage
        {
            get { return _AreaCorrectionPercentage; }
            set { _AreaCorrectionPercentage = value; }
        }
        private bool _AutoUpdateFuelMap;

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
        private int _CorrectionPercentage;

        public int CorrectionPercentage
        {
            get { return _CorrectionPercentage; }
            set { _CorrectionPercentage = value; }
        }
        private bool _DiscardClosedThrottleMeasurements;

        public bool DiscardClosedThrottleMeasurements
        {
            get { return _DiscardClosedThrottleMeasurements; }
            set { _DiscardClosedThrottleMeasurements = value; }
        }
        private bool _DiscardFuelcutMeasurements;

        public bool DiscardFuelcutMeasurements
        {
            get { return _DiscardFuelcutMeasurements; }
            set { _DiscardFuelcutMeasurements = value; }
        }
        private int _EnrichmentFilter;

        public int EnrichmentFilter
        {
            get { return _EnrichmentFilter; }
            set { _EnrichmentFilter = value; }
        }
        private int _FuelCutDecayTime_ms;

        public int FuelCutDecayTime_ms
        {
            get { return _FuelCutDecayTime_ms; }
            set { _FuelCutDecayTime_ms = value; }
        }
        private int _MaximumAdjustmentPerCyclePercentage;

        public int MaximumAdjustmentPerCyclePercentage
        {
            get { return _MaximumAdjustmentPerCyclePercentage; }
            set { _MaximumAdjustmentPerCyclePercentage = value; }
        }
        private int _MaximumAFRDeviance;

        public int MaximumAFRDeviance
        {
            get { return _MaximumAFRDeviance; }
            set { _MaximumAFRDeviance = value; }
        }
        private int _MinimumAFRMeasurements;

        public int MinimumAFRMeasurements
        {
            get { return _MinimumAFRMeasurements; }
            set { _MinimumAFRMeasurements = value; }
        }

        public string WideBandAFRSymbol
        {
            get { return _wideBandAFRSymbol; }
            set { _wideBandAFRSymbol = value; }
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
                        _currentAFR = 0;
                        _cellDurationMonitor.Stop();
                        _cellDurationMonitor.Reset();
                    }
                }
            }
        }

        private bool _HasValidFuelmap = false;

        public bool HasValidFuelmap
        {
            get { return _HasValidFuelmap; }
            set { _HasValidFuelmap = value; }
        }

        private bool _HasValidIdleFuelmap = false;

        public bool HasValidIdleFuelmap
        {
            get { return _HasValidIdleFuelmap; }
            set { _HasValidIdleFuelmap = value; }
        }

        public IECUFile TrionicFile
        {
            get { return m_TrionicFile; }
            set { m_TrionicFile = value; }
        }

        private double _currentEngineSpeed = 0;
        private double _currentBoostLevel = 0;
        private double _currentThrottlePosition = 0;
        private double _currentAFR = 0;
        private Stopwatch _cellDurationMonitor = new Stopwatch();
        private int _monitoringCellRPMIndex = -1;
        private int _monitoringCellMAPIndex = -1;
        private AFRMeasurementCollection _afrMeasurements = new AFRMeasurementCollection();

        public void InitAutoTuneVars(bool ClosedLoopOn)
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
                InitAFRFeedbackMaps();
                m_FuelMapInformation = new FuelMapInformation();
                // set default values
               // m_FuelMapInformation.SetOriginalFuelMap(fuelmap);
            }

        }

        public void IdleLockCell(int mapindex, int rpmindex)
        {
            idleAFRlockedmap.SetValue(1, (rpmindex * 12) + mapindex);
            SaveIdleLockedMap();
        }

        public void IdleUnlockCell(int mapindex, int rpmindex)
        {
            idleAFRlockedmap.SetValue(0, (rpmindex * 12) + mapindex);
            SaveIdleLockedMap();
        }

        private void SaveIdleLockedMap()
        {
            if (idleAFRlockedmap != null)
            {
                SaveIdleTargetAFRCounterMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idleAFRLockedmap.afr", idleAFRlockedmap);
            }

        }

        public void LockCell(int mapindex, int rpmindex)
        {
            AFRlockedmap.SetValue(1, (rpmindex * 16) + mapindex);
            SaveLockedMap();
        }

        public void UnlockCell(int mapindex, int rpmindex)
        {
            AFRlockedmap.SetValue(0, (rpmindex * 16) + mapindex);
            SaveLockedMap();
        }

        private void SaveLockedMap()
        {
            if (AFRlockedmap != null)
            {
                SaveTargetAFRCounterMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-AFRLockedmap.afr", AFRlockedmap);
            }

        }

        public int[] GetAFRLockedMap()
        {
            return AFRlockedmap;
        }

        public int[] GetIdleAFRLockedMap()
        {
            return idleAFRlockedmap;
        }

        /// <summary>
        /// Checks if the current cell is locked (we're not allowed to alter this cell)
        /// </summary>
        /// <param name="_monitoringCellMAPIndex"></param>
        /// <param name="_monitoringCellRPMIndex"></param>
        /// <returns></returns>
        private bool IsCellLocked(int mapindex, int rpmindex)
        {
            if ((int)AFRlockedmap.GetValue((rpmindex * 16) + mapindex) > 0)
            {
                return true;
            }
            return false;
        }

        private bool IsIdleCellLocked(int mapindex, int rpmindex)
        {
            if ((int)idleAFRlockedmap.GetValue((rpmindex * 12) + mapindex) > 0)
            {
                return true;
            }
            return false;
        }

        public void ClearIgnitionLockedMap()
        {
            for (int i = 0; i < AFRlockedmap.Length; i++) AFRlockedmap.SetValue(0, i);
            SaveLockedMap();
        }

        private void InitAFRFeedbackMaps()
        {
            int columns = 16;
            int rows = 16;

            m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetInjectionMap(), out columns, out rows);
            if (columns != 0)
            {
                AFRMapInMemory = new float[rows * columns];
                AFRMapCounterInMemory = new int[rows * columns];
                idleAFRMapInMemory = new float[12 * 8];
                idleAFRMapCounterInMemory = new int[12 * 8];
                SaveAFRAndCounterMaps();
            }
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

        private void CastIdleFuelmapCellChangedEvent(int mapindex, byte cellvalue)
        {
            if (onIdleFuelmapCellChanged != null)
            {
                onIdleFuelmapCellChanged(this, new IdleFuelmapChangedEventArgs(mapindex, cellvalue));
            }
        }

        public double[] GetPercentualDifferences()
        {
            double[] retval = m_FuelMapInformation.GetDifferencesInPercentages();
            for(int t= 0; t < retval.Length; t ++)
            {
                if (retval[t] > _MaximumAdjustmentPerCyclePercentage) retval[t] = _MaximumAdjustmentPerCyclePercentage;
            }
            return retval;
        }

        public double[] GetIdlePercentualDifferences()
        {
            double[] retval = m_FuelMapInformation.GetIdleDifferencesInPercentages();
            for (int t = 0; t < retval.Length; t++)
            {
                if (retval[t] > _MaximumAdjustmentPerCyclePercentage) retval[t] = _MaximumAdjustmentPerCyclePercentage;
            }
            return retval;
        }


        /// <summary>
        /// clears data that might cause problems when switching from idle to normal fuel map or v.v.
        /// </summary>
        public void AutoTuneSwitchMap()
        {
            _monitoringCellRPMIndex = -1;
            _monitoringCellMAPIndex = -1;
            _afrMeasurements.Clear();
        }

        public float GetCurrentTargetAFR(double rpm, double boost)
        {
            double currentLoad = boost;
            float targetafr_currentcell = 0;
            currentLoad += 1;
            currentLoad /= 0.01;
            int rpmindex = LookUpIndexAxisRPMMap(rpm, "Fuel_map_yaxis!", 10);
            int mapindex = LookUpIndexAxisMap(currentLoad, "Fuel_map_xaxis!");
            targetafr_currentcell = (float)targetmap.GetValue((rpmindex * 16) + mapindex);
            return targetafr_currentcell;
        }

        //when starting autotune function, first the adaption map should be implemented
        //into the main fuel map, and than the adaption map must be cleared.

        public float HandleRealtimeData(double rpm, double tps, double boost, double afr, bool _idleMapActive)
        {
            float targetafr_currentcell = 0;
            if (_isAutoMappingActive && _wideBandAFRSymbol != "") // must be wideband
            {

                if (_idleMapActive)
                {
                    double currentLoad = boost;
                    currentLoad += 1;
                    currentLoad /= 0.01;
                    int rpmindex = LookUpIndexAxisRPMMap(rpm, "Idle_st_rpm!", 10);
                    int mapindex = LookUpIndexAxisMap(currentLoad, "Idle_st_last!");

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
                                Console.WriteLine("Stable in cell: " + rpmindex.ToString() + " " + mapindex.ToString());
                                if (!IsIdleCellLocked(mapindex, rpmindex))
                                {
                                    // check afr against target afr
                                    // get the average AFR from the collection
                                    float average_afr_in_cell = (float)GetAverageFromMeasurements();

                                    targetafr_currentcell = (float)idletargetmap.GetValue((rpmindex * 12) + mapindex);
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

                                        // get the current fuel map value for the given cell
                                        // get the number of changes that have been made already to this cell
                                        // if the number of changes > x then don't do changes anymore and notify the user
                                        int _fuelcorrectionvalue = (int)idlefuelmap[(rpmindex * 12) + mapindex];

                                        if (average_afr_in_cell > targetafr_currentcell)
                                        {
                                            // we're running too lean, so we need to increase the fuelmap value by afr_diff_to_correct %
                                            // get the current fuelmap value
                                            // correct it with the percentage

                                            _fuelcorrectionvalue *= (int)(100 + afr_diff_to_correct);
                                            _fuelcorrectionvalue /= 100;
                                            if (_fuelcorrectionvalue > 254) _fuelcorrectionvalue = 254;
                                            // save it to the map

                                        }
                                        else
                                        {
                                            // we're running too rich, so we need to decrease the fuelmap value by afr_diff_to_correct %
                                            // correct it with the percentage
                                            _fuelcorrectionvalue *= (int)(100 - afr_diff_to_correct);
                                            _fuelcorrectionvalue /= 100;
                                            // don't go under 25, seems to be some kind of boundary!
                                            // <GS-28102010> changed for testing purposes, if problems occur, revert back to 25 as boundary
                                            if (_fuelcorrectionvalue < 1) _fuelcorrectionvalue = 1;

                                            // save it to the map
                                        }
                                        if (idlefuelmap[(rpmindex * 12) + mapindex] != (byte)_fuelcorrectionvalue)
                                        {
                                            // if autowrite to ECU
                                            if (_AutoUpdateFuelMap)
                                            {
                                                // if the user should be notified, do so now, ask permission to alter the fuel map
                                                //Console.WriteLine("Altering rpmidx: " + rpmindex.ToString() + " mapidx: " + mapindex.ToString() + " from value: " + fuelmap[(rpmindex * 16) + mapindex].ToString() + " to value: " + _fuelcorrectionvalue.ToString());
                                                idlefuelmap[(rpmindex * 12) + mapindex] = (byte)_fuelcorrectionvalue;
                                                // increase counter
                                                idlefuelcorrectioncountermap[(rpmindex * 12) + mapindex]++;
                                                // cast an event that will write the data into the ECU (?)
                                                CastIdleFuelmapCellChangedEvent((rpmindex * 12) + mapindex, (byte)_fuelcorrectionvalue);
                                            }
                                            else
                                            {
                                                // keep track of the changes in the fuelmapinfo
                                                m_FuelMapInformation.UpdateIdleFuelMap(mapindex, rpmindex, _fuelcorrectionvalue);
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
                                else
                                {
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
                else
                {
                    // only handle this stuff if automapping is active
                    // first off, get the index in the fuel map for the given engine speed and load
                    double currentLoad = boost;
                    currentLoad += 1;
                    currentLoad /= 0.01;
                    int rpmindex = LookUpIndexAxisRPMMap(rpm, "Fuel_map_yaxis!", 10);
                    int mapindex = LookUpIndexAxisMap(currentLoad, "Fuel_map_xaxis!");
                    //Console.WriteLine("MAP index: " + mapindex.ToString() + " for load: " + boost.ToString());
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
                                if (!IsCellLocked(mapindex, rpmindex))
                                {
                                    float average_afr_in_cell = (float)GetAverageFromMeasurements();
                                    targetafr_currentcell = (float)targetmap.GetValue((rpmindex * 16) + mapindex);
                                    // calculate the difference in percentage
                                    float _afr_diff_percentage = Math.Abs(((targetafr_currentcell - average_afr_in_cell) / targetafr_currentcell) * 100);
                                    if (_afr_diff_percentage > _AcceptableTargetErrorPercentage)
                                    {
                                        //Console.WriteLine("Stable in cell: " + rpmindex.ToString() + " " + mapindex.ToString() + " afrtarget = " + targetafr_currentcell.ToString("F2") + " afravg: " + average_afr_in_cell.ToString("F2") + " percdiff: " + _afr_diff_percentage.ToString("F2"));
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

                                        // get the current fuel map value for the given cell
                                        // get the number of changes that have been made already to this cell
                                        // if the number of changes > x then don't do changes anymore and notify the user
                                        int _fuelcorrectionvalue = (int)fuelmap[(rpmindex * 16) + mapindex];

                                        if (average_afr_in_cell > targetafr_currentcell)
                                        {

                                            if (afr_diff_to_correct < 2)
                                            {
                                                Console.WriteLine("Stable in cell (LEAN): " + rpmindex.ToString() + " " + mapindex.ToString() + " afrtarget = " + targetafr_currentcell.ToString("F2") + " afravg: " + average_afr_in_cell.ToString("F2") + " percdiff: " + _afr_diff_percentage.ToString("F2") + " corrperc: " + afr_diff_to_correct.ToString("F2"));
                                                Console.WriteLine("Ori fuel value: " + _fuelcorrectionvalue.ToString());
                                            }

                                            // we're running too lean, so we need to increase the fuelmap value by afr_diff_to_correct %
                                            // get the current fuelmap value
                                            // correct it with the percentage
                                            float _tempcorrectionvalue = _fuelcorrectionvalue;

                                            _tempcorrectionvalue *= 100F + afr_diff_to_correct;
                                            _tempcorrectionvalue /= 100F;
                                            if (afr_diff_to_correct < 10)
                                            {
                                                Console.WriteLine("Multiply fuel value: " + _tempcorrectionvalue.ToString("F3"));
                                            }
                                            if (_tempcorrectionvalue > 254) _tempcorrectionvalue = 254;
                                            _fuelcorrectionvalue = Convert.ToInt32(Math.Round(_tempcorrectionvalue));
                                            if (afr_diff_to_correct < 2)
                                            {
                                                Console.WriteLine("New fuel value: " + _fuelcorrectionvalue.ToString());
                                            }
                                            /*_fuelcorrectionvalue *= (int)(100 + afr_diff_to_correct);
                                            if (afr_diff_to_correct < 10)
                                            {
                                                Console.WriteLine("Multiply fuel value: " + _fuelcorrectionvalue.ToString());
                                            }

                                            _fuelcorrectionvalue /= 100;
                                            if (afr_diff_to_correct < 10)
                                            {
                                                Console.WriteLine("New fuel value: " + _fuelcorrectionvalue.ToString());
                                            }

                                            if (_fuelcorrectionvalue > 254) _fuelcorrectionvalue = 254;*/
                                            // save it to the map

                                        }
                                        else
                                        {
                                            // we're running too rich, so we need to decrease the fuelmap value by afr_diff_to_correct %
                                            // correct it with the percentage
                                            Console.WriteLine("Stable in cell (RICH): " + rpmindex.ToString() + " " + mapindex.ToString() + " afrtarget = " + targetafr_currentcell.ToString("F2") + " afravg: " + average_afr_in_cell.ToString("F2") + " percdiff: " + _afr_diff_percentage.ToString("F2") + " corrperc: " + afr_diff_to_correct.ToString("F2"));
                                            float _tempcorrectionvalue = _fuelcorrectionvalue;
                                            _tempcorrectionvalue *= 100F - afr_diff_to_correct;
                                            _tempcorrectionvalue /= 100F;
                                            if (_tempcorrectionvalue < 1) _tempcorrectionvalue = 1;
                                            _fuelcorrectionvalue = Convert.ToInt32(Math.Round(_tempcorrectionvalue));

                                            /*
                                            _fuelcorrectionvalue *= (int)(100 - afr_diff_to_correct);
                                            _fuelcorrectionvalue /= 100;
                                            // don't go under 25, seems to be some kind of boundary!
                                            // <GS-28102010> changed for testing purposes, if problems occur, revert back to 25 as boundary
                                            if (_fuelcorrectionvalue < 1) _fuelcorrectionvalue = 1;*/

                                            // save it to the map
                                        }
                                        if (fuelmap[(rpmindex * 16) + mapindex] != (byte)_fuelcorrectionvalue)
                                        {
                                            // if autowrite to ECU
                                            if (_AutoUpdateFuelMap)
                                            {
                                                // if the user should be notified, do so now, ask permission to alter the fuel map
                                                //Console.WriteLine("Altering rpmidx: " + rpmindex.ToString() + " mapidx: " + mapindex.ToString() + " from value: " + fuelmap[(rpmindex * 16) + mapindex].ToString() + " to value: " + _fuelcorrectionvalue.ToString());
                                                fuelmap[(rpmindex * 16) + mapindex] = (byte)_fuelcorrectionvalue;
                                                // increase counter
                                                fuelcorrectioncountermap[(rpmindex * 16) + mapindex]++;
                                                // cast an event that will write the data into the ECU (?)
                                                CastFuelmapCellChangedEvent((rpmindex * 16) + mapindex, (byte)_fuelcorrectionvalue);
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
                                        Console.WriteLine("Fuelling correct in cell: " + rpmindex.ToString() + " " + mapindex.ToString() + " afrtarget = " + targetafr_currentcell.ToString("F2") + " afravg: " + average_afr_in_cell.ToString("F2") + " percdiff: " + _afr_diff_percentage.ToString("F2"));
                                        //what to do if AFR is correct (within limits), should we reset the stopwatch?
                                        _afrMeasurements.Clear(); // clear average collection
                                        _cellDurationMonitor.Stop();
                                        _cellDurationMonitor.Reset();
                                        _cellDurationMonitor.Start();
                                    }
                                }
                                else
                                {
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

            }
            return targetafr_currentcell;
        }

        public byte[] GetCurrentlyMutatedFuelMap()
        {
            return fuelmap;
        }

        public byte[] GetIdleCurrentlyMutatedFuelMap()
        {
            return idlefuelmap;
        }


        public int[] GetCurrentlyMutatedFuelMapCounter()
        {
            return fuelcorrectioncountermap;
        }

        public int[] GetIdleCurrentlyMutatedFuelMapCounter()
        {
            return idlefuelcorrectioncountermap;
        }

        public byte[] GetOriginalFuelmap()
        {
            return originalfuelmap;
        }

        public byte[] GetIdleOriginalFuelmap()
        {
            return idleoriginalfuelmap;
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

        public void SetIdleOriginalFuelMap(byte[] idlefuelmapdata)
        {
            idleoriginalfuelmap = new byte[idlefuelmapdata.Length];
            for (int t = 0; t < idlefuelmapdata.Length; t++)
            {
                idleoriginalfuelmap[t] = idlefuelmapdata[t];
            }
            m_FuelMapInformation.SetIdleOriginalFuelMap(idlefuelmapdata);
        }


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
            for(int i = 0; i < fuelcorrectioncountermap.Length; i ++)
            {
                fuelcorrectioncountermap[i] = 0; // initialize
            }
            m_FuelMapInformation.SetOriginalFuelMap(fuelmapdata);
            _HasValidFuelmap = true;
        }

        public void SetIdleCurrentFuelMap(byte[] idlefuelmapdata)
        {
            // copy the data
            idlefuelmap = new byte[idlefuelmapdata.Length];
            for (int t = 0; t < idlefuelmapdata.Length; t++)
            {
                idlefuelmap[t] = idlefuelmapdata[t];
            }
            // and create a correction counter map with all 0's
            idlefuelcorrectioncountermap = new int[idlefuelmapdata.Length];
            for (int i = 0; i < idlefuelcorrectioncountermap.Length; i++)
            {
                idlefuelcorrectioncountermap[i] = 0; // initialize
            }
            m_FuelMapInformation.SetIdleOriginalFuelMap(idlefuelmapdata);
            _HasValidIdleFuelmap = true;
        }

        public void InitializeMaps()
        {
            // first get the data length we should use (default is 16x16 insp_mat)
            string injection_map_name = m_TrionicFile.GetFileInfo().GetInjectionMap();
            int map_length = m_TrionicFile.GetFileInfo().GetSymbolLength(injection_map_name);
            if (map_length == 0) map_length = 16 * 16; // fail safe for bins without a correct symbol table, e.g. maptun/nordic files etc
            try
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }

                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetafr.afr")))
                {
                    targetmap = LoadTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetafr.afr");
                }
                else
                {
                    //targetmap = null;
                    CreateTargetMap();

                }
                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-AFRLockedmap.afr")))
                {
                    AFRlockedmap = LoadCounterMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-AFRLockedmap.afr"));
                }
                else
                {
                    AFRlockedmap = new int[16 * 16];
                    SaveLockedMap();
                }

                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-AFRFeedbackmap.afr")))
                {
                    AFRMapInMemory = LoadTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-AFRFeedbackmap.afr");
                }
                else
                {
                    int columns = 16;
                    int rows = 16;

                    m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetInjectionMap(), out columns, out rows);
                    if (columns != 0)
                    {
                        AFRMapInMemory = new float[rows * columns];
                        AFRMapCounterInMemory = new int[rows * columns];
                        SaveAFRAndCounterMaps();
                    }
                }
                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-AFRFeedbackCountermap.afr")))
                {
                    AFRMapCounterInMemory = LoadCounterMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-AFRFeedbackCountermap.afr"));
                }
                else
                {
                    //AFRMapCounterInMemory = null;
                }
            }
            catch (Exception tafrE)
            {
                Console.WriteLine(tafrE.Message);
            }

            string idleinjection_map_name = "Idle_fuel_korr!";
            map_length = m_TrionicFile.GetFileInfo().GetSymbolLength(idleinjection_map_name);
            if (map_length == 0) map_length = 12 * 8; // fail safe for bins without a correct symbol table, e.g. maptun/nordic files etc
            try
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }

                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idletargetafr.afr")))
                {
                    idletargetmap = LoadIdleTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idletargetafr.afr");
                }
                else
                {
                    CreateIdleTargetMap();
                }

                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idleAFRLockedmap.afr")))
                {
                    idleAFRlockedmap = LoadIdleCounterMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idleAFRLockedmap.afr"));
                }
                else
                {
                    idleAFRlockedmap = new int[12 * 8];
                    SaveIdleLockedMap();
                }

                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idleAFRFeedbackmap.afr")))
                {
                    idleAFRMapInMemory = LoadIdleTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idleAFRFeedbackmap.afr");
                }
                else
                {
                    int columns = 12;
                    int rows = 8;

                    //m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetInjectionMap(), out columns, out rows);
                    if (columns != 0)
                    {
                        idleAFRMapInMemory = new float[rows * columns];
                        idleAFRMapCounterInMemory = new int[rows * columns];
                        
                        SaveAFRAndCounterMaps();
                    }
                }
                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idleAFRFeedbackCountermap.afr")))
                {
                    idleAFRMapCounterInMemory = LoadIdleCounterMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idleAFRFeedbackCountermap.afr"));
                }
                else
                {
                    //AFRMapCounterInMemory = null;
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

            double factor = CalculateConversionFactor(MapSensorType.MapSensor25, m_TrionicFile.GetMapSensorType(false));

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
                        //if (symbolname.StartsWith("Fuel_map_")) Console.WriteLine("Difference was: " + diff.ToString() + " at index " + return_index.ToString());
                        //Console.WriteLine("Difference was: " + diff.ToString() + " at index " + return_index.ToString() +  " boost was: "  + value.ToString());

                    }
                }
            }
            //Console.WriteLine("Index found = " + return_index.ToString());
            return return_index;
        }

        public int[] GetAFRCountermap()
        {
            return AFRMapCounterInMemory;
        }

        public int[] GetIdleAFRCountermap()
        {
            return idleAFRMapCounterInMemory;
        }

        public byte[] GetTargetAFRMapinBytes()
        {
            return LoadTargetAFRMapInBytes(targetmap);
        }

        public byte[] GetIdleTargetAFRMapinBytes()
        {
            return LoadTargetAFRMapInBytes(idletargetmap);
        }

        public float[] GetTargetAFRMap()
        {
            return targetmap;
        }

        public float[] GetIdleTargetAFRMap()
        {
            return idletargetmap;
        }


        public float[] GetFeedbackAFRMap()
        {
            return AFRMapInMemory;
        }

        public float[] GetIdleFeedbackAFRMap()
        {
            return idleAFRMapInMemory;
        }

        public byte[] GetFeedbackAFRMapinBytes()
        {
            return LoadTargetAFRMapInBytes(AFRMapInMemory);
        }

        public byte[] GetIdleFeedbackAFRMapinBytes()
        {
            return LoadTargetAFRMapInBytes(idleAFRMapInMemory);
        }

        public byte[] GetDifferenceMapinBytes()
        {
            if (targetmap == null)
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                

                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetafr.afr")))
                {
                    targetmap = LoadTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetafr.afr");
                }
                else
                {
                    targetmap = CreateDefaultTargetAFRMap();
                    SaveTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetafr.afr", targetmap);
                }
            }

            float[] tempmap = new float[AFRMapInMemory.Length];
            for (int t = 0; t < AFRMapInMemory.Length; t++)
            {
                if ((float)AFRMapInMemory.GetValue(t) != 0)
                {
                    float newval = (float)AFRMapInMemory.GetValue(t) - (float)targetmap.GetValue(t);
                    if (newval == 0)
                    {
                        newval = 0.06F;
                    }
                    tempmap.SetValue(newval, t);
                }
            }
            return LoadTargetAFRMapInBytes(tempmap);
            
        }

        public byte[] GetIdleDifferenceMapinBytes()
        {
            if (idletargetmap == null)
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }


                if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idletargetafr.afr")))
                {
                    idletargetmap = LoadIdleTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idletargetafr.afr");
                }
                else
                {
                    idletargetmap = CreateDefaultIdleTargetAFRMap();

                    SaveIdleTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idletargetafr.afr", idletargetmap);
                }
            }

            float[] tempmap = new float[idleAFRMapInMemory.Length];
            for (int t = 0; t < idleAFRMapInMemory.Length; t++)
            {
                if ((float)idleAFRMapInMemory.GetValue(t) != 0)
                {
                    float newval = (float)idleAFRMapInMemory.GetValue(t) - (float)idletargetmap.GetValue(t);
                    if (newval == 0)
                    {
                        newval = 0.06F;
                    }
                    tempmap.SetValue(newval, t);
                }
            }
            return LoadTargetAFRMapInBytes(tempmap);

        }

        public byte[] GetFeedBackmapInBytes()
        {
            return LoadTargetAFRMapInBytes(AFRMapInMemory);
        }

        public byte[] GetIdleFeedBackmapInBytes()
        {
            return LoadTargetAFRMapInBytes(idleAFRMapInMemory);
        }

        public void LogWidebandAFR(double afr, double _lastRPM, double _lastLoad, bool _isIdleMapActive)
        {
            try
            {
                //Console.WriteLine("Wideband log: " + afr.ToString() + " rpm: " + _lastRPM.ToString() + " load: " + _lastLoad.ToString());
                if (_lastLoad != -1 && _lastRPM > 600 && afr > 0 && afr < 25)
                {
                    if (!_isIdleMapActive)
                    {
                        int columns = 0;
                        int rows = 0;

                        m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetInjectionMap(), out columns, out rows);
                        // calculate averages on the fly so we can display the AFR map live as well.
                        if (columns != 0)
                        {
                            if (AFRMapInMemory == null)
                            {
                                AFRMapInMemory = new float[rows * columns];
                                AFRMapCounterInMemory = new int[rows * columns];
                            }
                            _lastLoad += 1;
                            _lastLoad /= 0.01;
                            int rpmindex = LookUpIndexAxisRPMMap(_lastRPM, "Fuel_map_yaxis!", 10);
                            int mapindex = LookUpIndexAxisMap(_lastLoad, "Fuel_map_xaxis!");
                            // get current counter
                            int current_count = Convert.ToInt32(AFRMapCounterInMemory.GetValue((rpmindex * columns) + mapindex));
                            float newvalue = (float)AFRMapInMemory.GetValue((rpmindex * columns) + mapindex);
                            //Console.WriteLine("Count: " + current_count.ToString() + " value: " + newvalue.ToString("F2"));
                            newvalue *= current_count;
                            newvalue += (float)afr;
                            current_count++;
                            newvalue /= current_count;
                            //Console.WriteLine("newvalue: " + newvalue.ToString("F2"));
                            // save both values
                            AFRMapInMemory.SetValue(newvalue, (rpmindex * columns) + mapindex);
                            AFRMapCounterInMemory.SetValue(current_count, (rpmindex * columns) + mapindex);
                            // make it fast, no calculations here... do the math when the user requests the target AFR and feedback AFR maps
                            UpdateFeedbackMaps();
                            //UpdateMapViewers(_currentEngineSpeed, _currentThrottlePosition, _currentBoostLevel);
                        }
                    }
                    else
                    {
                        int columns = 12;
                        int rows = 8;
                        // calculate averages on the fly so we can display the AFR map live as well.
                        if (columns != 0)
                        {
                            if (idleAFRMapInMemory == null)
                            {
                                idleAFRMapInMemory = new float[rows * columns];
                                idleAFRMapCounterInMemory = new int[rows * columns];
                            }
                            _lastLoad += 1;
                            _lastLoad /= 0.01;
                            int rpmindex = LookUpIndexAxisRPMMap(_lastRPM, "Idle_st_rpm!", 10);
                            int mapindex = LookUpIndexAxisMap(_lastLoad, "Idle_st_last!");
                            // get current counter
                            int current_count = Convert.ToInt32(idleAFRMapCounterInMemory.GetValue((rpmindex * columns) + mapindex));
                            float newvalue = (float)idleAFRMapInMemory.GetValue((rpmindex * columns) + mapindex);
                            //Console.WriteLine("Count: " + current_count.ToString() + " value: " + newvalue.ToString("F2"));
                            newvalue *= current_count;
                            newvalue += (float)afr;
                            current_count++;
                            newvalue /= current_count;
                            //Console.WriteLine("newvalue: " + newvalue.ToString("F2"));
                            // save both values
                            idleAFRMapInMemory.SetValue(newvalue, (rpmindex * columns) + mapindex);
                            idleAFRMapCounterInMemory.SetValue(current_count, (rpmindex * columns) + mapindex);
                            // make it fast, no calculations here... do the math when the user requests the target AFR and feedback AFR maps
                            UpdateIdleFeedbackMaps();
                            //UpdateMapViewers(_currentEngineSpeed, _currentThrottlePosition, _currentBoostLevel);
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("LogWidebandAFR: " + E.Message);
            }
        }

        private void UpdateFeedbackMaps()
        {
            // cast an event to the main application to refresh the AFR maps if visible
        }

        private void UpdateIdleFeedbackMaps()
        {
            // cast an event to the main application to refresh the idle AFR maps if visible
        }

        public void SetTargetAFRMapInBytes(byte[] data)
        {
            // set it to be the target AFR map
            for (int t = 0; t < data.Length; t += 2)
            {
                int value = Convert.ToInt32(data[t]) * 256;
                value += Convert.ToInt32(data[t + 1]);
                double dval = (double)value / 10;
                targetmap.SetValue((float)dval, t / 2);
            }
        }

        public void SetIdleTargetAFRMapInBytes(byte[] data)
        {
            // set it to be the target AFR map
            for (int t = 0; t < data.Length; t += 2)
            {
                int value = Convert.ToInt32(data[t]) * 256;
                value += Convert.ToInt32(data[t + 1]);
                double dval = (double)value / 10;
                idletargetmap.SetValue((float)dval, t / 2);
            }
        }

        public void SaveMaps()
        {
            SaveTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetafr.afr", targetmap);
            SaveAFRAndCounterMaps();
            SaveLockedMap();
        }

        public void SaveIdleMaps()
        {
            try
            {
                if (idletargetmap == null) idletargetmap = CreateDefaultIdleTargetAFRMap();
                SaveIdleTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idletargetafr.afr", idletargetmap);
                SaveAFRAndCounterMaps();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }


        public void CreateIdleTargetMap()
        {
            targetmap = CreateDefaultIdleTargetAFRMap();
            SaveIdleTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idletargetafr.afr", targetmap);
        }


        public void CreateTargetMap()
        {
            targetmap = CreateDefaultTargetAFRMap();
            SaveTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetafr.afr", targetmap);
            
        }

        public void CreateTargetMap(InjectorType injectorType)
        {
            targetmap = CreateDefaultTargetAFRMap(injectorType);
            SaveTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetafr.afr", targetmap);
        }

        public string GetFeedbackAFRMapname()
        {
            string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }

            return Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-feedbackafrtab.afr");
        }

        public string GetIdleFeedbackAFRMapname()
        {
            string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }

            return Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idlefeedbackafrtab.afr");
        }


        private void SaveAFRAndCounterMaps()
        {
            try
            {
                if (AFRMapInMemory != null)
                {
                    // save it to the correct filename
                    SaveTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-AFRFeedbackmap.afr", AFRMapInMemory);
                }
                if (AFRMapCounterInMemory != null)
                {
                    // save it
                    SaveTargetAFRCounterMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-AFRFeedbackCountermap.afr", AFRMapCounterInMemory);
                }

                if (idleAFRMapInMemory != null)
                {
                    // save it to the correct filename
                    SaveIdleTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idleAFRFeedbackmap.afr", idleAFRMapInMemory);
                }
                if (idleAFRMapCounterInMemory != null)
                {
                    // save it
                    SaveIdleTargetAFRCounterMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idleAFRFeedbackCountermap.afr", idleAFRMapCounterInMemory);
                }
            }
            catch (Exception stargetE)
            {
                Console.WriteLine(stargetE.Message);
            }
        }


        private float[] CreateDefaultIdleTargetAFRMap()
        {
            int columns = 12;
            int rows = 8;
            if (columns != 0)
            {
                float[] map = new float[rows * columns];

                for (int rpmtel = 0; rpmtel < rows; rpmtel++)
                {
                    for (int maptel = 0; maptel < columns; maptel++)
                    {
                        map.SetValue(14.7F, rpmtel * columns + maptel);
                    }
                }
                return map;
            }
            float[] map2 = new float[1];
            map2.SetValue(0F, 0);
            return map2;
        }

        private float[] CreateDefaultTargetAFRMap()
        {
            int columns = 0;
            int rows = 0;

            m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetInjectionMap(), out columns, out rows);
            if (columns != 0)
            {
                float[] map = new float[rows * columns];

                int[] fuel_xaxis = m_TrionicFile.GetMapXaxisValues(m_TrionicFile.GetFileInfo().GetInjectionMap());
                int[] fuel_yaxis = m_TrionicFile.GetMapYaxisValues(m_TrionicFile.GetFileInfo().GetInjectionMap());

                for (int rpmtel = 0; rpmtel < fuel_yaxis.Length; rpmtel++)
                {
                    for (int maptel = 0; maptel < fuel_xaxis.Length; maptel++)
                    {
                        int mapvalue = (int)fuel_xaxis.GetValue(maptel);
                        int rpmvalue = (int)fuel_yaxis.GetValue(rpmtel);
                        //rpmvalue += (int)fuel_yaxis.GetValue(rpmtel + 1);
                        //rpmvalue *=10;
                        float afrtarget = 14.7F;
                        // now, decrease as mapvalue increases
                        // top MAP should be ~12.0
                        if (mapvalue > 100)
                        {
                            // positive boost

                            afrtarget -= 3.5F * (float)maptel / (float)columns;
                            // compensate for peak torque (+/- 3000 rpm)
                            if (rpmvalue > 4000) rpmvalue = 4000 - rpmvalue % 4000;
                            if (rpmvalue < 0) rpmvalue = 0;
                            
                            afrtarget += Math.Abs((4000 - (float)rpmvalue) / 4000);
                        }

                        else if (mapvalue < 30)
                        {
                            // vacuum, 15.0
                            afrtarget = 15.0F;
                        }
                        if (rpmvalue < 1000) afrtarget = 13.0F;
                        map.SetValue(afrtarget, rpmtel * columns + maptel);
                    }
                }
                return map;
            }
            float[] map2 = new float[1];
            map2.SetValue(0F, 0);
            return map2;
        }

        private float[] CreateDefaultTargetAFRMap(InjectorType injectorType)
        {
            int columns = 0;
            int rows = 0;
            Trionic5Properties props = m_TrionicFile.GetTrionicProperties();
            


            m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetInjectionMap(), out columns, out rows);
            if (columns != 0)
            {
                float[] map = new float[rows * columns];

                int[] fuel_xaxis = m_TrionicFile.GetMapXaxisValues(m_TrionicFile.GetFileInfo().GetInjectionMap());
                int[] fuel_yaxis = m_TrionicFile.GetMapYaxisValues(m_TrionicFile.GetFileInfo().GetInjectionMap());

                for (int rpmtel = 0; rpmtel < fuel_yaxis.Length; rpmtel++)
                {
                    for (int maptel = 0; maptel < fuel_xaxis.Length; maptel++)
                    {
                        int mapvalue = (int)fuel_xaxis.GetValue(maptel);
                        int rpmvalue = (int)fuel_yaxis.GetValue(rpmtel);
                        //rpmvalue += (int)fuel_yaxis.GetValue(rpmtel + 1);
                        //rpmvalue *=10;
                        float afrtarget = 14.7F;
                        // now, decrease as mapvalue increases
                        // top MAP should be ~12.0
                        //if (mapvalue > 100)
                        {
                            // positive boost

                            afrtarget -= 3.0F * (float)maptel / (float)columns;
                            // compensate for peak torque (+/- 3000 rpm)
                            if (rpmvalue > 4000) rpmvalue = 4000 - rpmvalue % 4000;
                            if (rpmvalue < 0) rpmvalue = 0;

                            afrtarget += Math.Abs((4000 - (float)rpmvalue) / 4000);
                        }

                        /*else */if (mapvalue < 30)
                        {
                            // vacuum, 15.0
                            afrtarget = 15.0F;
                        }
                        if (props.Lambdacontrol == false)
                        {
                            if (mapvalue < 70 && rpmvalue < 1750) afrtarget = 13.5F;
                            if (rpmvalue < 1000) afrtarget = 13.5F;
                        }
                        if (props.Lambdacontrolduringidle == false)
                        {
                            if (rpmvalue < 1000) afrtarget = 13.5F;
                        }
                        /*if (injectorType == InjectorType.Siemens630Dekas || injectorType == InjectorType.Siemens875Dekas || injectorType == InjectorType.Siemens1000cc)
                        {
                            // because of the opening times of the injector, make the target mixture richer here.
                            if (mapvalue < 70 && rpmvalue < 1750) afrtarget = 13.5F;
                            if (rpmvalue < 1000) afrtarget = 13.5F;
                        }*/
                        // if in closed loop, always 14.7
                        if (props.Lambdacontrol)
                        {
                            if (IsCellClosedLoop(mapvalue, rpmtel))
                            {
                                if (props.Lambdacontrolduringidle == false && rpmvalue < 1000) ;
                                else afrtarget = 14.7F; 
                            }
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

        private void SaveTargetAFRMap(string filename, float[] map)
        {
            int columns = 0;
            int rows = 0;
            

            if (m_TrionicFile.Exists())
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                filename = Path.Combine(foldername, filename);
                m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetInjectionMap(), out columns, out rows);
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
                    //  MessageBox.Show("Target AFR map has different length than main VE map");
                }
            }
        }

        private void SaveIdleTargetAFRMap(string filename, float[] map)
        {
            int columns = 12;
            int rows = 8;


            if (m_TrionicFile.Exists())
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                filename = Path.Combine(foldername, filename);
                //m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetInjectionMap(), out columns, out rows);
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
                    //  MessageBox.Show("Target AFR map has different length than main VE map");
                }
            }
        }

        private void SaveTargetAFRCounterMap(string filename, int[] map)
        {
            int columns = 0;
            int rows = 0;
            if (m_TrionicFile.Exists())
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                filename = Path.Combine(foldername, filename);

                m_TrionicFile.GetMapMatrixWitdhByName(m_TrionicFile.GetFileInfo().GetInjectionMap(), out columns, out rows);
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
                    //   MessageBox.Show("Target AFR counter map has different length than main VE map");
                }
            }

        }

        private void SaveIdleTargetAFRCounterMap(string filename, int[] map)
        {
            int columns = 12;
            int rows = 8;
            if (m_TrionicFile.Exists())
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                filename = Path.Combine(foldername, filename);
                
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
                    //   MessageBox.Show("Target AFR counter map has different length than main VE map");
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

        public byte[] LoadTargetAFRMapInBytes()
        {
            string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }

            string filename = Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetafr.afr");
            byte[] map = new byte[m_TrionicFile.GetFileInfo().GetSymbolLength(m_TrionicFile.GetFileInfo().GetInjectionMap()) * 2];
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
                        //  MessageBox.Show("Failed to load target AFR map: " + E.Message);
                        Console.WriteLine(E.Message);

                    }
                }
            }
            else
            {
                map = new byte[0x100 * 2];
                map.Initialize();
            }
            return map;

        }


        public byte[] LoadIdleTargetAFRMapInBytes()
        {
            string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }

            string filename = Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idletargetafr.afr");
            byte[] map = new byte[12* 8 * 2];
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
                        //  MessageBox.Show("Failed to load target AFR map: " + E.Message);
                        Console.WriteLine(E.Message);

                    }
                }
            }
            else
            {
                map = new byte[12 * 8 * 2];
                map.Initialize();
            }
            return map;

        }

        private byte[] LoadTargetAFRMapInBytes(float[] orginalmap)
        {

            byte[] map = new byte[/*m_TrionicFile.GetFileInfo().GetSymbolLength(m_TrionicFile.GetFileInfo().GetInjectionMap())*/ orginalmap.Length * 2];
            map.Initialize();
            if (m_TrionicFile.Exists())
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
                    Console.WriteLine(E.Message);
                }
            }
            else
            {
                map = new byte[orginalmap.Length * 2];
                map.Initialize();
            }
            return map;
        }

        

        private float[] LoadTargetAFRMap(string filename)
        {
            // load the target AFR map into memory
            float[] map = new float[m_TrionicFile.GetFileInfo().GetSymbolLength(m_TrionicFile.GetFileInfo().GetInjectionMap())];
            map.Initialize();
            if (m_TrionicFile.Exists())
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
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
                        Console.WriteLine(E.Message);

                    }
                }
            }
            else
            {
                map = new float[0x100];
                map.Initialize();
            }
            return map;
        }



        private float[] LoadIdleTargetAFRMap(string filename)
        {
            // load the target AFR map into memory
            float[] map = new float[12*8];
            map.Initialize();
            if (m_TrionicFile.Exists())
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_TrionicFile.GetFileInfo().Filename), "AFRMaps");
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
                        Console.WriteLine(E.Message);

                    }
                }
            }
            else
            {
                map = new float[12*8];
                map.Initialize();
            }
            return map;
        }

        private int[] LoadCounterMap(string filename)
        {
            // load the target AFR map into memory
            int[] map = new int[m_TrionicFile.GetFileInfo().GetSymbolLength(m_TrionicFile.GetFileInfo().GetInjectionMap())];
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
                        //  MessageBox.Show("Failed to load target AFR counter map: " + E.Message);
                        Console.WriteLine(E.Message);
                    }
                }
            }
            else
            {
                map = new int[0x100];
                map.Initialize();

            }
            return map;
        }


        private int[] LoadIdleCounterMap(string filename)
        {
            // load the target AFR map into memory
            int[] map = new int[12*8];
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
                        //  MessageBox.Show("Failed to load target AFR counter map: " + E.Message);
                        Console.WriteLine(E.Message);
                    }
                }
            }
            else
            {
                map = new int[12*8];
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

        public void CheckTargetAFRMap()
        {
            if (targetmap.Length == 0)
            {
                targetmap = CreateDefaultTargetAFRMap();
            }
            else if(IsAllZero(targetmap))
            {
                targetmap = CreateDefaultTargetAFRMap();
            }
            SaveTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetafr.afr", targetmap);

        }

        public void CheckIdleTargetAFRMap()
        {
            if (idletargetmap == null)
            {
                idletargetmap = CreateDefaultIdleTargetAFRMap();
            }
            if (idletargetmap.Length == 0)
            {
                idletargetmap = CreateDefaultIdleTargetAFRMap();
            }
            else if (IsAllZero(idletargetmap))
            {
                idletargetmap = CreateDefaultIdleTargetAFRMap();
            }

            SaveIdleTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-idletargetafr.afr", idletargetmap);

        }

        public void CheckTargetAFRMap(InjectorType injectorType)
        {
            if (targetmap.Length == 0)
            {
                targetmap = CreateDefaultTargetAFRMap(injectorType);
            }
            else if (IsAllZero(targetmap))
            {
                targetmap = CreateDefaultTargetAFRMap(injectorType);
            }
            SaveTargetAFRMap(Path.GetFileNameWithoutExtension(m_TrionicFile.GetFileInfo().Filename) + "-targetafr.afr", targetmap);

        }

        public void ClearAFRFeedbackMapCell(int mapindex)
        {
            try
            {
                if (AFRMapInMemory != null)
                {
                    AFRMapInMemory[mapindex] = 0;
                }
                if (AFRMapCounterInMemory != null)
                {
                    AFRMapCounterInMemory[mapindex] = 0;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        public void ClearIdleAFRFeedbackMapCell(int mapindex)
        {
            try
            {
                if (idleAFRMapInMemory != null)
                {
                    idleAFRMapInMemory[mapindex] = 0;
                }
                if (idleAFRMapCounterInMemory != null)
                {
                    idleAFRMapCounterInMemory[mapindex] = 0;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }
    }
}
