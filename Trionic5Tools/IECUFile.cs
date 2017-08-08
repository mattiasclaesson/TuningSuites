using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Trionic5Tools
{
    /// <summary>
    /// ECUFileType 
    /// </summary>
    public enum ECUFileType
    {
        Trionic52File,
        Trionic55File,
        Trionic7File,
        Trionic8File,
        UnknownFile
    }
   
    /// <summary>
    /// </summary>
    abstract public class IECUFile
    {
        /// <summary>
        /// </summary>
        /// 
        public abstract string LibraryPath
        {
            get;
            set;
        }
        public delegate void DecodeProgress(object sender, DecodeProgressEventArgs e);
        abstract public event DecodeProgress onDecodeProgress;

        public delegate void TransactionLogChanged(object sender, TransactionsEventArgs e);
        abstract public event TransactionLogChanged onTransactionLogChanged;

        abstract public int GetMaxInjection();

        abstract public void SetHardcodedRPMLimit(string filename, int rpmlimit);
        abstract public int GetHardcodedRPMLimit(string filename);
        abstract public int GetHardcodedRPMLimitTwo(string filename);

        abstract public MapSensorType DetermineMapSensorType();
        abstract public TuningStage DetermineTuningStage(out float max_boostRequest);
        abstract public byte[] ReadData(uint offset, uint length);
        abstract public int[] GetSymbolAsIntArray(string symbolname);
        abstract public int GetSymbolAsInt(string symbolname);
        abstract public int[] GetXaxisValues(string filename, string symbolname);
        abstract public int[] GetYaxisValues(string filename, string symbolname);

        public abstract int[] Temp_steg_array
        {
            get;
            set;
        }
        public abstract int[] Kyltemp_steg_array
        {
            get;
            set;
        }
        public abstract int[] Kyltemp_tab_array
        {
            get;
            set;
        }
        public abstract int[] Lamb_kyl_array
        {
            get;
            set;
        }
        public abstract int[] Lufttemp_steg_array
        {
            get;
            set;
        }
        public abstract int[] Lufttemp_tab_array
        {
            get;
            set;
        }
        public abstract int[] Luft_kompfak_array
        {
            get;
            set;
        }
        /*Temp_steg = GetSymbolAsIntArray("Temp_steg!");
                Kyltemp_steg = GetSymbolAsIntArray("Kyltemp_steg!");
                Kyltemp_tab = GetSymbolAsIntArray("Kyltemp_tab!");
                Lamb_kyl = GetSymbolAsIntArray("Lamb_kyl!");
                Lufttemp_steg = GetSymbolAsIntArray("Lufttemp_steg!");
                Lufttemp_tab = GetSymbolAsIntArray("Lufttemp_tab!");
                Luft_kompfak = GetSymbolAsIntArray("Luft_kompfak!");*/

        abstract public int GetRegulationDivisorValue();
        abstract public void SetRegulationDivisorValue(int rpm);
        abstract public int GetManualRpmLow();

        abstract public DataTable CheckForAnomalies();

        abstract public void SetTransactionLog(TrionicTransactionLog transactionLog);

        //abstract public int GetInjectorType();
        //abstract public void SetInjectorType(InjectorType type);
        abstract public void WriteInjectorTypeMarker(InjectorType injectorType);
        abstract public void WriteTurboTypeMarker(TurboType turboType);
        abstract public void WriteTuningStageMarker(TuningStage tuningStage);
        abstract public int ReadInjectorTypeMarker();
        abstract public int ReadTurboTypeMarker();
        abstract public int ReadTuningStageMarker();
        abstract public long GetStartVectorAddress(string filename, int number);
        abstract public long[] GetVectorAddresses(string filename);
        abstract public int GetManualRpmHigh();
        abstract public int GetAutoRpmLow();
        abstract public int GetAutoRpmHigh();
        abstract public int GetMaxBoostError();
        abstract public void SetManualRpmLow(int rpm);
        abstract public void SetManualRpmHigh(int rpm);
        abstract public void SetAutoRpmLow(int rpm);
        abstract public void SetAutoRpmHigh(int rpm);
        abstract public void SetMaxBoostError(int boosterror);

        abstract public void SetAutoUpdateChecksum(bool autoUpdate);
        abstract public Int64 GetMemorySyncCounter();
        abstract public DateTime GetMemorySyncDate();
        abstract public void SetMemorySyncCounter(Int64 countervalue);
        abstract public void SetMemorySyncDate(DateTime syncdt);


        abstract public byte[] ReadDataFromFile(string filename, uint offset, uint length);

        abstract public bool WriteDataNoLog(byte[] data, uint offset);
        abstract public bool WriteData(byte[] data, uint offset);
        abstract public bool WriteData(byte[] data, uint offset, string note);
        abstract public bool WriteDataNoCounterIncrease(byte[] data, uint offset);

        abstract public bool ValidateChecksum();

        abstract public void UpdateChecksum();

        abstract public bool HasSymbol(string symbolname);

        abstract public bool IsTableSixteenBits(string symbolname);

        abstract public double GetCorrectionFactorForMap(string symbolname);

        abstract public int[] GetMapXaxisValues(string symbolname);

        abstract public void GetMapAxisDescriptions(string symbolname, out string x, out string y, out string z);

        abstract public void GetMapMatrixWitdhByName(string symbolname, out int columns, out int rows);

        abstract public int[] GetMapYaxisValues(string symbolname);

        abstract public double GetOffsetForMap(string symbolname);

        abstract public void SelectFile(string filename);

        abstract public void BackupFile();

        abstract public string GetSoftwareVersion();
        
        abstract public string GetPartnumber();

        abstract public Trionic5FileInformation ParseFile();

        abstract public Trionic5FileInformation GetFileInfo();

        abstract public ECUFileType DetermineFileType();

        abstract public MapSensorType GetMapSensorType(bool autoDetectMapsensorType);

        abstract public bool Exists();

        abstract public Trionic5Properties GetTrionicProperties();

        abstract public void SetTrionicOptions(Trionic5Properties properties);

    }

    public class TransactionsEventArgs : System.EventArgs
    {
        private TransactionEntry _entry;

        public TransactionEntry Entry
        {
            get { return _entry; }
            set { _entry = value; }
        }

        public TransactionsEventArgs(TransactionEntry entry)
        {
            this._entry = entry;
        }
    }

    public class DecodeProgressEventArgs : System.EventArgs
    {
        private int _progress;

        public int Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        public DecodeProgressEventArgs(int progress)
        {
            this._progress = progress;
        }
    }
}

