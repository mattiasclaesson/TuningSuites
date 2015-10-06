/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 04/04/2008
 * Ora: 12.18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Drawing;

namespace LBSoft.IndustrialCtrls.Meters
{
	public class LBMeterThreshold
	{
		#region Properties variables
		private	Color	color = Color.Empty;
        private int tag = 0;

        public int Tag
        {
            get { return tag; }
            set { tag = value; }
        }

		private double	startValue = 0.0;
		private double	endValue = 1.0;
		#endregion
		
		#region Constructor
		public LBMeterThreshold()
		{			
		}
		#endregion
		
		#region Properties
		public Color Color
		{
			set { this.color = value; }
			get { return this.color; }
		}
		
		public double StartValue
		{
			set { this.startValue = value; }
			get { return this.startValue; }
		}
		
		public double EndValue
		{
			set { this.endValue = value; }
			get { return this.endValue; }
		}
		#endregion
		
		#region Public methods
		public bool IsInRange ( double val )
		{
			if ( val > this.EndValue )
				return false;
			
			if ( val < this.StartValue )
				return false;
			
			return true;
		}
		#endregion
	}
		
		
		
	public class LBMeterThresholdCollection : CollectionBase
    {
        #region Properties variables
		private bool _IsReadOnly = false;
		#endregion
		
		#region Constructor
        public LBMeterThresholdCollection()
        {
        }
		#endregion
		
        #region Properties
        public virtual LBMeterThreshold this[int index]
        {
            get { return (LBMeterThreshold)InnerList[index]; }
            set { InnerList[index] = value; }
        }

        public virtual bool IsReadOnly
        {
            get { return _IsReadOnly; }
        }
		#endregion
		
		#region Public methods
		/// <summary>
		/// Add an object to the collection
		/// </summary>
		/// <param name="sector"></param>
		public virtual void Add(LBMeterThreshold sector)
        {
            InnerList.Add(sector);
        }

		/// <summary>
		/// Remove an object from the collection
		/// </summary>
		/// <param name="sector"></param>
		/// <returns></returns>
        public virtual bool Remove(LBMeterThreshold sector) 
        {
            bool result = false;

            //loop through the inner array's indices
            for (int i = 0; i < InnerList.Count; i++)
            {
                //store current index being checked
                LBMeterThreshold obj = (LBMeterThreshold)InnerList[i];

                //compare the values of the objects
                if ( ( obj.StartValue == sector.StartValue ) && 
                    ( obj.EndValue == sector.EndValue ) )
                {
                    //remove item from inner ArrayList at index i
                    InnerList.RemoveAt(i);
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Check if the object is containing in the collection
        /// </summary>
        /// <param name="sector"></param>
        /// <returns></returns>
        public bool Contains(LBMeterThreshold sector)
        {
            //loop through the inner ArrayList
            foreach (LBMeterThreshold obj in InnerList)
            {
               //compare the values of the objects
                if ( ( obj.StartValue == sector.StartValue ) && 
                    ( obj.EndValue == sector.EndValue ) )
                {
                    //if it matches return true
                    return true;
                }
            }
            
            //no match
            return false;
        }
 
        /// <summary>
        /// Copy the collection
        /// </summary>
        /// <param name="LBAnalogMeterSectorArray"></param>
        /// <param name="index"></param>
        public virtual void CopyTo(LBMeterThreshold[] MeterThresholdArray, int index)
        {
            throw new Exception("This Method is not valid for this implementation.");
        }
		#endregion
	}
}
