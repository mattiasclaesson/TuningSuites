using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using NLog;

namespace CommonSuite
{
    public class ExcelXLS
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static byte[] TurnMapUpsideDown(byte[] mapdata, int numcolumns, int numrows, bool issixteenbit)
        {
            byte[] mapdatanew = new byte[mapdata.Length];
            if (issixteenbit) numcolumns *= 2;
            int internal_rows = mapdata.Length / numcolumns;
            for (int tel = 0; tel < internal_rows; tel++)
            {
                for (int ctel = 0; ctel < numcolumns; ctel++)
                {
                    int orgoffset = (((internal_rows - 1) - tel) * numcolumns) + ctel;
                    mapdatanew.SetValue(mapdata.GetValue(orgoffset), (tel * numcolumns) + ctel);
                }
            }
            return mapdatanew;
        }

        public static double ConvertSignedValue(int values)
        {
            byte val1 = (byte)(values >> 8 & 0xff);
            byte val2 = (byte)(values & 0xff);
            return ConvertSignedValue(val1, val2);
        }

        public static double ConvertSignedValue(byte val1, byte val2)
        {
            bool convertSign = false;
            if (val1 == 0xff)
            {
                val1 = 0;
                val2 = (byte)(0x100 - val2);
                convertSign = true;
            }
            int ival1 = Convert.ToInt32(val1);
            int ival2 = Convert.ToInt32(val2);
            double value = (ival1 * 256) + ival2;
            if (convertSign)
            {
                value = -value;
            }
            return value;
        }

        public static double[,] AddData(int nRows, int nColumns, byte[] mapdata, bool isSixteenbit, double factor, double offset)
        {
            double[,] dataArray = new double[nRows, nColumns];
            double[] xarray = new double[nColumns];
            for (int i = 0; i < xarray.Length; i++)
            {
                xarray[i] = -3.0f + i * 0.25f;
            }
            double[] yarray = xarray;

            int mapindex = 0;
            for (int i = 0; i < dataArray.GetLength(0); i++)
            {
                for (int j = 0; j < dataArray.GetLength(1); j++)
                {
                    if (isSixteenbit)
                    {
                        byte val1 = (byte)mapdata.GetValue(mapindex++);
                        byte val2 = (byte)mapdata.GetValue(mapindex++);

                        double value = ConvertSignedValue(val1, val2);
                        value *= factor;
                        value += offset;

                        dataArray[i, j] = Math.Round(value, 2);
                    }
                    else
                    {
                        byte val1 = (byte)mapdata.GetValue(mapindex++);
                        int ival1 = Convert.ToInt32(val1);

                        double value = ival1;
                        value *= factor;
                        value += offset;

                        dataArray[i, j] = Math.Round(value, 2);
                    }
                }
            }
            return dataArray;
        }

        public static System.Data.DataTable getDataFromXLS(string strFilePath)
        {
            try
            {
                string strConnectionString = string.Empty;
                strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFilePath + @";Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";
                OleDbConnection cnCSV = new OleDbConnection(strConnectionString);
                cnCSV.Open();
                OleDbCommand cmdSelect = new OleDbCommand(@"SELECT * FROM [symboldata$]", cnCSV);
                OleDbDataAdapter daCSV = new OleDbDataAdapter();
                daCSV.SelectCommand = cmdSelect;
                System.Data.DataTable dtCSV = new System.Data.DataTable();
                daCSV.Fill(dtCSV);
                cnCSV.Close();
                daCSV = null;
                return dtCSV;
            }
            catch (Exception ex)
            {
                frmInfoBox info = new frmInfoBox("Error failed OleDbConnection");
                logger.Debug(ex, "Error failed OldDbConnection");
                return null;
            }
            finally { }
        }
    }
}
