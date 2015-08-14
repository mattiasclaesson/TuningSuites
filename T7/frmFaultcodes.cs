using System;
using System.Collections.Generic;
using System.Data;
using CommonSuite;
using NLog;

namespace T7
{
    public partial class frmFaultcodes : DevExpress.XtraEditors.XtraForm
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public delegate void onClearDTC(object sender, ClearDTCEventArgs e);
        public event frmFaultcodes.onClearDTC onClearCurrentDTC;

        public frmFaultcodes()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void addFault(string faultcode)
        {
            AddFaultCode(faultcode);
            gridView1.BestFitColumns();
            //listBox1.Items.Add(faultcode);
        }

        private void AddFaultCode(string faultcode)
        {
            if (gridControl1.DataSource == null)
            {
                DataTable dtn = new DataTable();
                dtn.Columns.Add("Code");
                dtn.Columns.Add("Description");
                gridControl1.DataSource = dtn;
            }
            DataTable dt = (DataTable)gridControl1.DataSource;
            bool _found = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Code"] != DBNull.Value)
                {
                    if (dr["Code"].ToString() == faultcode)
                    {
                        _found = true;
                    }
                }
            }
            if (!_found)
            {
                dt.Rows.Add(faultcode, TranslateFaultcode(faultcode));
            }
               
        }

        private string TranslateFaultcode(string faultcode)
        {
            string retval = faultcode;
           // build the list
            switch (faultcode)
            {

                case "P0030":
                    retval = "Ox front sensor preheater control circuit";
                    break;
                case "P0031":
                    retval = "Ox front sensor preheater control circuit low";
                    break;
                case "P0032":
                    retval = "Ox front sensor preheater control circuit high";
                    break;
                case "P0033":
                    retval = "Turbo charger bypass valve control circuit performance problem";
                    break;
                case "P0034":
                    retval = "Turbo charger bypass valve control circuit low input";
                    break;
                case "P0035":
                    retval = "Turbo charger bypass valve control circuit high input";
                    break;
                case "P0036":
                    retval = "Ox rear sensor preheater control circuit";
                    break;
                case "P0037":
                    retval = "Ox rear sensor preheater control circuit low";
                    break;
                case "P0038":
                    retval = "Ox rear sensor preheater control circuit high";
                    break;

                case "P0100":
                    retval = "Mass or Volume Air flow Circuit Malfunction";
                    break;
                case "P0101":
                    retval = "Mass or Volume Air flow Circuit Range/Performance Problem";
                    break;
                case "P0102":
                    retval = "Mass or Volume Air Flow Circuit low Input";
                    break;
                case "P0103":
                    retval = "Mass or Volume Air flow Circuit High Input";
                    break;
                case "P0104":
                    retval = "Mass or Volume Air flow Circuit Intermittent";
                    break;
                case "P0105":
                    retval = "Manifold Absolute Pressure/Barometric Pressure Circuit Malfunction";
                    break;
                case "P0106":
                    retval = "Manifold Absolute Pressure/Barometric Pressure Circuit Range/Performance Problem";
                    break;
                case "P0107":
                    retval = "Manifold Absolute Pressure/Barometric Pressure Circuit Low Input";
                    break;
                case "P0108":
                    retval = "Manifold Absolute Pressure/Barometric Pressure Circuit High Input";
                    break;
                case "P0109":
                    retval = "Manifold Absolute Pressure/Barometric Pressure Circuit Intermittent";
                    break;
                case "P0110":
                    retval = "Intake Air Temperature Circuit Malfunction";
                    break;
                case "P0111":
                    retval = "Intake Air Temperature Circuit Range/Performance Problem";
                    break;
                case "P0112":
                    retval = "Intake Air Temperature Circuit Low Input";
                    break;
                case "P0113":
                    retval = "Intake Air Temperature Circuit High Input";
                    break;
                case "P0114":
                    retval = "Intake Air Temperature Circuit Intermittent";
                    break;
                case "P0115":
                    retval = "Engine Coolant Temperature Circuit Malfunction";
                    break;
                case "P0116":
                    retval = "Engine Coolant Temperature Circuit Range/Performance Problem";
                    break;
                case "P0117":
                    retval = "Engine Coolant Temperature Circuit Low Input";
                    break;
                case "P0118":
                    retval = "Engine Coolant Temperature Circuit High Input";
                    break;
                case "P0119":
                    retval = "Engine Coolant Temperature Circuit Intermittent";
                    break;
                case "P0120":
                    retval = "Throttle Pedal Position Sensor/Switch A Circuit Malfunction";
                    break;
                case "P0121":
                    retval = "Throttle/Pedal Position Sensor/Switch A Circuit Range/Performance Problem";
                    break;
                case "P0122":
                    retval = "Throttle/Pedal Position Sensor/Switch A Circuit Low Input";
                    break;
                case "P0123":
                    retval = "Throttle/Pedal Position Sensor/Switch A Circuit High Input";
                    break;
                case "P0124":
                    retval = "Throttle/Pedal Position Sensor/Switch A Circuit Intermittent";
                    break;
                case "P0125":
                    retval = "Insufficient Coolant Temperature for Closed Loop Fuel Control / Time to closed loop too long";
                    break;
                case "P0126":
                    retval = "Insufficient Coolant Temperature for Stable Operation";
                    break;
                case "P0130":
                    retval = "Ox Sensor Circuit Malfunction (Bank 1 Sensor 1)";
                    break;
                case "P0131":
                    retval = "Ox Sensor Circuit Low Voltage (Bank 1 Sensor 1)";
                    break;
                case "P0132":
                    retval = "Ox Sensor Circuit High Voltage (Bank 1 Sensor 1)";
                    break;
                case "P0133":
                    retval = "Ox Sensor Circuit Slow Response (Bank 1 Sensor 1)";
                    break;
                case "P0134":
                    retval = "Ox Sensor Circuit No Activity Detected (Bank 1 Sensor 1)";
                    break;
                case "P0135":
                    retval = "Ox Sensor Heater Circuit Malfunction (Bank 1 Sensor 1)";
                    break;
                case "P0136":
                    retval = "Ox Sensor Circuit Malfunction (Bank 1 Sensor 2)";
                    break;
                case "P0137":
                    retval = "Ox Sensor Circuit Low Voltage (Bank 1 Sensor 2)";
                    break;
                case "P0138":
                    retval = "Ox Sensor Circuit High Voltage (Bank 1 Sensor 2)";
                    break;
                case "P0139":
                    retval = "Ox Sensor Circuit Slow Response (Bank 1 Sensor 2)";
                    break;
                case "P0140":
                    retval = "Ox Sensor Circuit No Activity Detected (Bank 1 Sensor 2)";
                    break;
                case "P0141":
                    retval = "Ox Sensor Heater Circuit Malfunction (Bank 1 Sensor 2)";
                    break;
                case "P0142":
                    retval = "Ox Sensor Circuit Malfunction (Bank 1 Sensor 3)";
                    break;
                case "P0143":
                    retval = "Ox Sensor Circuit Low Voltage (Bank 1 Sensor 3)";
                    break;
                case "P0144":
                    retval = "Ox Sensor Circuit High Voltage (Bank 1 Sensor 3)";
                    break;
                case "P0145":
                    retval = "Ox Sensor Circuit Slow Response (Bank 1 Sensor 3)";
                    break;
                case "P0146":
                    retval = "Ox Sensor Circuit No Activity Detected (Bank 1 Sensor 3)";
                    break;
                case "P0147":
                    retval = "Ox Sensor Heater Circuit Malfunction (Bank 1 Sensor 3)";
                    break;
                case "P0150":
                    retval = "Ox Sensor Circuit Malfunction (Bank 2 Sensor 1)";
                    break;
                case "P0151":
                    retval = "Ox Sensor Circuit Low Voltage (Bank 2 Sensor 1)";
                    break;
                case "P0152":
                    retval = "Ox Sensor Circuit High Voltage (Bank 2 Sensor 1)";
                    break;
                case "P0153":
                    retval = "Ox Sensor Circuit Slow Response (Bank 2 Sensor 1)";
                    break;
                case "P0154":
                    retval = "Ox Sensor Circuit No Activity Detected (Bank 2 Sensor 1)";
                    break;
                case "P0155":
                    retval = "Ox Sensor Heater Circuit Malfunction (Bank 2 Sensor 1)";
                    break;
                case "P0156":
                    retval = "Ox Sensor Circuit Malfunction (Bank 2 Sensor 2)";
                    break;
                case "P0157":
                    retval = "Ox Sensor Circuit Low Voltage (Bank 2 Sensor 2)";
                    break;
                case "P0158":
                    retval = "Ox Sensor Circuit High Voltage (Bank 2 Sensor 2)";
                    break;
                case "P0159":
                    retval = "Ox Sensor Circuit Slow Response (Bank 2 Sensor 2)";
                    break;
                case "P0160":
                    retval = "Ox Sensor Circuit No Activity Detected (Bank 2 Sensor 2)";
                    break;
                case "P0161":
                    retval = "Ox Sensor Heater Circuit Malfunction (Bank 2 Sensor 2)";
                    break;
                case "P0162":
                    retval = "Ox Sensor Circuit Malfunction (Bank 2 Sensor 3)";
                    break;
                case "P0163":
                    retval = "Ox Sensor Circuit Low Voltage (Bank 2 Sensor 3)";
                    break;
                case "P0164":
                    retval = "Ox Sensor Circuit High Voltage (Bank 2 Sensor 3)";
                    break;
                case "P0165":
                    retval = "Ox Sensor Circuit Slow Response (Bank 2 Sensor 3)";
                    break;
                case "P0166":
                    retval = "Ox Sensor Circuit No Activity Detected (Bank 2 Sensor 3)";
                    break;
                case "P0167":
                    retval = "Ox Sensor Heater Circuit Malfunction (Bank 2 Sensor 3)";
                    break;
                case "P0170":
                    retval = "Fuel Trim Malfunction (Bank 1)";
                    break;

                case "P2187":
                    retval = "System Too Lean on idle";
                    break;
                case "P2188":
                    retval = "System Too Rich on idle";
                    break;
                case "P2195":
                    retval = "Short term fuel trim too lean";
                    break;
                case "P2196":
                    retval = "Short term fuel trim too rich";
                    break;

                case "P0171":
                    retval = "System Too Lean (Bank 1)";
                    break;
                case "P0172":
                    retval = "System Too Rich (Bank 1)";
                    break;
                case "P0173":
                    retval = "Fuel Trim Malfunction (Bank 2)";
                    break;
                case "P0174":
                    retval = "System Too Lean (Bank 2)";
                    break;
                case "P0175":
                    retval = "System Too Rich (Bank 2)";
                    break;
                case "P0176":
                    retval = "Fuel Composition Sensor Circuit Malfunction";
                    break;
                case "P0177":
                    retval = "Fuel Composition Sensor Circuit Range/Performance";
                    break;
                case "P0178":
                    retval = "Fuel Composition Sensor Circuit Low Input";
                    break;
                case "P0179":
                    retval = "Fuel Composition Sensor Circuit High Input";
                    break;
                case "P0180":
                    retval = "Fuel Temperature Sensor A Circuit Malfunction";
                    break;
                case "P0181":
                    retval = "Fuel Temperature Sensor A Circuit Performance";
                    break;
                case "P0182":
                    retval = "Fuel Temperature Sensor A Circuit low Input";
                    break;
                case "P0183":
                    retval = "Fuel Temperature Sensor A Circuit High Input";
                    break;
                case "P0184":
                    retval = "Fuel Temperature Sensor A Circuit Intermittent";
                    break;
                case "P0185":
                    retval = "Fuel Temperature Sensor B Circuit Malfunction";
                    break;
                case "P0186":
                    retval = "Fuel Temperature Sensor B Circuit Range/Performance";
                    break;
                case "P0187":
                    retval = "Fuel Temperature Sensor U Circuit Low Input";
                    break;
                case "P0188":
                    retval = "Fuel Temperature Sensor B Circuit High Input";
                    break;
                case "P0189":
                    retval = "Fuel Temperature Sensor B Circuit Intermittent";
                    break;
                case "P0190":
                    retval = "Fuel Rail Pressure Sensor Circuit Malfunction";
                    break;
                case "P0191":
                    retval = "Fuel Rail Pressure Sensor Circuit Range/Performance";
                    break;
                case "P0192":
                    retval = "Fuel Rail Pressure Sensor Circuit Low Input";
                    break;
                case "P0193":
                    retval = "Fuel Rail Pressure Sensor Circuit High Input";
                    break;
                case "P0194":
                    retval = "Fuel Rail Pressure Sensor Circuit Intermittent";
                    break;
                case "P0195":
                    retval = "Engine Oil Temperature Sensor Malfunction";
                    break;
                case "P0196":
                    retval = "Engine Oil Temperature Sensor Range/Performance";
                    break;
                case "P0197":
                    retval = "Engine Oil Temperature Sensor Low";
                    break;
                case "P0198":
                    retval = "Engine Oil Temperature Sensor High";
                    break;
                case "P0199":
                    retval = "Engine Oil Temperature Sensor Intermittent";
                    break;
                case "P0200":
                    retval = "Injector Circuit Malfunction";
                    break;
                case "P0201":
                    retval = "Injector Circuit Malfunction - Cylinder 1";
                    break;
                case "P0202":
                    retval = "Injector Circuit Malfunction - Cylinder 2";
                    break;
                case "P0203":
                    retval = "Injector Circuit Malfunction - Cylinder 3";
                    break;
                case "P0204":
                    retval = "Injector Circuit Malfunction - Cylinder 4";
                    break;
                case "P0205":
                    retval = "Injector Circuit Malfunction - Cylinder 5";
                    break;
                case "P0206":
                    retval = "Injector Circuit Malfunction - Cylinder 6";
                    break;
                case "P0207":
                    retval = "Injector Circuit Malfunction - Cylinder 7";
                    break;
                case "P0208":
                    retval = "Injector Circuit Malfunction - Cylinder 8";
                    break;
                case "P0209":
                    retval = "Injector Circuit Malfunction - Cylinder 9";
                    break;
                case "P0210":
                    retval = "Injector Circuit Malfunction - Cylinder 10";
                    break;
                case "P0211":
                    retval = "Injector Circuit Malfunction - Cylinder 11";
                    break;
                case "P0212":
                    retval = "Injector Circuit Malfunction - Cylinder 12";
                    break;
                case "P0213":
                    retval = "Cold Start Injector 1 Malfunction";
                    break;
                case "P0214":
                    retval = "Cold Start Injector 2 Malfunction";
                    break;
                case "P0215":
                    retval = "Engine Shutoff Solenoid Malfunction";
                    break;
                case "P0216":
                    retval = "Injection Timing Control Circuit Malfunction";
                    break;
                case "P0217":
                    retval = "Engine Overtemp Condition";
                    break;
                case "P0218":
                    retval = "Transmission Over Temperature Condition";
                    break;
                case "P0219":
                    retval = "Engine Over Speed Condition";
                    break;
                case "P0220":
                    retval = "Throttle/Pedal Position Sensor/Switch B Circuit Malfunction";
                    break;
                case "P0221":
                    retval = "Throttle/Pedal Position Sensor/Switch B Circuit Range/Performance Problem";
                    break;
                case "P0222":
                    retval = "Throttle/Pedal Position Sensor/Switch B Circuit Low Input";
                    break;
                case "P0223":
                    retval = "Throttle/Pedal Position Sensor/Switch B Circuit High Input";
                    break;
                case "P0224":
                    retval = "Throttle/Pedal Position Sensor/Switch B Circuit Intermittent";
                    break;
                case "P0225":
                    retval = "Throttle/Pedal Position Sensor/Switch C Circuit Malfunction";
                    break;
                case "P0226":
                    retval = "Throttle/Pedal Position Sensor/Switch C Circuit Range/Performance Problem";
                    break;
                case "P0227":
                    retval = "Throttle/Pedal Position Sensor/Switch C Circuit Low Input";
                    break;
                case "P0228":
                    retval = "Throttle/Pedal Position Sensor/Switch C Circuit High Input";
                    break;
                case "P0229":
                    retval = "Throttle/Pedal Position Sensor/Switch C Circuit Intermittent";
                    break;
                case "P0230":
                    retval = "Fuel Pump Primary Circuit Malfunction";
                    break;
                case "P0231":
                    retval = "Fuel Pump Secondary Circuit Low";
                    break;
                case "P0232":
                    retval = "Fuel Pump Secondary Circuit High";
                    break;
                case "P0233":
                    retval = "Fuel Pump Secondary Circuit Intermittent";
                    break;
                case "P0234":
                    retval = "Turbocharger Overboost Condition";
                    break;
                case "P0235":
                    retval = "Turbocharger Boost Sensor A Circuit Malfunction";
                    break;
                case "P0236":
                    retval = "Turbocharger Boost Sensor A Circuit Range/Performance";
                    break;
                case "P0237":
                    retval = "Turbocharger Boost Sensor A Circuit Low";
                    break;
                case "P0238":
                    retval = "Turbocharger Boost Sensor A Circuit High";
                    break;
                case "P0239":
                    retval = "Turbocharger Boost Sensor B Circuit Malfunction";
                    break;
                case "P0240":
                    retval = "Turbocharger Boost Sensor B Circuit Range/Performance";
                    break;
                case "P0241":
                    retval = "Turbocharger Boost Sensor B Circuit Low";
                    break;
                case "P0242":
                    retval = "Turbocharger Boost Sensor B Circuit High";
                    break;
                case "P0243":
                    retval = "Turbocharger Wastegate Solenoid A Malfunction";
                    break;
                case "P0244":
                    retval = "Turbocharger Wastegate Solenoid A Range/Performance";
                    break;
                case "P0245":
                    retval = "Turbocharger Wastegate Solenoid A low";
                    break;
                case "P0246":
                    retval = "Turbocharger Wastegate Solenoid A High";
                    break;
                case "P0247":
                    retval = "Turbocharger Wastegate Solenoid B Malfunction";
                    break;
                case "P0248":
                    retval = "Turbocharger Wastegate Solenoid B Range/Performance";
                    break;
                case "P0249":
                    retval = "Turbocharger Wastegate Solenoid B Low";
                    break;
                case "P0250":
                    retval = "Turbocharger Wastegate Solenoid B High";
                    break;
                case "P0251":
                    retval = "Injection Pump Fuel Metering Control A Malfunction (Cam/Rotor/Injector)";
                    break;
                case "P0252":
                    retval = "Injection Pump Fuel Metering Control A Range/Performance (Cam/Rotor/Injector)";
                    break;
                case "P0253":
                    retval = "Injection Pump Fuel Metering Control A Low (Cam/Rotor/Injector)";
                    break;
                case "P0254":
                    retval = "Injection Pump Fuel Metering Control A High (Cam/Rotor/Injector)";
                    break;
                case "P0255":
                    retval = "Injection Pump Fuel Metering Control A Intermittent (Cam/Rotor/Injector)";
                    break;
                case "P0256":
                    retval = "Injection Pump Fuel Metering Control B Malfunction (Cam/Rotor/Injector)";
                    break;
                case "P0257":
                    retval = "Injection Pump Fuel Metering Control B Range/Performance (Cam/Rotor/Injector)";
                    break;
                case "P0258":
                    retval = "Injection Pump Fuel Metering Control B Low (Cam/Rotor/Injector)";
                    break;
                case "P0259":
                    retval = "Injection lump Fuel Metering Control B High (Cam/Rotor/Injector)";
                    break;
                case "P0260":
                    retval = "Injection Pump Fuel Metering Control B Intermittent (Cam/Rotor/Injector)";
                    break;
                case "P0261":
                    retval = "Cylinder 1 Injector Circuit Low";
                    break;
                case "P0262":
                    retval = "Cylinder 1 Injector Circuit High";
                    break;
                case "P0263":
                    retval = "Cylinder 1 Contribution/Balance Fault";
                    break;
                case "P0264":
                    retval = "Cylinder 2 Injector Circuit Low";
                    break;
                case "P0265":
                    retval = "Cylinder 2 Injector Circuit High";
                    break;
                case "P0266":
                    retval = "Cylinder 2 Contribution/Balance Fault";
                    break;
                case "P0267":
                    retval = "Cylinder 3 Injector Circuit Low";
                    break;
                case "P0268":
                    retval = "Cylinder 3 Injector Circuit High";
                    break;
                case "P0269":
                    retval = "Cylinder 3 Contribution/Balance Fault";
                    break;
                case "P0270":
                    retval = "Cylinder 4 Injector Circuit Low";
                    break;
                case "P0271":
                    retval = "Cylinder 4 Injector Circuit High";
                    break;
                case "P0272":
                    retval = "Cylinder 4 Contribution/Balance Fault";
                    break;
                case "P0273":
                    retval = "Cylinder 5 Injector Circuit Low";
                    break;
                case "P0274":
                    retval = "Cylinder 5 Injector Circuit High";
                    break;
                case "P0275":
                    retval = "Cylinder 5 Contribution/Balance Fault";
                    break;
                case "P0276":
                    retval = "Cylinder 6 Injector Circuit Low";
                    break;
                case "P0277":
                    retval = "Cylinder 6 Injector Circuit High";
                    break;
                case "P0278":
                    retval = "Cylinder 6 Contribution/Balance Fault";
                    break;
                case "P0279":
                    retval = "Cylinder 7 Injector Circuit Low";
                    break;
                case "P0280":
                    retval = "Cylinder 7 Injector Circuit High";
                    break;
                case "P0281":
                    retval = "Cylinder 7 Contribution/Balance Fault";
                    break;
                case "P0282":
                    retval = "Cylinder 8 Injector Circuit Low";
                    break;
                case "P0283":
                    retval = "Cylinder 8 Injector Circuit High";
                    break;
                case "P0284":
                    retval = "Cylinder 8 Contribution/Balance Fault";
                    break;
                case "P0285":
                    retval = "Cylinder 9 Injector Circuit Low";
                    break;
                case "P0286":
                    retval = "Cylinder 9 Injector Circuit High";
                    break;
                case "P0287":
                    retval = "Cylinder 9 Contribution/Balance Fault";
                    break;
                case "P0288":
                    retval = "Cylinder 10 Injector Circuit Low";
                    break;
                case "P0289":
                    retval = "Cylinder 10 Injector Circuit High";
                    break;
                case "P0290":
                    retval = "Cylinder 10 Contribution/balance Fault";
                    break;
                case "P0291":
                    retval = "Cylinder 11 Injector Circuit Low";
                    break;
                case "P0292":
                    retval = "Cylinder 11 Injector Circuit High";
                    break;
                case "P0293":
                    retval = "Cylinder 11 Contribution/balance Fault";
                    break;
                case "P0294":
                    retval = "Cylinder 12 Injector Circuit Low";
                    break;
                case "P0295":
                    retval = "Cylinder 12 Injector Circuit High";
                    break;
                case "P0296":
                    retval = "Cylinder 12 Contribution/Balance Fault";
                    break;
                case "P0298":
                    retval = "Engine Oil Over Temperature";
                    break;
                case "P0300":
                    retval = "Random/Multiple Cylinder Misfire Detected";
                    break;
                case "P0301":
                    retval = "Cylinder 1 Misfire Detected";
                    break;
                case "P0302":
                    retval = "Cylinder 2 Misfire Detected";
                    break;
                case "P0303":
                    retval = "Cylinder 3 Misfire Detected";
                    break;
                case "P0304":
                    retval = "Cylinder 4 Misfire Detected";
                    break;
                case "P0305":
                    retval = "Cylinder 5 Misfire Detected";
                    break;
                case "P0306":
                    retval = "Cylinder 6 Misfire Detected";
                    break;
                case "P0307":
                    retval = "Cylinder 7 Misfire Detected";
                    break;
                case "P0308":
                    retval = "Cylinder 8 Misfire Detected";
                    break;
                case "P0309":
                    retval = "Cylinder 9 Misfire Detected";
                    break;
                case "P0310":
                    retval = "Cylinder 10 Misfire Detected";
                    break;
                case "P0311":
                    retval = "Cylinder 11 Misfire Detected";
                    break;
                case "P0312":
                    retval = "Cylinder 12 Misfire Detected";
                    break;
                case "P0313":
                    retval = "Misfire Detected with Low Fuel";
                    break;
                case "P0314":
                    retval = "Single Cylinder Misfire (Cylinder not Specified)";
                    break;
                case "P0320":
                    retval = "Ignition/Distributor Engine Speed Input Circuit Malfunction";
                    break;
                case "P0321":
                    retval = "Ignition/Distributor Engine Speed Input Circuit Range/Performance";
                    break;
                case "P0322":
                    retval = "Ignition/Distributor Engine Speed Input Circuit No Signal";
                    break;
                case "P0323":
                    retval = "Ignition/Distributor Engine Speed Input Circuit Intermittent";
                    break;
                case "P0325":
                    retval = "Knock Sensor 1 Circuit Malfunction (Bank 1 or Single Sensor) / Knock circuit no activity";
                    break;
                case "P0326":
                    retval = "Knock Sensor 1 Circuit Range/Performance (Bank 1 or Single Sensor)";
                    break;
                case "P0327":
                    retval = "Knock Sensor 1 Circuit low Input (Bank 1 or Single Sensor)";
                    break;
                case "P0328":
                    retval = "Knock Sensor 1 Circuit High Input (Bank 1 or Single Sensor)";
                    break;
                case "P0329":
                    retval = "Knock Sensor 1 Circuit Input Intermittent (Bank 1 or Single Sensor)";
                    break;
                case "P0330":
                    retval = "Knock Sensor 2 Circuit Malfunction (Bank 2)";
                    break;
                case "P0331":
                    retval = "Knock Sensor 2 Circuit Range/Performance (Bank 2)";
                    break;
                case "P0332":
                    retval = "Knock Sensor 2 Circuit Low Input (Bank 2)";
                    break;
                case "P0333":
                    retval = "Knock Sensor 2 Circuit High Input (Bank 2)";
                    break;
                case "P0334":
                    retval = "Knock Sensor 2 Circuit Input Intermittent (Bank 2)";
                    break;
                case "P0335":
                    retval = "Crankshaft Position Sensor A Circuit Malfunction";
                    break;
                case "P0336":
                    retval = "Crankshaft Position Sensor A Circuit Range/Performance";
                    break;
                case "P0337":
                    retval = "Crankshaft Position Sensor A Circuit Low Input";
                    break;
                case "P0338":
                    retval = "Crankshaft Position Sensor A Circuit High Input";
                    break;
                case "P0339":
                    retval = "Crankshaft Position Sensor A Circuit Intermittent";
                    break;
                case "P0340":
                    retval = "Camshaft Position Sensor Circuit Malfunction / synchronize error high voltage";
                    break;
                case "P1340":
                    retval = "Camshaft Position Sensor Circuit Malfunction / synchronize error low voltage";
                    break;
                case "P1341":
                    retval = "Combustion detection low cylinder 1";
                    break;
                case "P1342":
                    retval = "Combustion detection low cylinder 2";
                    break;
                case "P1343":
                    retval = "Combustion detection low cylinder 3";
                    break;
                case "P1344":
                    retval = "Combustion detection low cylinder 4";
                    break;
                case "P1312":
                    retval = "Combustion detection high";
                    break;
                case "P1315":
                    retval = "Ion detect module";
                    break;
                case "P2300":
                    retval = "Ignition coil trigger 1 circuit low";
                    break;
                case "P2301":
                    retval = "Ignition coil trigger 1 circuit high";
                    break;
                case "P2303":
                    retval = "Ignition coil trigger 2 circuit low";
                    break;
                case "P2304":
                    retval = "Ignition coil trigger 2 circuit high";
                    break;
                case "P2306":
                    retval = "Ignition coil trigger 3 circuit low";
                    break;
                case "P2307":
                    retval = "Ignition coil trigger 3 circuit high";
                    break;
                case "P2309":
                    retval = "Ignition coil trigger 4 circuit low";
                    break;
                case "P2310":
                    retval = "Ignition coil trigger 4 circuit high";
                    break;

                case "P0341":
                    retval = "Camshaft Position Sensor Circuit Range/Performance";
                    break;
                case "P0342":
                    retval = "Camshaft Position Sensor Circuit Low Input";
                    break;
                case "P0343":
                    retval = "Camshaft Position Sensor Circuit High Input";
                    break;
                case "P0344":
                    retval = "Camshaft Position Sensor Circuit Intermittent";
                    break;
                case "P0350":
                    retval = "Ignition Coil Primary/Secondary Circuit Malfunction";
                    break;
                case "P0351":
                    retval = "Ignition Coil A Primary/Secondary Circuit Malfunction";
                    break;
                case "P0352":
                    retval = "Ignition Coil B Primary/Secondary Circuit Malfunction";
                    break;
                case "P0353":
                    retval = "Ignition Coil C Primary/Secondary Circuit Malfunction";
                    break;
                case "P0354":
                    retval = "Ignition Coil D Primary/Secondary Circuit Malfunction";
                    break;
                case "P0355":
                    retval = "Ignition Coil E Primary/Secondary Circuit Malfunction";
                    break;
                case "P0356":
                    retval = "Ignition Coil F Primary/Secondary Circuit Malfunction";
                    break;
                case "P0357":
                    retval = "Ignition Coil G Primary/Secondary Circuit Malfunction";
                    break;
                case "P0358":
                    retval = "Ignition Coil H Primary/Secondary Circuit Malfunction";
                    break;
                case "P0359":
                    retval = "Ignition Coil I Primary/Secondary Circuit Malfunction";
                    break;
                case "P0360":
                    retval = "Ignition Coil J Primary/Secondary Circuit Malfunction";
                    break;
                case "P0361":
                    retval = "Ignition Coil K Primary/Secondary Circuit Malfunction";
                    break;
                case "P0362":
                    retval = "Ignition Coil L Primary/Secondary Circuit Malfunction";
                    break;
                case "P0370":
                    retval = "Timing Reference High Resolution Signal A Malfunction";
                    break;
                case "P0371":
                    retval = "Timing Reference High Resolution Signal A Too Many Pulses";
                    break;
                case "P0372":
                    retval = "Timing Reference High Resolution Signal A Too Few Pulses";
                    break;
                case "P0373":
                    retval = "Timing Reference High Resolution Signal A Intermittent/Erratic Pulses";
                    break;
                case "P0374":
                    retval = "Timing Reference High Resolution Signal A No Pulses";
                    break;
                case "P0375":
                    retval = "Timing Reference High Resolution Signal B Malfunction";
                    break;
                case "P0376":
                    retval = "Timing Reference High Resolution Signal B Too Many Pulses";
                    break;
                case "P0377":
                    retval = "Timing Reference High Resolution Signal B Too Few Pulses";
                    break;
                case "P0378":
                    retval = "Timing Reference High Resolution Signal B Intermittent/Erratic Pulses";
                    break;
                case "P0379":
                    retval = "Timing Reference High Resolution Signal B No Pulses";
                    break;
                case "P0380":
                    retval = "Glow Plug/Heater Circuit A Malfunction";
                    break;
                case "P0381":
                    retval = "Glow Plug/Heater Indicator Circuit Malfunction";
                    break;
                case "P0382":
                    retval = "Glow Plug/Heater Circuit B Malfunction";
                    break;
                case "P0385":
                    retval = "Crankshaft Position Sensor B Circuit Malfunction";
                    break;
                case "P0386":
                    retval = "Crankshaft Position Sensor B Circuit Range/Performance";
                    break;
                case "P0387":
                    retval = "Crankshaft Position Sensor B Circuit Low Input";
                    break;
                case "P0388":
                    retval = "Crankshaft Position Sensor B Circuit High Input";
                    break;
                case "P0389":
                    retval = "Crankshaft Position Sensor B Circuit Intermittent";
                    break;
                case "P0400":
                    retval = "Exhaust Gas Recirculation Flow Malfunction";
                    break;
                case "P0401":
                    retval = "Exhaust Gas Recirculation Flow Insufficient Detected";
                    break;
                case "P0402":
                    retval = "Exhaust Gas Recirculation Flow Excessive Detected";
                    break;
                case "P0403":
                    retval = "Exhaust Gas Recirculation Circuit Malfunction";
                    break;
                case "P0404":
                    retval = "Exhaust Gas Recirculation Circuit Range/Performance";
                    break;
                case "P0405":
                    retval = "Exhaust Gas Recirculation Sensor A Circuit Low";
                    break;
                case "P0406":
                    retval = "Exhaust Gas Recirculation Sensor A Circuit High";
                    break;
                case "P0407":
                    retval = "Exhaust Gas Recirculation Sensor B Circuit Low";
                    break;
                case "P0408":
                    retval = "Exhaust Gas Recirculation Sensor B Circuit High";
                    break;
                case "P0410":
                    retval = "Secondary Air Injection System Malfunction";
                    break;
                case "P0411":
                    retval = "Secondary Air Injection System Incorrect Flow Detected";
                    break;
                case "P0412":
                    retval = "Secondary Air Injection System Switching Valve A Circuit Malfunction";
                    break;
                case "P0413":
                    retval = "Secondary Air Injection System Switching Valve A Circuit Open";
                    break;
                case "P0414":
                    retval = "Secondary Air Injection System Switching Valve A Circuit Shorted";
                    break;
                case "P0415":
                    retval = "Secondary Air Injection System Switching Valve B Circuit Malfunction";
                    break;
                case "P0416":
                    retval = "Secondary Air Injection System Switching Valve B Circuit Open";
                    break;
                case "P0417":
                    retval = "Secondary Air Injection System Switching Valve B Circuit Shorted";
                    break;
                case "P0418":
                    retval = "Secondary Air Injection System Relay A circuit Malfunction";
                    break;
                case "P0419":
                    retval = "Secondary Air Injection System Relay B” Circuit Malfunction";
                    break;
                case "P0420":
                    retval = "Catalyst System Efficiency Below Threshold (Bank 1)";
                    break;
                case "P0421":
                    retval = "Warm Up Catalyst Efficiency Below Threshold (Bank 1)";
                    break;
                case "P0422":
                    retval = "Main Catalyst Efficiency Below Threshold (Bank 1)";
                    break;
                case "P0423":
                    retval = "Heated Catalyst Efficiency Below Threshold (Bank l)";
                    break;
                case "P0424":
                    retval = "Heated Catalyst Temperature Below Threshold (Bank 1)";
                    break;
                case "P0430":
                    retval = "Catalyst System Efficiency Below Threshold (Bank 2)";
                    break;
                case "P0431":
                    retval = "Warm Up Catalyst Efficiency Below Threshold (Bank 2)";
                    break;
                case "P0432":
                    retval = "Main Catalyst Efficiency Below Threshold (Bank 2)";
                    break;
                case "P0433":
                    retval = "Heated Catalyst Efficiency Below Threshold (Bank 2)";
                    break;
                case "P0434":
                    retval = "Heated Catalyst Temperature Below Threshold (Bank 2)";
                    break;
                case "P0440":
                    retval = "Evaporative Emission Control System Malfunction";
                    break;
                case "P0441":
                    retval = "Evaporative Emission Control System Incorrect Purge flow";
                    break;
                case "P0442":
                    retval = "Evaporative Emission Control System leak Detected (small leak)";
                    break;
                case "P1442":
                    retval = "Evaporative Emission Control System leak Detected (small leak, fuel)";
                    break;
                case "P0443":
                    retval = "Evaporative Emission Control System Purge Control Valve circuit Malfunction";
                    break;
                case "P0444":
                    retval = "Evaporative Emission Control System Purge Control Valve Circuit Open";
                    break;
                case "P0445":
                    retval = "Evaporative Emission Control System Purge Control Valve Circuit Shorted";
                    break;
                case "P0446":
                    retval = "Evaporative Emission Control System Vent Control Circuit Malfunction";
                    break;
                case "P0447":
                    retval = "Evaporative Emission Control System Vent Control Circuit Open";
                    break;
                case "P0448":
                    retval = "Evaporative Emission Control System Vent Control Circuit Shorted";
                    break;
                case "P0449":
                    retval = "Evaporative Emission Control System Vent Valve/Solenoid Circuit Malfunction";
                    break;
                case "P0450":
                    retval = "Evaporative Emission Control System Pressure Sensor Malfunction";
                    break;
                case "P0451":
                    retval = "Evaporative Emission Control System Pressure Sensor Range/Performance";
                    break;
                case "P0452":
                    retval = "Evaporative Emission Control System Pressure Sensor Low Input";
                    break;
                case "P0453":
                    retval = "Evaporative Emission Control System Pressure Sensor High Input";
                    break;
                case "P0454":
                    retval = "Evaporative Emission Control System Pressure Sensor Intermittent";
                    break;
                case "P0455":
                    retval = "Evaporative Emission Control System Tank Detected (gross leak)";
                    break;
                case "P1455":
                    retval = "Evaporative Emission Control System Tank Detected (gross leak, fuel)";
                    break;
                case "P0456":
                    retval = "Evaporative Emission Control System Tank Detected (very small leak)";
                    break;
                case "P1456":
                    retval = "Evaporative Emission Control System Tank Detected (very small leak, fuel)";
                    break;
                case "P0460":
                    retval = "Fuel Level Sensor Circuit Malfunction";
                    break;
                case "P0461":
                    retval = "Fuel Level Sensor Circuit Range/Performance";
                    break;
                case "P0462":
                    retval = "Fuel Level Sensor Circuit Low Input";
                    break;
                case "P0463":
                    retval = "Fuel Level Sensor Circuit High Input";
                    break;
                case "P0464":
                    retval = "Fuel Level Sensor Circuit Intermittent";
                    break;
                case "P0465":
                    retval = "Purge Flow Sensor Circuit Malfunction";
                    break;
                case "P0466":
                    retval = "Purge Flow Sensor Circuit Range/Performance";
                    break;
                case "P0467":
                    retval = "Purge Flow Sensor Circuit Low Input";
                    break;
                case "P0468":
                    retval = "Purge Flow Sensor Circuit High Input";
                    break;
                case "P0469":
                    retval = "Purge Flow Sensor Circuit Intermittent";
                    break;
                case "P0470":
                    retval = "Exhaust Pressure Sensor Malfunction";
                    break;
                case "P0471":
                    retval = "Exhaust Pressure Sensor Range/Performance";
                    break;
                case "P0472":
                    retval = "Exhaust Pressure Sensor Low";
                    break;
                case "P0473":
                    retval = "Exhaust Pressure Sensor High";
                    break;
                case "P0474":
                    retval = "Exhaust Pressure Sensor Intermittent";
                    break;
                case "P0475":
                    retval = "Exhaust Pressure Control Valve Malfunction";
                    break;
                case "P0476":
                    retval = "Exhaust Pressure Control Valve Range/Performance";
                    break;
                case "P0477":
                    retval = "Exhaust Pressure Control Valve Low";
                    break;
                case "P0478":
                    retval = "Exhaust Pressure Control Valve High";
                    break;
                case "P0479":
                    retval = "Exhaust Pressure Control Valve Intermittent";
                    break;
                case "P0480":
                    retval = "Cooling Fan 1 Control Circuit Malfunction";
                    break;
                case "P0481":
                    retval = "Cooling Fan 2 Control Circuit Malfunction";
                    break;
                case "P0482":
                    retval = "Cooling Fan 3 Control Circuit Malfunction";
                    break;
                case "P0483":
                    retval = "Cooling Fan Rationality Check Malfunction";
                    break;
                case "P0484":
                    retval = "Cooling Fan Circuit Over Current";
                    break;
                case "P0485":
                    retval = "Cooling Fan Power/Ground Circuit Malfunction";
                    break;
                case "P0486":
                    retval = "Exhaust Gas Recirculation Sensor B Circuit";
                    break;
                case "P0487":
                    retval = "Exhaust Gas Recirculation Throttle Position Control Circuit";
                    break;
                case "P0488":
                    retval = "Exhaust Gas Recirculation Throttle Position Control Range/Performance";
                    break;
                case "P0491":
                    retval = "Secondary Air Injection System (Bank 1)";
                    break;
                case "P0492":
                    retval = "Secondary Air Injection System (Bank 2)";
                    break;
                case "P0498":
                    retval = "CMD purge diagnostics low limit";
                    break;
                case "P0499":
                    retval = "CMD purge diagnostics high limit";
                    break;
                case "P0500":
                    retval = "Vehicle Speed Sensor Malfunction";
                    break;
                case "P0501":
                    retval = "Vehicle Speed Sensor Range/Performance";
                    break;
                case "P0502":
                    retval = "Vehicle Speed Sensor Circuit Low Input";
                    break;
                case "P0503":
                    retval = "Vehicle Speed Sensor Intermittent/Erratic/High";
                    break;
                case "P0505":
                    retval = "Idle Control System Malfunction";
                    break;
                case "P0506":
                    retval = "Idle Control System RPM lower Than Expected";
                    break;
                case "P0507":
                    retval = "Idle Control System RPM higher Than Expected";
                    break;
                case "P0510":
                    retval = "Closed Throttle Position Switch Malfunction";
                    break;
                case "P0520":
                    retval = "Engine Oil Pressure Sensor/Switch Circuit Malfunction";
                    break;
                case "P0521":
                    retval = "Engine Oil Pressure Sensor/Switch Range/Performance";
                    break;
                case "P0522":
                    retval = "Engine Oil Pressure Sensor/Switch Low Voltage";
                    break;
                case "P0523":
                    retval = "Engine Oil Pressure Sensor/Switch High Voltage";
                    break;
                case "P0530":
                    retval = "A/C Refrigerant Pressure Sensor Circuit Malfunction";
                    break;
                case "P0531":
                    retval = "A/C Refrigerant Pressure Sensor Circuit Range/Performance";
                    break;
                case "P0532":
                    retval = "A/C Refrigerant Pressure Sensor Circuit Low Input";
                    break;
                case "P0533":
                    retval = "A/C Refrigerant pressure Sensor Circuit High Input";
                    break;
                case "P0534":
                    retval = "Air Conditioner Refrigerant Charge Loss";
                    break;
                case "P0545":
                    retval = "Exhaust gas temperature sensor circuit low";
                    break;
                case "P0546":
                    retval = "Exhaust gas temperature sensor circuit high";
                    break;
                case "P0544":
                    retval = "Exhaust gas temperature sensor circuit performance problem";
                    break;
                case "P0550":
                    retval = "Power Steering Pressure Sensor Circuit Malfunction";
                    break;
                case "P0551":
                    retval = "Power Steering Pressure Sensor Circuit Range/Performance";
                    break;
                case "P0552":
                    retval = "Power Steering Pressure Sensor Circuit Low Input";
                    break;
                case "P0553":
                    retval = "Power Steering Pressure Sensor Circuit High Input";
                    break;
                case "P0554":
                    retval = "Power Steering Pressure sensor Circuit Intermittent";
                    break;
                case "P0560":
                    retval = "System Voltage Malfunction";
                    break;
                case "P0561":
                    retval = "System Voltage Unstable";
                    break;
                case "P0562":
                    retval = "System Voltage Low";
                    break;
                case "P0563":
                    retval = "System Voltage High";
                    break;
                case "P0565":
                    retval = "Cruise Control On Signal Malfunction";
                    break;
                case "P0566":
                    retval = "Cruise Control Off Signal Malfunction";
                    break;
                case "P0567":
                    retval = "Cruise Control Resume Signal Malfunction";
                    break;
                case "P0568":
                    retval = "Cruise Control Set Signal Malfunction";
                    break;
                case "P0569":
                    retval = "Cruise Control Coast Signal Malfunction";
                    break;
                case "P0570":
                    retval = "Cruise Control Accel Signal Malfunction";
                    break;
                case "P0571":
                    retval = "Cruise Control/Brake Switch A Circuit Malfunction";
                    break;
                case "P0572":
                    retval = "Cruise Control/Brake Switch A Circuit Low";
                    break;
                case "P0573":
                    retval = "Cruise Control/Brake Switch A Circuit High";
                    break;
                case "P0574":
                    retval = "Cruise Control System - Vehicle Speed Too High";
                    break;
                case "P0575":
                    retval = "Cruise Control Input Circuit";
                    break;
                case "P0576":
                    retval = "Cruise Control Input Circuit Low";
                    break;
                case "P0577":
                    retval = "Cruise Control Input Circuit High";
                    break;
                case "P0578":
                    retval = "through P0580 Reserved for Cruise Control Codes";
                    break;
                case "P0600":
                    retval = "Serial Communication Link Malfunction (high speed CAN bus performance problem)";
                    break;
                case "P0601":
                    retval = "Internal Control Module Memory Check Sum Error";
                    break;
                case "P0602":
                    retval = "Control Module Programming Error";
                    break;
                case "P0603":
                    retval = "Internal Control Module Keep Alive Memory (KAM) Error";
                    break;
                case "P0604":
                    retval = "Internal Control Module Random Access Memory (RAM) Error";
                    break;
                case "P0605":
                    retval = "Internal Control Module Read Only Memory (ROM) Error ";
                    break;
                case "P0606":
                    retval = "ECM/PCM Processor Fault";
                    break;
                case "P0607":
                    retval = "CPU Fault";
                    break;
                case "P1606":
                    retval = "Main CPU fault";
                    break;
                case "P0608":
                    retval = "Control Module VSS Output A Malfunction";
                    break;
                case "P0609":
                    retval = "Control Module VSS Output B Malfunction";
                    break;
                case "P0610":
                    retval = "Control Module VSS Output B Malfunction";
                    break;
                case "P0615":
                    retval = "Starter Relay Circuit";
                    break;
                case "P0616":
                    retval = "Starter Relay Circuit Low";
                    break;
                case "P0617":
                    retval = "Starter Relay Circuit High";
                    break;
                case "P0618":
                    retval = "Alternative Fuel Control Module KAM Error";
                    break;
                case "P0619":
                    retval = "Alternative Fuel Control Module RAM/ROM Error";
                    break;
                case "P0620":
                    retval = "Generator Control Circuit Malfunction";
                    break;
                case "P0621":
                    retval = "Generator Lamp L Control Circuit Malfunction";
                    break;
                case "P0622":
                    retval = "Generator Field F Control Circuit Malfunction";
                    break;
                case "P0623":
                    retval = "Generator Lamp Control Circuit";
                    break;
                case "P0624":
                    retval = "Fuel Cap Lamp Control Circuit";
                    break;
                case "P0625":
                    retval = "Generator field F terminal control circuit low";
                    break;
                case "P0626":
                    retval = "Generator field F terminal control circuit high";
                    break;
                case "P2500":
                    retval = "Generator field L terminal control circuit low";
                    break;
                case "P2501":
                    retval = "Generator field L terminal control circuit high";
                    break;
                case "P0628":
                    retval = "Fuel pump relay circuit low input";
                    break;
                case "P0629":
                    retval = "Fuel pump relay circuit high input";
                    break;
                case "P0630":
                    retval = "VIN Not Programmed or Mismatch - ECM/PCM";
                    break;
                case "P0631":
                    retval = "VIN Not Programmed or Mismatch - TCM";
                    break;
                case "P0632":
                    retval = "Wheel circumference  not programmed";
                    break;
                case "P0633":
                    retval = "Immo not programmed";
                    break;
                case "P1609":
                    retval = "CIM transmitted response was distorted";
                    break;
                case "P1619":
                    retval = "Security access not armed";
                    break;
                case "P0635":
                    retval = "Power Steering Control Circuit";
                    break;
                case "P0636":
                    retval = "Power Steering Control Circuit Low";
                    break;
                case "P0637":
                    retval = "Power Steering Control Circuit High";
                    break;
                case "P0638":
                    retval = "Throttle Actuator Control Range/Performance (Bank 1)";
                    break;
                case "P0639":
                    retval = "Throttle Actuator Control Range/Performance (Bank 2)";
                    break;
                case "P0640":
                    retval = "Intake Air Heater Control Circuit";
                    break;
                case "P0641":
                    retval = "Reference voltage 1 out of range";
                    break;
                case "P0651":
                    retval = "Reference voltage 2 out of range";
                    break;
                case "P0645":
                    retval = "A/C Clutch Relay Control Circuit";
                    break;
                case "P0646":
                    retval = "A/C Clutch Relay Control Circuit Low";
                    break;
                case "P0647":
                    retval = "A/C Clutch Relay Control Circuit High";
                    break;
                case "P0648":
                    retval = "Immobilizer Lamp Control Circuit";
                    break;
                case "P0649":
                    retval = "Speed Control Lamp Control Circuit";
                    break;
                case "P0650":
                    retval = "Malfunction Indicator Lamp (MIL) Control Circuit Malfunction";
                    break;
                case "P0654":
                    retval = "Engine RPM Output Circuit Malfunction";
                    break;
                case "P0655":
                    retval = "Engine Hot Lamp Output Control Circuit Malfunction";
                    break;
                case "P0656":
                    retval = "Fuel Level Output Circuit Malfunction";
                    break;
                case "P0660":
                    retval = "Intake Manifold Tuning Valve Control Circuit (Bank 1)";
                    break;
                case "P0661":
                    retval = "Intake Manifold Tuning Valve Control Circuit Low (Bank 1)";
                    break;
                case "P0662":
                    retval = "Intake Manifold Tuning Valve Control Circuit High (Bank 1)";
                    break;
                case "P0663":
                    retval = "Intake Manifold Tuning Valve Control Circuit (Bank 2)";
                    break;
                case "P0664":
                    retval = "Intake Manifold Tuning Valve Control Circuit Low (Bank 2)";
                    break;
                case "P0665":
                    retval = "Intake Manifold Tuning Valve Control Circuit High (Bank 2)";
                    break;
                case "P0686":
                    retval = "Powertrain relay low performance problem";
                    break;
                case "P0687":
                    retval = "Powertrain relay high performance problem";
                    break;
                case "P0685":
                    retval = "Powertrain relay signal performance problem";
                    break;
                case "P0691":
                    retval = "Cooling fan 1 control circuit low";
                    break;
                case "P0692":
                    retval = "Cooling fan 1 control circuit high";
                    break;
                case "P0693":
                    retval = "Cooling fan 2 control circuit low";
                    break;
                case "P0694":
                    retval = "Cooling fan 2 control circuit high";
                    break;
                case "P0695":
                    retval = "Cooling fan 3 control circuit low";
                    break;
                case "P0696":
                    retval = "Cooling fan 3 control circuit high";
                    break;
                case "P0700":
                    retval = "Transmission Control System Malfunction";
                    break;
                case "P0701":
                    retval = "Transmission Control System Range/Performance";
                    break;
                case "P0702":
                    retval = "Transmission Control System Electrical";
                    break;
                case "P0703":
                    retval = "Torque Converter/Brake Switch B Circuit Malfunction";
                    break;
                case "P0704":
                    retval = "Clutch Switch Input Circuit Malfunction";
                    break;
                case "P0705":
                    retval = "Transmission Range Sensor Circuit Malfunction (PRNDL Input)";
                    break;
                case "P0706":
                    retval = "Transmission Range Sensor Circuit Range/Performance";
                    break;
                case "P0707":
                    retval = "Transmission Range Sensor Circuit Low Input";
                    break;
                case "P0708":
                    retval = "Transmission Range Sensor Circuit High Input";
                    break;
                case "P0709":
                    retval = "Transmission Range Sensor Circuit Intermittent";
                    break;
                case "P0710":
                    retval = "Transmission Fluid Temperature Sensor Circuit Malfunction";
                    break;
                case "P0711":
                    retval = "Transmission Fluid Temperature Sensor Circuit Range/Performance";
                    break;
                case "P0712":
                    retval = "Transmission Fluid Temperature Sensor Circuit Low Input";
                    break;
                case "P0713":
                    retval = "Transmission Fluid Temperature Sensor Circuit High Input";
                    break;
                case "P0714":
                    retval = "Transmission Fluid Temperature Sensor Circuit Intermittent";
                    break;
                case "P0715":
                    retval = "Input/Turbine Speed Sensor Circuit Malfunction";
                    break;
                case "P0716":
                    retval = "Input/Turbine Speed Sensor Circuit Range/Performance";
                    break;
                case "P0717":
                    retval = "Input/Turbine Speed Sensor Circuit No Signal";
                    break;
                case "P0718":
                    retval = "Input/Turbine Speed Sensor Circuit Intermittent";
                    break;
                case "P0719":
                    retval = "Torque Converter/Brake Switch B Circuit Low";
                    break;
                case "P0720":
                    retval = "Output Speed Sensor Circuit Malfunction";
                    break;
                case "P0721":
                    retval = "Output Speed Sensor Circuit Range/Performance";
                    break;
                case "P0722":
                    retval = "Output Speed Sensor Circuit No Signal";
                    break;
                case "P0723":
                    retval = "Output Speed Sensor Circuit Intermittent";
                    break;
                case "P0724":
                    retval = "Torque Converter/Brake Switch B Circuit High";
                    break;
                case "P0725":
                    retval = "Engine Speed Input Circuit Malfunction";
                    break;
                case "P0726":
                    retval = "Engine Speed Input Circuit Range/Performance";
                    break;
                case "P0727":
                    retval = "Engine Speed Input Circuit No Signal";
                    break;
                case "P0728":
                    retval = "Engine Speed Input Circuit Intermittent";
                    break;
                case "P0730":
                    retval = "Incorrect Gear Ratio";
                    break;
                case "P0731":
                    retval = "Gear 1 Incorrect Ratio";
                    break;
                case "P0732":
                    retval = "Gear 2 Incorrect Ratio";
                    break;
                case "P0733":
                    retval = "Gear 3 Incorrect Ratio";
                    break;
                case "P0734":
                    retval = "Gear 4 Incorrect Ratio";
                    break;
                case "P0735":
                    retval = "Gear 5 Incorrect Ratio";
                    break;
                case "P0736":
                    retval = "Reverse Incorrect Ratio";
                    break;
                case "P0740":
                    retval = "Torque Converter Clutch Circuit Malfunction";
                    break;
                case "P0741":
                    retval = "Torque Converter Clutch Circuit Performance or Stuck Off";
                    break;
                case "P0742":
                    retval = "Torque Converter Clutch Circuit Stuck On";
                    break;
                case "P0743":
                    retval = "Torque Converter Clutch Circuit Electrical";
                    break;
                case "P0744":
                    retval = "Torque Converter Clutch Circuit Intermittent";
                    break;
                case "P0745":
                    retval = "Pressure Control Solenoid Malfunction";
                    break;
                case "P0746":
                    retval = "Pressure Control Solenoid Performance or Stuck Off";
                    break;
                case "P0747":
                    retval = "Pressure Control Solenoid Stuck On";
                    break;
                case "P0748":
                    retval = "Pressure Control Solenoid Electrical";
                    break;
                case "P0749":
                    retval = "Pressure Control Solenoid Intermittent";
                    break;
                case "P0750":
                    retval = "Shift Solenoid A Malfunction";
                    break;
                case "P0751":
                    retval = "Shift Solenoid A Performance or Stuck Off";
                    break;
                case "P0752":
                    retval = "Shift Solenoid A Stuck On";
                    break;
                case "P0753":
                    retval = "Shift Solenoid A Electrical";
                    break;
                case "P0754":
                    retval = "Shift Solenoid A Intermittent";
                    break;
                case "P0755":
                    retval = "Shift Solenoid B Malfunction";
                    break;
                case "P0756":
                    retval = "Shift Solenoid B Performance or Stuck Off";
                    break;
                case "P0757":
                    retval = "Shift Solenoid B Stuck On";
                    break;
                case "P0758":
                    retval = "Shift Solenoid B Electrical";
                    break;
                case "P0759":
                    retval = "Shift Solenoid B Intermittent";
                    break;
                case "P0760":
                    retval = "Shift Solenoid C Malfunction";
                    break;
                case "P0761":
                    retval = "Shift Solenoid C Performance or Stuck Off";
                    break;
                case "P0762":
                    retval = "Shift Solenoid C Stuck On";
                    break;
                case "P0763":
                    retval = "Shift Solenoid C Electrical";
                    break;
                case "P0764":
                    retval = "Shift Solenoid C Intermittent";
                    break;
                case "P0765":
                    retval = "Shift Solenoid D Malfunction";
                    break;
                case "P0766":
                    retval = "Shift Solenoid D Performance or Stuck Off";
                    break;
                case "P0767":
                    retval = "Shift Solenoid D Stuck On";
                    break;
                case "P0768":
                    retval = "Shift Solenoid D Electrical";
                    break;
                case "P0769":
                    retval = "Shift Solenoid D Intermittent";
                    break;
                case "P0770":
                    retval = "Shift Solenoid E Malfunction";
                    break;
                case "P0771":
                    retval = "Shift Solenoid E Performance or Stuck Off";
                    break;
                case "P0772":
                    retval = "Shift Solenoid E Stuck On";
                    break;
                case "P0773":
                    retval = "Shift Solenoid E Electrical";
                    break;
                case "P0774":
                    retval = "Shift Solenoid E Intermittent";
                    break;
                case "P0775":
                    retval = "Pressure Control Solenoid B";
                    break;
                case "P0776":
                    retval = "Pressure Control Solenoid B Performance or Stuck off";
                    break;
                case "P0777":
                    retval = "Pressure Control Solenoid B Stuck On";
                    break;
                case "P0778":
                    retval = "Pressure Control Solenoid B Electrical";
                    break;
                case "P0779":
                    retval = "Pressure Control Solenoid B Intermittent";
                    break;
                case "P0780":
                    retval = "Shift Malfunction";
                    break;
                case "P0781":
                    retval = "1-2 Shift Malfunction";
                    break;
                case "P0782":
                    retval = "2-3 Shift Malfunction";
                    break;
                case "P0783":
                    retval = "3-4 Shift Malfunction";
                    break;
                case "P0784":
                    retval = "4-5 Shift Malfunction";
                    break;
                case "P0785":
                    retval = "Shift/Timing Solenoid Malfunction";
                    break;
                case "P0786":
                    retval = "Shift/Timing Solenoid Range/Performance";
                    break;
                case "P0787":
                    retval = "Shift/Timing Solenoid low";
                    break;
                case "P0788":
                    retval = "Shift/Timing Solenoid High";
                    break;
                case "P0789":
                    retval = "Shift/Timing Solenoid Intermittent";
                    break;
                case "P0790":
                    retval = "Normal/Performance Switch Circuit Malfunction";
                    break;
                case "P0791":
                    retval = "Intermediate Shaft Speed Sensor Circuit";
                    break;
                case "P0792":
                    retval = "Intermediate Shaft Speed Sensor Circuit Range/Performance";
                    break;
                case "P0793":
                    retval = "Intermediate Shaft Speed Sensor Circuit No signal";
                    break;
                case "P0794":
                    retval = "Intermediate Shaft Speed Sensor Circuit Intermittent";
                    break;
                case "P0795":
                    retval = "Pressure Control Solenoid C";
                    break;
                case "P0796":
                    retval = "Pressure Control Solenoid C Performance or Stuck off";
                    break;
                case "P0797":
                    retval = "Pressure Control Solenoid C Stuck On";
                    break;
                case "P0798":
                    retval = "Pressure Control Solenoid C Electrical";
                    break;
                case "P0799":
                    retval = "Pressure Control Solenoid C Intermittent";
                    break;
                case "P0801":
                    retval = "Reverse Inhibit Control Circuit Malfunction";
                    break;
                case "P0803":
                    retval = "1-4 Upshift (Skip Shift) Solenoid Control Circuit Malfunction";
                    break;
                case "P0804":
                    retval = "1-4 Upshift (Skip Shift) Lamp Control Circuit Malfunction";
                    break;
                case "P0805":
                    retval = "Clutch Position Sensor Circuit Malfunction";
                    break;
                case "P0806":
                    retval = "Clutch Position Sensor Circuit Range/Performance Malfunction";
                    break;
                case "P0807":
                    retval = "Clutch Position Sensor Circuit Low Malfunction";
                    break;
                case "P0808":
                    retval = "Clutch Position Sensor Circuit High Malfunction";
                    break;
                case "P0809":
                    retval = "Clutch Position Sensor Circuit Intermittent Malfunction";
                    break;
                case "P0810":
                    retval = "Clutch Position Control Error";
                    break;
                case "P0811":
                    retval = "Excessive Clutch Slippage";
                    break;
                case "P0812":
                    retval = "Reverse Input Circuit";
                    break;
                case "P0813":
                    retval = "Reverse Output Circuit";
                    break;
                case "P0814":
                    retval = "Transmission Range Display Circuit";
                    break;
                case "P0815":
                    retval = "Upshift Switch Circuit";
                    break;
                case "P0816":
                    retval = "Downshift Switch Circuit";
                    break;
                case "P0817":
                    retval = "Starter Disable Circuit";
                    break;
                case "P0818":
                    retval = "Driveline Disconnect Switch Input Circuit";
                    break;
                case "P0820":
                    retval = "Gear Lever X - Y Position Sensor Circuit";
                    break;
                case "P0821":
                    retval = "Gear Lever X Position Circuit";
                    break;
                case "P0822":
                    retval = "Gear Lever Y Position Circuit";
                    break;
                case "P0823":
                    retval = "Gear Lever X Position Circuit Intermittent";
                    break;
                case "P0824":
                    retval = "Gear Lever Y Position Circuit Intermittent";
                    break;
                case "P0825":
                    retval = "Gear Lever Push - Pull Switch (Shift Anticipate)";
                    break;
                case "P0830":
                    retval = "Clutch Pedal Switch A Circuit";
                    break;
                case "P0831":
                    retval = "Clutch Pedal Switch A Circuit Low";
                    break;
                case "P0832":
                    retval = "Clutch Pedal Switch A Circuit High";
                    break;
                case "P0833":
                    retval = "Clutch Pedal Switch B Circuit";
                    break;
                case "P0834":
                    retval = "Clutch Pedal Switch B Circuit Low";
                    break;
                case "P0835":
                    retval = "Clutch Pedal Switch B Circuit High";
                    break;
                case "P0836":
                    retval = "Four Wheel Drive (4WD) Switch Circuit";
                    break;
                case "P0837":
                    retval = "Four Wheel Drive (4WD) Switch Circuit Range/Performance";
                    break;
                case "P0838":
                    retval = "Four Wheel Drive (4WD) Switch Circuit Low";
                    break;
                case "P0839":
                    retval = "Four Wheel Drive (4WD) Switch Circuit High";
                    break;
                case "P0840":
                    retval = "Transmission Fluid Pressure Sensor/Switch A Circuit";
                    break;
                case "P0841":
                    retval = "Transmission Fluid Pressure Sensor/Switch A Circuit Range/Performance";
                    break;
                case "P0842":
                    retval = "Transmission Fluid Pressure Sensor/Switch A Circuit Low";
                    break;
                case "P0843":
                    retval = "Transmission Fluid Pressure Sensor/Switch A Circuit High";
                    break;
                case "P0844":
                    retval = "Transmission Fluid Pressure Sensor/Switch A Circuit Intermittent";
                    break;
                case "P0845":
                    retval = "Transmission Fluid Pressure Sensor/Switch B Circuit";
                    break;
                case "P0846":
                    retval = "Transmission Fluid Pressure Sensor/Switch B Circuit Range/Performance";
                    break;
                case "P0847":
                    retval = "Transmission Fluid Pressure Sensor/Switch B Circuit Low";
                    break;
                case "P0848":
                    retval = "Transmission Fluid Pressure Sensor/Switch B Circuit High";
                    break;
                case "P0849":
                    retval = "Transmission Fluid Pressure Sensor/Switch B Circuit Intermittent";
                    break;
                case "P1000 ":
                    retval = "OBD II Test Incomplete ";
                    break;
                case "P1001":
                    retval = "KOER Not Able To Complete, KOER Aborted ";
                    break;
                case "P1100":
                    retval = "MAF Intermittant";
                    break;
                case "P1101":
                    retval = "MAF Out Of Range";
                    break;
                case "P1106":
                    retval = "Manifold Absolute Pressure (MAP) sensor circuit intermittent high voltage";
                    break;
                case "P1107":
                    retval = "Manifold Absolute Pressure (MAP) sensor circuit intermittent low voltage";
                    break;
                case "P1108":
                    retval = "BARO to MAP signal circuit comparison too high";
                    break;
                case "P1111":
                    retval = "Intake Air Temperature (IAT) sensor intermittent high voltage";
                    break;
                case "P1112":
                    retval = "Intake Air Temperature (IAT) sensor intermittent low voltage";
                    break;
                case "P1114":
                    retval = "Engine Coolant Temperature (ECT) sensor circuit intermittent low voltage";
                    break;
                case "P1115":
                    retval = "Engine Coolant Temperature (ECT) sensor circuit intermittent high voltage";
                    break;
                case "P1116":
                    retval = "ECT Sensor Out Of Range";
                    break;
                case "P1117":
                    retval = "ECT Intermittent";
                    break;
                case "P1120":
                    retval = "TPS Out Of Range Low";
                    break;
                case "P1121":
                    retval = "Throttle Position (TP) Sensor Inconsistent With MAF Sensor High Voltage";
                    break;
                case "P1122":
                    retval = "Throttle Position (TP) Sensor Inconsistent With MAF Sensor Low Voltage";
                    break;
                case "P1124":
                    retval = "TPS Out Of Self Test Range";
                    break;
                case "P1125":
                    retval = "TPS Intermittant";
                    break;
                case "P1127":
                    retval = "Exhaust Not Warm Enough, Downstream Sensor Not Tested ";
                    break;
                case "P1128":
                    retval = "MAP Lower Than Expected";
                    break;
                case "P1129":
                    retval = "MAP Higher Than Expected";
                    break;
                case "P1130":
                    retval = "Lack Of HO2S-11, Fuel Trim At Limit ";
                    break;
                case "P1131":
                    retval = "HO2S 11 Indicates Lean";
                    break;
                case "P1132":
                    retval = "HO2S 11 Indicates Rich";
                    break;
                case "P1133":
                    retval = "Heated Oxygen Sensor (HO2S) insufficient switching bank 1 sensor 1 (Rear Bank)";
                    break;
                case "P1134":
                    retval = "Heated Oxygen Sensor (HO2S) transition time ratio bank 1 sensor 1 (Rear Bank)";
                    break;
                case "P1137":
                    retval = "Lack Of HO2S-12 Switch Indicates Lean ";
                    break;
                case "P1138":
                    retval = "Lack Of HO2S-12 Switch Indicates Rich ";
                    break;
                case "P1150":
                    retval = "Lack Of HO2S-21 Switch Fuel Trim At Limit ";
                    break;
                case "P1151":
                    retval = "Lack Of HO2S-21 Switch Indicates Lean ";
                    break;
                case "P1152":
                    retval = "Lack Of HO2S-21 Switch Indicates Rich ";
                    break;
                case "P1153":
                    retval = "Heated Oxygen Sensor (HO2S) insufficient switching bank 2 sensor 1 (Front Bank)";
                    break;
                case "P1154":
                    retval = "Heated Oxygen Sensor (HO2S) transition time ratio bank 2 sensor 1 (Front Bank)";
                    break;
                case "P1157":
                    retval = "Lack Of HO2S-22 Switch Indicates Lean ";
                    break;
                case "P1158":
                    retval = "Lack Of HO2S-22 Switch Indicates Rich ";
                    break;
                case "P1168":
                    retval = "FRP Sensor In Range But Low ";
                    break;
                case "P1169":
                    retval = "FRP Sensor In Range But High ";
                    break;
                case "P1180":
                    retval = "Fuel Delivery System Low ";
                    break;
                case "P1181":
                    retval = "Fuel Delivery System High ";
                    break;
                case "P1183":
                    retval = "EOT Sensor Circuit Malfunction ";
                    break;
                case "P1184":
                    retval = "EOT Sensor Out Of Range ";
                    break;
                case "P1189":
                    retval = "Engine Oil Pressure Switch Circuit";
                    break;
                case "P1192":
                    retval = "Inlet Air Temp. Circuit Low";
                    break;
                case "P1193":
                    retval = "Inlet Air Temp. Circuit High";
                    break;
                case "P1195":
                    retval = "1/1 O2 Sensor Slow During Catalyst Monitor";
                    break;
                case "P1196":
                    retval = "2/1 O2 Sensor Slow During Catalyst Monitor";
                    break;
                case "P1197":
                    retval = "1/2 O2 Sensor Slow During Catalyst Monitor";
                    break;
                case "P1198":
                    retval = "Radiator Temperature Sensor Volts Too High";
                    break;
                case "P1199":
                    retval = "Radiator Temperature Sensor Volts Too Low";
                    break;
                case "P1229":
                    retval = "Supercharger Intercooler Pump Not Working ";
                    break;
                case "P1230":
                case "P1231":
                    retval = "Throttle body sensor failed";
                    break;
                case "P1232":
                    retval = "Low Speed Fuel Pump Primary Circuit Malfunction ";
                    break;
                case "P1233":
                    retval = "Fuel System Disabled Or Offline ";
                    break;
                case "P1234":
                    retval = "Fuel System Disabled Or Offline ";
                    break;
                case "P1235":
                    retval = "Fuel Pump Control Out Of Range ";
                    break;
                case "P1236":
                    retval = "Fuel Pump Control Out Of Range ";
                    break;
                case "P1237":
                    retval = "Fuel Pump Secondary Circuit Malfunction ";
                    break;
                case "P1238":
                    retval = "Fuel Pump Secondary Circuit Malfunction ";
                    break;
                case "P1244":
                    retval = "Generator Load Low ";
                    break;
                case "P1245":
                    retval = "Generator Load Input High ";
                    break;
                case "P1246":
                    retval = "Generator Load Input Failed ";
                    break;
                case "P1258":
                    retval = "Engine Metal Over Temperature Protection";
                    break;
                case "P1259":
                    retval = "VTEC System Malfunction";
                    break;
                case "P1260":
                    retval = "Fuel Pump Speed Relay Control Circuit";
                    break;
                case "P1269":
                    retval = "Injector 3 Correction Circuit. Short to B+/open ";
                    break;
                case "P1270":
                    retval = "Vehicle Speed Limiter Reached";
                    break;
                case "P1281":
                    retval = "Engine Is Cold Too Long";
                    break;
                case "P1282":
                    retval = "Fuel Pump Relay Control Circuit";
                    break;
                case "P1285":
                    retval = "Cylinder Head Over Temperature Sensed ";
                    break;
                case "P1288":
                    retval = "Intake Manifold Short Runner Solenoid Circuit";
                    break;
                case "P1290":
                    retval = "CHT Sensor Out Of Range ";
                    break;
                case "P1289":
                    retval = "CHT Sensor High Input ";
                    break;
                case "P1291":
                    retval = "No Temp Rise Seen From Fuel Heaters";
                    break;
                case "P1292":
                    retval = "CNG Pressure Sensor Voltage Too High";
                    break;
                case "P1293":
                    retval = "CNG Pressure Sensor Voltage Too Low";
                    break;
                case "P1294":
                    retval = "Target Idle Not Reached";
                    break;
                case "P1295":
                    retval = "No 5 Volts To TP Sensor";
                    break;
                case "P1296":
                    retval = "No 5 Volts To MAP Sensor";
                    break;
                case "P1297":
                    retval = "Low Voltage ELD Circuit";
                    break;
                case "P1298":
                    retval = "High Voltage In ELD Circuit";
                    break;
                case "P1299":
                    retval = "Vacuum Leak Found (IAC Fully Seated)";
                    break;
                case "P1300":
                    retval = "Random Misfire";
                    break;
                case "P1309":
                    retval = "Misfire Monitor Disabled ";
                    break;
                case "P1320":
                    retval = "Ignition Control (IC) Module 4x Reference Circuit Intermittent No Pulses";
                    break;
                case "P1323":
                    retval = "Ignition Control (IC) Module 24x Reference Circuit low frequency";
                    break;
                case "P1336":
                    retval = "Crankshaft Position System Variation Not Learned";
                    break;
                case "P1337":
                    retval = "Crankshaft Speed Fluctuation Sensor No Signal";
                    break;
                case "P1350":
                    retval = "Ignition Control System";
                    break;
                case "P1351":
                    retval = "Ignition Coil Control Circuit High Voltage";
                    break;
                case "P1352":
                    retval = "Ignition Bypass Circuit High Voltage";
                    break;
                case "P1359":
                    retval = "Crankshaft Position/TDC/Cylinder Position Sensor Connector Disconnection";
                    break;
                case "P1361":
                    retval = "Ignition Control (IC) Circuit Low Voltage";
                    break;
                case "P1362":
                    retval = "Ignition Bypass Circuit Low Voltage";
                    break;
                case "P1366":
                    retval = "Intermittent Interruption In TDC2 Sensor Circuit";
                    break;
                case "P1367":
                    retval = "No Signal In TDC2 Sensor Circuit";
                    break;
                case "P1370":
                    retval = "Ignition Control (IC) Module 4x Reference too many pulses";
                    break;
                case "P1371":
                    retval = "Ignition Control (IC) Module 4x Reference too few pulses";
                    break;
                case "P1374":
                    retval = "CKP High to Low Resolution Frequency Correlation";
                    break;
                case "P1375":
                    retval = "Ignition Control (IC) Module 24x Reference High Voltage";
                    break;
                case "P1376":
                    retval = "Ignition Ground Circuit";
                    break;
                case "P1377":
                    retval = "Ignition Control (IC) Module Cam Pulse to 4x Reference Pulse Comparison";
                    break;
                case "P1380":
                    retval = "Variable Cam Timing Solenoid A Circuit Malfunction ";
                    break;
                case "P1381":
                    retval = "Misfire Detected-No EBTCM/PCM Serial Data";
                    break;
                case "P1382":
                    retval = "Cylinder Position Sensor No Signal";
                    break;
                case "P1383":
                    retval = "Variable Cam Timing Over-retarded (Bank 1) ";
                    break;
                case "P1388":
                    retval = "Auto Shutdown Relay Circuit";
                    break;
                case "P1389":
                    retval = "No ASD Relay Output Voltage At PCM";
                    break;
                case "P1390":
                    retval = "Octane Adjust Out Of Range";
                    break;
                case "P1391":
                    retval = "Intermittent Loss of CMP or CKP";
                    break;
                case "P1398":
                    retval = "Mis-Fire Adapter Numerator at Limit";
                    break;
                case "P1399":
                    retval = "Wait To Start Lamp Circuit";
                    break;
                case "P1400":
                    retval = "DPFE Sensor Low Voltage";
                    break;
                case "P1401":
                    retval = "DPFE Sensor High Voltage";
                    break;
                case "P1403":
                    retval = "No 5 Volts To EGR Sensor";
                    break;
                case "P1404":
                    retval = "Exhaust Gas Recirculation (EGR) Valve Pintle Stuck Open";
                    break;
                case "P1405":
                    retval = "DPFE Upstream Hose Off Or Plugged ";
                    break;
                case "P1406":
                    retval = "DPFE Downstream Hose Off Or Plugged ";
                    break;
                case "P1407":
                    retval = "EGR No Flow Detected";
                    break;
                case "P1408":
                    retval = "EGR Out Of Self Test Range ";
                    break;
                case "P1409":
                    retval = "EGR Vacuum Regulator Solenoid Circuit Malfunction ";
                    break;
                case "P1411":
                    retval = "Secondary Air Injection System Downstream Flow ";
                    break;
                case "P1413":
                    retval = "Secondary Air Injection System Monitor Circuit Low ";
                    break;
                case "P1414":
                    retval = "Secondary Air Injection System Monitor Circuit High ";
                    break;
                case "P1432":
                    retval = "THTRC Circuit Failure ";
                    break;
                case "P1441":
                    retval = "Evaporative System Flow During Non-Purge";
                    break;
                case "P1443":
                    retval = "Small Or No Purge Flow Condition ";
                    break;
                case "P1444":
                    retval = "Purge Flow Sensor Low Input";
                    break;
                case "P1445":
                    retval = "Purge Flow Sensor High Input";
                    break;
                case "P1450":
                    retval = "Unable To Bleed Up Fuel Tank Vacuum ";
                    break;
                case "P1451":
                    retval = "EVAP Control System Canister Vent Solenoid Circuit Malfunction ";
                    break;
                case "P1452":
                    retval = "EVAP Control System Canister Vent Solenoid sensor offset low";
                    break;
                case "P1492":
                    retval = "EVAP Control System Canister Vent Solenoid sensor offset low (fuel)";
                    break;
                case "P1453":
                    retval = "EVAP Control System Canister Vent Solenoid sensor offset high";
                    break;
                case "P1493":
                    retval = "EVAP Control System Canister Vent Solenoid sensor offset high (fuel)";
                    break;
                case "P1457":
                    retval = "Leak Detected In EVAP Control Sys.(EVAP Canister Sys.)";
                    break;
                case "P1460":
                    retval = "WOT A/C Cutoff Circuit Malfunction ";
                    break;
                case "P1461":
                    retval = "ACP Sensor High Voltage ";
                    break;
                case "P1462":
                    retval = "ACP Sensor Low Voltage ";
                    break;
                case "P1463":
                    retval = "ACP Sensor Insufficent Pressure Change ";
                    break;
                case "P1464":
                    retval = "A/C Demand Out Of Range ";
                    break;
                case "P1469":
                    retval = "Low A/C Cycling Period ";
                    break;
                case "P1474":
                    retval = "HCF Primary Circuit Failure ";
                    break;
                case "P1476":
                    retval = "Too Little Secondary Air";
                    break;
                case "P1477":
                    retval = "Too Much Secondary Air";
                    break;
                case "P1478":
                    retval = "Battery Temp Sensor Volts Out of Limit";
                    break;
                case "P1479":
                    retval = "HFC Primary Circuit Failure ";
                    break;
                case "P1480":
                    retval = "PCV Solenoid Valve";
                    break;
                case "P1482":
                    retval = "Catalyst Temperature Sensor Circuit Shorted Low";
                    break;
                case "P1483":
                    retval = "Engine Cooling System Performance";
                    break;
                case "P1484":
                    retval = "Catalytic Converter overheat Detected";
                    break;
                case "P1485":
                    retval = "Air Injection Solenoid Circuit";
                    break;
                case "P1486":
                    retval = "Evap Leak Monitor Pinched Hose";
                    break;
                case "P1487":
                    retval = "Hi Speed Rad Fan CTRL Relay Circuit";
                    break;
                case "P1488":
                    retval = "Auxiliary 5 Volt Supply Output Too Low";
                    break;
                case "P1489":
                    retval = "High Speed Fan CTRL Relay Circuit";
                    break;
                case "P1490":
                    retval = "Low Speed Fan CTRL Relay Circuit";
                    break;
                case "P1491":
                    retval = "Rad Fan Control Relay Circuit";
                    break;
                /*case "P1492":
                    retval = "Ambient/Batt Temp Sen Volts Too High";
                    break;
                case "P1493":
                    retval = "Ambient/Batt Temp Sen Volts Too Low";
                    break;*/
                case "P1494":
                    retval = "Leak Detection Pump Switch or Mechanical Fault";
                    break;
                case "P1495":
                    retval = "Leak Detection Pump Solenoid Circuit";
                    break;
                case "P1496":
                    retval = "5 Volt Supply Output Too Low";
                    break;
                case "P1498":
                    retval = "High speed Rad Fan Ground CTRL Rly Circuit";
                    break;
                case "P1500":
                    retval = "Vehicle Speed Sensor (VSS) Intermittant ";
                    break;
                case "P1501":
                    retval = "Vehicle Speed Sensor (VSS) Out Of Range ";
                    break;
                case "P1502":
                    retval = "Vehicle Speed Sensor (VSS) Intermittant ";
                    break;
                case "P1505":
                    retval = "Idle Air Control At Adaptive Clip";
                    break;
                case "P1506":
                    retval = "Idle Air Control System Overspeed Error ";
                    break;
                case "P1507":
                    retval = "Idle Air Control System Underspeed Error ";
                    break;
                case "P1508":
                    retval = "Idle Air Control (IAC) System - Low RPM";
                    break;
                case "P1516":
                    retval = "Inlet Manifold Runner Control Input Error (Bank 1) ";
                    break;
                case "P1517":
                    retval = "Inlet Manifold Runner Control Input Error (Bank 2) ";
                    break;
                case "P1518":
                    retval = "Inlet Manifold Runner Control Stuck Open ";
                    break;
                case "P1519":
                    retval = "Inlet Manifold Runner Control Stuck Closed ";
                    break;
                case "P1520":
                    retval = "Intake Manifold Runner Control (IMRC) circuit malfunction";
                    break;
                case "P1524":
                    retval = "Throttle Position (TP) Sensor Learned Closed Throttle Angle Degrees Out-Of-Range";
                    break;
                case "P1527":
                    retval = "Trans Range / Pressure Switch Comparison";
                    break;
                case "P1546":
                    retval = "Air Conditioning (A/C) Clutch Relay Control Circuit";
                    break;
                case "P1549":
                    retval = "Intake Manifold Control Circuit Malfunction ";
                    break;
                case "P1550":
                    retval = "PSP Sensor Malfunction ";
                    break;
                case "P1554":
                    retval = "Cruise Engaged Circuit High Voltage";
                    break;
                case "P1560":
                    retval = "Cruise Control System-Transaxle Not in Drive";
                    break;
                case "P1564":
                    retval = "Cruise Control System-Vehicle Acceleration too high";
                    break;
                case "P1566":
                    retval = "Cruise Control System-Engine RPM Too High";
                    break;
                case "P1567":
                    retval = "Cruise Control- ABCS Active";
                    break;
                case "P1570":
                    retval = "Cruise Control System - Traction Control Active";
                    break;
                case "P1571":
                    retval = "Traction Control System PWM Circuit No Frequency";
                    break;
                case "P1572":
                    retval = "Brake Pedal Switch Circuit ";
                    break;
                case "P1574":
                    retval = "EBTCM System- Stop Lamp Switch Circuit High Voltage";
                    break;
                case "P1575":
                    retval = "Extended Travel Brake Switch Circuit High Voltage";
                    break;
                case "P1579":
                    retval = "Park/Neutral to Drive/Reverse At high Throttle Angle";
                    break;
                case "P1585":
                    retval = "Cruise Control Inhibit Output Circuit";
                    break;
                case "P1594":
                    retval = "Charging System Voltage Too High";
                    break;
                case "P1595":
                    retval = "Speed Control Solenoid Circuits";
                    break;
                case "P1596":
                    retval = "Speed Control Switch always High";
                    break;
                case "P1597":
                    retval = "Speed Control Switch always Low";
                    break;
                case "P1598":
                    retval = "A/C Pressure Sensor Volts Too High";
                    break;
                case "P1599":
                    retval = "A/C Pressure Sensor Volts Too Low";
                    break;
                case "P1602":
                    retval = "Loss of EBTCM Serial Data";
                    break;
                case "P1603":
                    retval = "Loss of SDM Serial Data";
                    break;
                case "P1604":
                    retval = "Loss of IPC Serial Data";
                    break;
                case "P1605":
                    retval = "Loss of HVACC Serial Data";
                    break;
                case "P1607":
                    retval = "Malfunction In PCM Internal Circuit";
                    break;
                case "P1610":
                    retval = "Immo switched off";
                    break;
                case "P1611":
                    retval = "Incorrect security code input";
                    break;
                case "P1613":
                    retval = "CIM response not received";
                    break;
                case "P1614":
                    retval = "CIM transmitted response incorrect";
                    break;
                case "P1615":
                    retval = "CIM failed powertrain identification";
                    break;
                case "P1616":
                    retval = "CIM failed environment identification";
                    break;
                case "P1617":
                    retval = "Engine Oil Level Switch Circuit";
                    break;
                case "P1621":
                    retval = "PCM Memory Performance";
                    break;
                case "P1626":
                    retval = "Theft Deterrent System - Fuel Enable Circuit";
                    break;
                case "P1630":
                    retval = "Theft Deterrent - PCM in Learn Mode";
                    break;
                case "P1631":
                    retval = "Theft Deterrent - Password Incorrect";
                    break;
                case "P1632":
                    retval = "Theft Deterrent - Fuel Disabled";
                    break;
                case "P1633":
                    retval = "Ignition Supplement Power Circuit Low Voltage";
                    break;
                case "P1634":
                    retval = "Ignition 1 Power Circuit Low Voltage";
                    break;
                case "P1635":
                    retval = "5 Volt Reference Circuit";
                    break;
                case "P1639":
                    retval = "5 Volt Reference 2 Circuit";
                    break;
                case "P1640":
                    retval = "Driver 1 - Input High Voltage";
                    break;
                case "P1641":
                    retval = "Malfunction Indicator Lamp (MIL) Control Circuit";
                    break;
                case "P1642":
                    retval = "Vehicle Speed Output Circuit";
                    break;
                case "P1644":
                    retval = "Delivered Torque Output Circuit";
                    break;
                case "P1645":
                    retval = "EVAP Solenoid Output Circuit";
                    break;
                case "P1646":
                    retval = "EVAP Vent Valve Output Circuit";
                    break;
                case "P1650":
                    retval = "Power Steering Pressure Switch Malfunction ";
                    break;
                case "P1651":
                    retval = "Power Steering Pressure Switch Input Malfunction";
                    break;
                case "P1652":
                    retval = "Lift/Dive Circuit";
                    break;
                case "P1654":
                    retval = "Cruise Disable Output Circuit";
                    break;
                case "P1656":
                    retval = "Automatic Transaxle";
                    break;
                case "P1660":
                    retval = "Cooling Fan Control Circuits";
                    break;
                case "P1676":
                    retval = "FPTDR Signal Failure";
                    break;
                case "P1678":
                    retval = "FPTDR Signal Line Failure";
                    break;
                case "P1680":
                    retval = "Clutch Released Switch Circuit/ECU ADC fault";
                    break;
                case "P1681":
                    retval = "No I/P Cluster CCD/J1850 Messages Received / Sensor switch fault";
                    break;
                case "P1682":
                    retval = "Charging System Voltage Too Low";
                    break;
                case "P1683":
                    retval = "Speed Control Power Relay or S/C 12V Driver Circuit";
                    break;
                case "P1685":
                    retval = "Skim Invalid Key";
                    break;
                case "P1686":
                    retval = "No SKIM Bus Message Received";
                    break;
                case "P1687":
                    retval = "A/T FI Signal B High Input";
                    break;
                case "P1689":
                    retval = "Traction Control Delivered Torque Output Circuit";
                    break;
                case "P1693":
                    retval = "DTC Detected In Companion Mode";
                    break;
                case "P1694":
                    retval = "Fault In Companion Mode";
                    break;
                case "P1695":
                    retval = "No CCD/J185O Message From BCM";
                    break;
                case "P1696":
                    retval = "PCM Failure EEPROM Write Denied";
                    break;
                case "P1697":
                    retval = "PCM Failure SRI Mile Not Stored";
                    break;
                case "P1698":
                    retval = "No Bus Message From TCM";
                    break;
                case "P1701":
                    retval = "Transmission Solenoid Malfunction";
                    break;
                case "P1703":
                    retval = "Brake On/Off Switch Out Of Range ";
                    break;
                case "P1705":
                    retval = "Transmisson Range Sensor Out Of Range  ";
                    break;
                case "P1706":
                    retval = "Automatic Transaxle";
                    break;
                case "P1709":
                    retval = "Park/Neutral Position Switch Out Of Range ";
                    break;
                case "P1710":
                    retval = "Automatic Transaxle";
                    break;
                case "P1711":
                    retval = "Transmission Fluid Temperature Sensor";
                    break;
                case "P1713":
                    retval = "Automatic Transaxle";
                    break;
                case "P1719":
                    retval = "Skip Shift Solenoid Circuit";
                    break;
                case "P1729":
                    retval = "4x4 Low Switch Malfunction ";
                    break;
                case "P1738":
                    retval = "Automatic Transaxle";
                    break;
                case "P1739":
                    retval = "Automatic Transaxle";
                    break;
                case "P1740":
                    retval = "Automatic Transaxle";
                    break;
                case "P1746":
                    retval = "EPC Solenoid Failed Low";
                    break;
                case "P1747":
                    retval = "EPC Solenoid Short Circuit";
                    break;
                case "P1749":
                    retval = "EPC Solenoid Open Circuit";
                    break;
                case "P1751":
                    retval = "Shift Solenoid #1 (SS1)";
                    break;
                case "P1753":
                    retval = "Automatic Transaxle";
                    break;
                case "P1754":
                    retval = "Coast Clutch Solenoid";
                    break;
                case "P1756":
                    retval = "GOV Press Not Equal To Target @ 15-20 PSI";
                    break;
                case "P1757":
                    retval = "GOV Press Not Equal To Target @ 15-20 PSI";
                    break;
                case "P1758":
                    retval = "Automatic Transaxle";
                    break;
                case "P1761":
                    retval = "Shift Solenoid #3 (SS3)";
                    break;
                case "P1762":
                    retval = "Gov Press Sen Offset Volts Too Low or high";
                    break;
                case "P1763":
                    retval = "Governor Pressure Sensor Volts Too High";
                    break;
                case "P1764":
                    retval = "Governor Pressure Sensor Volts Too Low";
                    break;
                case "P1765":
                    retval = "Trans 12 Volt Supply Relay CTRL Circuit";
                    break;
                case "P1768":
                    retval = "Automatic Transaxle";
                    break;
                case "P1773":
                    retval = "Automatic Transaxle";
                    break;
                case "P1778":
                    retval = "Automatic Transaxle";
                    break;
                case "P1780":
                    retval = "Transmission Control Switch Out Of Range";
                    break;
                case "P1781":
                    retval = "4x4 Switch Out Of Range ";
                    break;
                case "P1783":
                    retval = "Transmission Over Temperature Condition";
                    break;
                case "P1786":
                    retval = "Automatic Transaxle";
                    break;
                case "P1790":
                    retval = "Automatic Transaxle";
                    break;
                case "P1791":
                    retval = "Automatic Transaxle";
                    break;
                case "P1792":
                    retval = "Automatic Transaxle";
                    break;
                case "P1794":
                    retval = "Automatic Transaxle";
                    break;
                case "P1810":
                    retval = "TFP Valve Position Switch Circuit";
                    break;
                case "P1811":
                    retval = "Maximum Adapt and Long Shift";
                    break;
                case "P1819":
                    retval = "Internal Mode Switch - No Start/Wrong Range";
                    break;
                /*case "P1820":
                    retval = "Internal Mode Switch Circuit A Low";
                    break;*/
                case "P1822":
                    retval = "Internal Mode Switch Circuit B High";
                    break;
                case "P1823":
                    retval = "Internal Mode Switch Circuit P Low";
                    break;
                case "P1825":
                    retval = "Internal Mode Switch - Invalid Range";
                    break;
                case "P1826":
                    retval = "Internal Mode Switch Circuit C High";
                    break;
                case "P1860":
                    retval = "TCC PWM Solenoid Circuit Electrical";
                    break;
                case "P1887":
                    retval = "TCC Release Switch Circuit";
                    break;
                case "P1899":
                    retval = "Park/Neutral Position Switch Stuck In Park or In Gear";
                    break;
                case "P2227":
                    retval = "Barometric pressure circuit performance problem";
                    break;
                case "P2228":
                    retval = "Barometric pressure circuit low input";
                    break;
                case "P2229":
                    retval = "Barometric pressure circuit high input";
                    break;
                case "P2257":
                    retval = "SAI relay circuit low";
                    break;
                case "P2258":
                    retval = "SAI relay circuit high";
                    break;
                case "P2535":
                    retval = "Run crank signal high";
                    break;
                case "P2537":
                    retval = "Accessory switch circuit low input";
                    break;
                case "P1625":
                    retval = "Lost communication with ABS control module";
                    break;
                case "P1624":
                    retval = "Lost communication with CIM (Column Integration Module)";
                    break;
                case "P1623":
                    retval = "Lost communication with TCM (Transmission Control Module)";
                    break;
                case "P2135":
                    retval = "Throttle position sensor correlation fault";
                    break;
                case "P2176":
                    retval = "Throttle minimum learning fault";
                    break;
                case "P1523":
                    retval = "Throttle default position fault";
                    break;
                case "P2121":
                    retval = "Pedal position sensor 1 circuit out of range";
                    break;
                case "P2122":
                    retval = "Pedal position sensor 1 circuit low input";
                    break;
                case "P2123":
                    retval = "Pedal position sensor 1 circuit high input";
                    break;
                case "P2126":
                    retval = "Pedal position sensor 2 circuit out of range";
                    break;
                case "P2127":
                    retval = "Pedal position sensor 2 circuit low input";
                    break;
                case "P2128":
                    retval = "Pedal position sensor 2 circuit high input";
                    break;
                case "P2138":
                    retval = "Pedal position sensor correlation fault";
                    break;
                case "P1743":
                    retval = "TCM lockup";
                    break;
                case "P1704":
                    retval = "TCM neutral control";
                    break;
                case "P1804":
                    retval = "TCM CAN bus counter overrun";
                    break;
                case "P1805":
                    retval = "TCM lost communication with ECM";
                    break;
                case "P1895":
                    retval = "TCM unreliable torque figures";
                    break;
                case "P1820":
                    retval = "TCM unreliable accelerator position";
                    break;

            }
            if (retval == faultcode)
            {
                // no translation done, give generic indication
                //0 = Generic (this is the digit zero -- not the letter "O") 
                //1 = Enhanced (manufacturer specific) 
                try
                {
                    if (faultcode[1] == '0')
                    {
                        retval = "Generic: ";
                    }
                    else
                    {
                        retval = "Enhanced: ";
                    }
                    switch (faultcode[2])
                    {
                        /*
    1 = Emission Management (Fuel or Air) 
    2 = Injector Circuit (Fuel or Air) 
    3 = Ignition or Misfire 
    4 = Emission Control 
    5 = Vehicle Speed & Idle Control 
    6 = Computer & Output Circuit 
    7 = Transmission 
    8 = Transmission 
    9 = SAE Reserved 
    0 = SAE Reserved
                         * */
                        case '1':
                            retval += "Emission Management (Fuel or Air)";
                            break;
                        case '2':
                            retval += "Injector Circuit (Fuel or Air)";
                            break;
                        case '3':
                            retval += "Ignition or Misfire";
                            break;
                        case '4':
                            retval += "Emission Control";
                            break;
                        case '5':
                            retval += "Vehicle Speed / Idle Control";
                            break;
                        case '6':
                            retval += "Computer / Output Circuit";
                            break;
                        case '7':
                        case '8':
                            retval += "Transmission";
                            break;
                        case '0':
                        case '9':
                            retval += "SAE Reserved";
                            break;
                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }


            }
            return retval;
        }

        public void ClearCodes()
        {
            //listBox1.Items.Clear();
            DataTable dtn = new DataTable();
            dtn.Columns.Add("Code");
            dtn.Columns.Add("Description");
            gridControl1.DataSource = dtn;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            // clear this specific DTC code
            //TODO: cast an event to the main application to have it cleared
            int[] selrows = gridView1.GetSelectedRows();
            if (selrows.Length > 0)
            {
                foreach (int i in selrows)
                {
                    DataRow drv = gridView1.GetDataRow(i);
                    if (drv["Code"] != DBNull.Value)
                    {
                        if (onClearCurrentDTC != null)
                        {
                            onClearCurrentDTC(this, new ClearDTCEventArgs(drv["Code"].ToString()));
                        }
                    }
                }
            }
                 
            

           /* if (listBox1.SelectedIndex >= 0)
            {
                if (onClearCurrentDTC != null)
                {
                    onClearCurrentDTC(this, new ClearDTCEventArgs((string)listBox1.Items[listBox1.SelectedIndex]));
                }
            }*/
        }

        public class ClearDTCEventArgs : System.EventArgs
        {
            private string _dtccode;
            
            public string DTCCode
            {
                get
                {
                    return _dtccode;
                }
            }


            public ClearDTCEventArgs(string dtccode)
            {
                this._dtccode = dtccode;
            }
        }
    }
}