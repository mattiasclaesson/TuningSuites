using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using NLog;

namespace CommonSuite
{
    public partial class frmFaultcodes : DevExpress.XtraEditors.XtraForm
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private Dictionary<string, DTCDescription> mDTCDescriptionDict;

        public delegate void onClearDTC(object sender, ClearDTCEventArgs e);
        public event frmFaultcodes.onClearDTC onClearCurrentDTC;

        public delegate void frmClose(object sender, EventArgs e);
        public event frmFaultcodes.frmClose onCloseFrm;

        public frmFaultcodes()
        {
            InitializeComponent();
            LoadDTCDescriptions();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (onCloseFrm != null)
            {
                onCloseFrm(this, e);
            } 
            
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
                if (mDTCDescriptionDict.ContainsKey(faultcode))
                {
                    DTCDescription dtc = mDTCDescriptionDict[faultcode];
                    dt.Rows.Add(faultcode, dtc.Description);
                } else {
                    logger.Debug("Warning: dtc not found (" + faultcode + ")");
                }
            }
               
        }

        // Load known dts codes from XML file
        private void LoadDTCDescriptions()
        {
            // Create the XmlSchemaSet class.
            XmlSchemaSet sc = new XmlSchemaSet();

            // Add the schema to the collection.
            sc.Add("", "DTCDescription.xsd");

            XmlReaderSettings reader_settings = new XmlReaderSettings();
            reader_settings.ValidationType = ValidationType.Schema;
            reader_settings.Schemas = sc;
            reader_settings.ValidationEventHandler += new ValidationEventHandler(DTCDescriptionsValidationEventHandler);

            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string[] dtc_files = Directory.GetFiles(path+"\\", "DTC_*.xml");
            // sort file list on alfabetic order
            Array.Sort(dtc_files);

            mDTCDescriptionDict = new Dictionary<string, DTCDescription>();
            foreach (string file in dtc_files)
            {
                XmlReader reader = XmlReader.Create(file, reader_settings);
                XmlDocument doc = new XmlDocument();
                string error_message;
                try
                {
                    // parsing the xml, warnings/errors are generated in this step
                    doc.Load(reader);
                    XmlNode dtcdescriptions = doc.DocumentElement;
                    foreach (XmlNode dtcdescription_node in dtcdescriptions.SelectNodes("dtcdescription"))
                    {
                        DTCDescription dtc = new DTCDescription(dtcdescription_node);
                        if (dtc.IsComplete() && !mDTCDescriptionDict.ContainsKey(dtc.Code))
                        {
                            mDTCDescriptionDict.Add(dtc.Code, dtc);
                        }
                        else
                        {
                            error_message = dtc.Description + " <-> " + mDTCDescriptionDict[dtc.Code].Description;
                            logger.Debug("Warning: double code: " + dtc.Code + " >> " + error_message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Second time the parsing error is logged
                    logger.Debug(ex.Message);
                }

            }
  
        }

        private void DTCDescriptionsValidationEventHandler(object sender, ValidationEventArgs e)
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