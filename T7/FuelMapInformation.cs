using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T7
{
    /// <summary>
    /// Holds information about the status of the fuel maps while autotuning
    /// </summary>
    public class FuelMapInformation
    {
        private FuelMap m_OriginalFuelMap = new FuelMap();
        private FuelMap m_AlteredFuelMap = new FuelMap();
        private FuelMap m_ChangesFuelMap = new FuelMap();
        private FuelMap m_DifferenceFuelMap = new FuelMap();
        private double[] m_differenceMapInPercentages = new double[18 * 16];


        /// <summary>
        /// Gets the percentual differences for each cell in the fuelmap after running autotune
        /// </summary>
        /// <returns></returns>
        public double[] GetDifferencesInPercentages()
        {
            // fill the array
            for (int i = 0; i < (18 * 16); i++)
            {
                double diff = Convert.ToInt32(m_AlteredFuelMap.GetByteAtPosition(i)) - Convert.ToInt32(m_OriginalFuelMap.GetByteAtPosition(i));
                double original = Convert.ToInt32(m_OriginalFuelMap.GetByteAtPosition(i));
                m_differenceMapInPercentages[i] = (diff * 100)/ original;
                if (m_differenceMapInPercentages[i] == Double.NaN) m_differenceMapInPercentages[i] = 0;
            }
            return m_differenceMapInPercentages;
        }

        

        /// <summary>
        /// Sets the initial fuel map information. Should be called before starting autotune
        /// </summary>
        /// <param name="map"></param>
        public void SetOriginalFuelMap(byte[] map)
        {
            m_OriginalFuelMap.FuelMapData = map;
            m_AlteredFuelMap.FuelMapData = map;
            m_ChangesFuelMap.Initialize();
            m_DifferenceFuelMap.Initialize();
        }

        
        public byte[] GetOriginalFuelMap()
        {
            return m_OriginalFuelMap.FuelMapData;
        }

        
        /// <summary>
        /// Updates the altered fuelmap and the changes table while running autotune
        /// </summary>
        /// <param name="x_index"></param>
        /// <param name="y_index"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool UpdateFuelMap(int x_index, int y_index, int offset)
        {
            // offset is the REAL new value for the fuelmap
            bool retval = false;

            // first check if the fuelmap has not been changed yet at that point
            int numberOfMeasurements = m_ChangesFuelMap.GetByteAtXY(x_index, y_index);
            if (numberOfMeasurements < 255) // max 255 measurements
            {
                // set the altered fuel map at this position with offset given
                
                byte originalValue = m_OriginalFuelMap.GetByteAtXY(x_index, y_index);
                //Console.WriteLine("Update fuelmap: " + originalValue.ToString("D3") + " to " + offset.ToString("D3"));
                // calculate new average value
                int newAverage = m_AlteredFuelMap.GetByteAtXY(x_index, y_index);
                newAverage *= numberOfMeasurements;
                newAverage += offset;
                newAverage /= (numberOfMeasurements + 1);

                m_AlteredFuelMap.SetByteXY(x_index, y_index, /*Convert.ToByte(offset)*/ Convert.ToByte(newAverage));
                numberOfMeasurements++;
                m_ChangesFuelMap.SetByteXY(x_index, y_index, Convert.ToByte(numberOfMeasurements));

                byte newvalue = Convert.ToByte(/*offset*/ newAverage);
                newvalue -= originalValue;
                m_DifferenceFuelMap.SetByteXY(x_index, y_index, newvalue);
                retval = true;
                
            }

            return retval;
        }


    }
}
