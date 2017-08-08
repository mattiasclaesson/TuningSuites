using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;

namespace Trionic5Controls
{
    public partial class frmMapSensorWizard : DevExpress.XtraEditors.XtraForm
    {
        public frmMapSensorWizard()
        {
            InitializeComponent();
        }

        public void SetMapSensorTypes(MapSensorType fromType, MapSensorType toType)
        {
            memoEdit1.Text += Environment.NewLine;
            string targetMapSensorString = "3 bar mapsensor";
            switch (toType)
            {
                case MapSensorType.MapSensor25:
                    targetMapSensorString = "2.5 bar mapsensor";
                    break;
                case MapSensorType.MapSensor30:
                    targetMapSensorString = "3.0 bar mapsensor";
                    break;
                case MapSensorType.MapSensor35:
                    targetMapSensorString = "3.5 bar mapsensor";
                    break;
                case MapSensorType.MapSensor40:
                    targetMapSensorString = "4.0 bar mapsensor";
                    break;
                case MapSensorType.MapSensor50:
                    targetMapSensorString = "5.0 bar mapsensor";
                    break;
            }
            string sourceMapSensorString = "3 bar mapsensor";
            switch (fromType)
            {
                case MapSensorType.MapSensor25:
                    sourceMapSensorString = "2.5 bar mapsensor";
                    break;
                case MapSensorType.MapSensor30:
                    sourceMapSensorString = "3.0 bar mapsensor";
                    break;
                case MapSensorType.MapSensor35:
                    sourceMapSensorString = "3.5 bar mapsensor";
                    break;
                case MapSensorType.MapSensor40:
                    sourceMapSensorString = "4.0 bar mapsensor";
                    break;
                case MapSensorType.MapSensor50:
                    sourceMapSensorString = "5.0 bar mapsensor";
                    break;
            }
            memoEdit1.Text += "You are converting from a " + sourceMapSensorString + " to a " + targetMapSensorString;
            //All boost related tables will be altered to make sure the correct values are calculated within the ECU based on the new mapsensor type.
        }
    }
}