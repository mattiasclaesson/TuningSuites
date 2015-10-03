using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CommonSuite
{
    public class DTCDescription
    {
        private string mCode;
        private string mDescription;
        private string mTips;
        private List<string> mReasonList = null;

        public string Code
        {
            get { return this.mCode;}
        }

        public string Description
        {
            get { return this.mDescription;}
        }

        public string Tips
        {
            get { return this.mTips; }
        }

        public List<string> ReasonList 
        {
            get { return this.mReasonList; }
        }


        public DTCDescription()
        {
 
        }

        public DTCDescription(XmlNode aDTCDescription)
        {
            XmlNode code = aDTCDescription.SelectNodes("code")[0];
            String str_code = code.InnerText.Trim();
            bool wis_code = (str_code.Length == 7);
            mCode = wis_code ? str_code.Substring(0, 5) : str_code;
            XmlNode desc = aDTCDescription.SelectNodes("description")[0];
            mDescription = desc.InnerText.Trim();
            XmlNode tips = aDTCDescription.SelectNodes("tips")[0];
            mTips = (tips != null) ? tips.InnerText.Trim() : string.Empty;
            XmlNodeList reason_list = aDTCDescription.SelectNodes("reason");
            mReasonList = new List<string>();
            foreach (XmlNode reason in reason_list)
            {
                mReasonList.Add(reason.InnerText.Trim());
            }
        }

        public bool IsComplete()
        {
            return !string.IsNullOrWhiteSpace(mCode) && !string.IsNullOrWhiteSpace(mDescription);
        }

        public string GenericOrEnhanced()
        {
            // no translation done, give generic indication
            //0 = Generic (this is the digit zero -- not the letter "O") 
            //1 = Enhanced (manufacturer specific)                        
            return (mCode[1] == '0') ? "Generic" : "Enhanced";
        }
        
        public string CodeType()
        {
            string retval = string.Empty;
            switch (mCode[2])
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
                */
                case '1':
                    retval = "Emission Management (Fuel or Air)";
                    break;
                case '2':
                    retval = "Injector Circuit (Fuel or Air)";
                    break;
                case '3':
                    retval = "Ignition or Misfire";
                    break;
                case '4':
                    retval = "Emission Control";
                    break;
                case '5':
                    retval = "Vehicle Speed / Idle Control";
                    break;
                case '6':
                    retval = "Computer / Output Circuit";
                    break;
                case '7':
                case '8':
                    retval = "Transmission";
                    break;
                case '0':
                case '9':
                    retval = "SAE Reserved";
                    break;
            }
            return retval;
        } 

    }
}
