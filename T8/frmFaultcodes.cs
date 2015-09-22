using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using CommonSuite;
using NLog;

namespace T8SuitePro
{
    public partial class frmFaultcodes : DevExpress.XtraEditors.XtraForm
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private Dictionary<string, DTSCode> mDTSCodesDict;

        public delegate void onClearDTC(object sender, ClearDTCEventArgs e);
        public event frmFaultcodes.onClearDTC onClearCurrentDTC;



        public frmFaultcodes()
        {
            InitializeComponent();
            LoadDTSCodes();
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
                if (mDTSCodesDict.ContainsKey(faultcode))
                {
                    DTSCode code = mDTSCodesDict[faultcode];
                    dt.Rows.Add(faultcode, code.Description);
                } else {
                    logger.Debug("Warning: DTSCode not found (" + faultcode + ")");
                }
            }
               
        }

        // Load known dts codes from XML file
        private void LoadDTSCodes()
        {
            // Create the XmlSchemaSet class.
            XmlSchemaSet sc = new XmlSchemaSet();

            // Add the schema to the collection.
            sc.Add("", "DTSCodes.xsd");

            XmlReaderSettings reader_settings = new XmlReaderSettings();
            reader_settings.ValidationType = ValidationType.Schema;
            reader_settings.Schemas = sc;
            reader_settings.ValidationEventHandler += new ValidationEventHandler(DTSCodesValidationEventHandler);
                        
            XmlReader reader = XmlReader.Create("DTSCodes.xml", reader_settings);
                        
            XmlDocument doc = new XmlDocument();
            try
            {
                // parsing the xml, warnings/errors are generated in this step
                doc.Load(reader);

                XmlNode dtscodes = doc.DocumentElement;

                mDTSCodesDict = new Dictionary<string, DTSCode>();
                foreach (XmlNode dtscode_node in dtscodes.SelectNodes("dtscode"))
                {
                    DTSCode code = new DTSCode(dtscode_node);
                    if (code.IsComplete() && !mDTSCodesDict.ContainsKey(code.Code))
                    {
                        mDTSCodesDict.Add(code.Code, code);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Second time the parsing error is logged
                logger.Debug(ex.Message);
            }
  
        }

        private void DTSCodesValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                logger.Debug("Warning: " + e.Message);
             }
            else if (e.Severity == XmlSeverityType.Error)
            {
                logger.Debug("Error: " + e.Message);
            }
        }


        private string TranslateFaultcode(string faultcode)
        {
            string retval = faultcode;
           // build the list
            switch (faultcode)
            {



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