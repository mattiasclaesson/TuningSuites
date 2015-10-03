using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace T7
{
    /// <summary>
    /// T7FileHeader represents the header (or, rather, tailer) of a T7 firmware file.
    /// The header contains meta data that describes some important parts of the firmware.
    /// 
    /// The header consists of several fields here represented by the FileHeaderField class.
    /// </summary>
    class T7FileHeader
    {
        string m_chassisID = "";
        string m_immobilizerID = "";
        int m_romChecksumType = 0;

        public int RomChecksumType
        {
            get { return m_romChecksumType; }
            set { m_romChecksumType = value; }
        }

        int m_bottomOfFlash = 0;

        public int BottomOfFlash
        {
            get { return m_bottomOfFlash; }
            set { m_bottomOfFlash = value; }
        }

        byte m_RomChecksumError = 0x31;

        public byte RomChecksumError
        {
            get { return m_RomChecksumError; }
            set { m_RomChecksumError = value; }
        }

        int m_valueF5 = 0;

        public int ValueF5
        {
            get { return m_valueF5; }
            set { m_valueF5 = value; }
        }
        int m_valueF6 = 0;

        public int ValueF6
        {
            get { return m_valueF6; }
            set { m_valueF6 = value; }
        }
        int m_valueF7 = 0;

        public int ValueF7
        {
            get { return m_valueF7; }
            set { m_valueF7 = value; }
        }
        int m_valueF8 = 0;

        public int ValueF8
        {
            get { return m_valueF8; }
            set { m_valueF8 = value; }
        }

        int m_9cvalue = 0;

        public int Unknown_9cvalue
        {
            get { return m_9cvalue; }
            set { m_9cvalue = value; }
        }

        int m_symboltableaddress = 0;

        public int Symboltableaddress
        {
            get { return m_symboltableaddress; }
            set { m_symboltableaddress = value; }
        }

        string m_vehicleidnr = string.Empty;
        string m_datemodified = string.Empty;

        byte[] m_LastModifiedBy = new byte[5];


        string m_testserialnr = string.Empty;

        public string Testserialnr
        {
            get { return m_testserialnr; }
            set { m_testserialnr = value; }
        }

        string m_enginetype = string.Empty;

        public string Enginetype
        {
            get { return m_enginetype; }
            set { m_enginetype = value; }
        }

        string m_ecuhardwarenr = string.Empty;

        public string Ecuhardwarenr
        {
            get { return m_ecuhardwarenr; }
            set { m_ecuhardwarenr = value; }
        }
        string m_softwareVersion = "";
        string m_carDescription = "";
        string m_partNumber = string.Empty;
        long fileLength;
        int m_checksumF2;
        int m_checksumFB;
        int m_fwLength;

        /// <summary>
        /// FileHeaderField represents a field in the file header.
        /// Each field consists of a field ID, a field length and data.
        /// </summary>
        class FileHeaderField
        {
            public byte m_fieldID;
            public byte m_fieldLength;
            public byte[] m_data = new byte[255];
        }

        /// <summary>
        /// This method saves a T7 file. This method should be called after one or more fields have
        /// been changed and you want to save the result.
        /// </summary>
        /// <param name="a_filename">File name of the file where to save the T7 file.</param>
        /// <returns>true on success, otherwise false.</returns>
        public bool save(string a_filename)
        {
            if (!File.Exists(a_filename))
                File.Create(a_filename);
            FileStream fs = new FileStream(a_filename, FileMode.Open, FileAccess.ReadWrite);
            FileHeaderField fhf;
            fs.Seek(0, SeekOrigin.End);
            long writePos;
            fileLength = fs.Position;
            do
            {
                writePos = fs.Position;
                fhf = readField(fs);
                switch (fhf.m_fieldID)
                {
                    case 0x90:  setHeaderString(fhf, m_chassisID);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0x97:  setHeaderString(fhf, m_carDescription);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0x95:  setHeaderString(fhf, m_softwareVersion);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0x92:  setHeaderString(fhf, m_immobilizerID);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0xFB:  setHeaderIntValue(fhf, m_checksumFB);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0xF2:  setHeaderIntValue(fhf, m_checksumF2);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0xFE:  setHeaderIntValue(fhf, m_fwLength);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    default:
                                break;
                }   
            }
            while (fhf.m_fieldID != 0xFF /* && fhf.m_fieldID != 0xF9 */);    // Don't write past 0xF9 "End of header"
            fs.Close();
            return true;
        }

        /// <summary>
        /// This method initiates this class with a new T7 file.
        /// </summary>
        /// <param name="a_filename">Name of the file to read.</param>
        /// <returns>True on success, otherwise false.</returns>
        public bool init(string a_filename, bool AutoFixFooter)
        {
            bool bChassisIDDetected = false;
            bool bImmocodeDetected = false;
            bool bSymbolTableMarkerDetected = false;
            bool bSymbolTableChecksumDetected = false;
            bool bF2ChecksumDetected = false;
            int iChassisIDCounter = 0;
            m_checksumF2 = 0;
            m_checksumFB = 0;
            m_chassisID = "";
            m_immobilizerID = "";
            m_softwareVersion = "";
            m_carDescription = "";
            m_partNumber = string.Empty;

            // init new values
            m_chassisID = "00000000000000000";
            m_immobilizerID = "000000000000000";
            m_enginetype = "0000000000000";
            m_vehicleidnr = "000000000";
            m_partNumber = "0000000";
            m_softwareVersion = "000000000000";
            m_carDescription = "00000000000000000000";
            m_datemodified = "0000";
            m_ecuhardwarenr = "0000000";
            m_LastModifiedBy = new byte[5];
            m_LastModifiedBy.SetValue((byte)0x42, 0);
            m_LastModifiedBy.SetValue((byte)0xFB, 1);
            m_LastModifiedBy.SetValue((byte)0xFA, 2);
            m_LastModifiedBy.SetValue((byte)0xFF, 3);
            m_LastModifiedBy.SetValue((byte)0xFF, 4);
            m_testserialnr = "050225";



            if (!File.Exists(a_filename)) 
                return false;
            FileStream fs = new FileStream(a_filename, FileMode.Open, FileAccess.Read);
            FileHeaderField fhf;
            fs.Seek(0, SeekOrigin.End);
            fileLength = fs.Position;
            do
            {
                fhf = readField(fs);
                switch (fhf.m_fieldID)
                {
                    case 0x90: 
                        m_chassisID = getHeaderString(fhf);
                        iChassisIDCounter++;
                        bChassisIDDetected = true;
                        break;
                    case 0x91:
                        // vehicleidnr ASCII
                        m_vehicleidnr = getHeaderString(fhf);
                        break;
                    case 0x92: 
                        m_immobilizerID = getHeaderString(fhf);
                        bImmocodeDetected = true;
                        break;
                    case 0x93:
                        // ecu hardware number ASCII
                        m_ecuhardwarenr = getHeaderString(fhf);
                        break;
                    case 0x94: 
                        m_partNumber = getHeaderString(fhf);
                        break;
                    case 0x95:
                        m_softwareVersion = getHeaderString(fhf);
                        break;
                    case 0x97: 
                        m_carDescription = getHeaderString(fhf);
                        break;
                    case 0x98:
                        m_enginetype = getHeaderString(fhf);
                        break;
                    case 0x99:
                        m_testserialnr = getHeaderString(fhf);
                        break;
                    case 0x9A:
                        m_datemodified = getHeaderString(fhf);
                        break;
                    case 0x9B: // symboltable address
                        m_symboltableaddress = getHeaderIntValue(fhf);
                        bSymbolTableMarkerDetected = true;
                        break;
                    case 0x9C: // unknown
                        m_9cvalue = getHeaderIntValue(fhf);
                        bSymbolTableChecksumDetected = true;
                        break;
                    case 0xF2:
                        m_checksumF2 = getHeaderIntValue(fhf);
                        bF2ChecksumDetected = true;
                        break;
                    case 0xF5:
                        m_valueF5 = getHeaderSmallIntValue(fhf);
                        break;
                    case 0xF6:
                        m_valueF6 = getHeaderSmallIntValue(fhf);
                        break;
                    case 0xF7:
                        m_valueF7 = getHeaderSmallIntValue(fhf);
                        break;
                    case 0xF8:
                        m_valueF8 = getHeaderSmallIntValue(fhf);
                        break;
                    case 0xF9:
                        m_RomChecksumError = getHeaderByteValue(fhf);
                        break;
                    case 0xFA:
                        m_LastModifiedBy = getHeaderDateValue(fhf);
                        break;
                    case 0xFB:
                        m_checksumFB = getHeaderIntValue(fhf);
                        break;
                    case 0xFC:
                        m_bottomOfFlash = getHeaderIntValue(fhf);
                        break;
                    case 0xFD:
                        m_romChecksumType = getHeaderIntValue(fhf);
                        break;
                    case 0xFE: 
                        m_fwLength = getHeaderIntValue(fhf);
                        break;
                    default:
                        break;
                }
            }
            while (fhf.m_fieldID != 0xFF /*&& fhf.m_fieldID != 0xF9 */);    // Don't read past 0xF9 "End of header"
            fs.Close();

            if ((iChassisIDCounter > 1 || !bImmocodeDetected || !bChassisIDDetected) && AutoFixFooter)
            {
                // seen chassis ID more than once, common error in binary files
                // rebuild the footer
                this.ClearFooter(a_filename);
                this.CreateNewFooter(a_filename, bSymbolTableMarkerDetected, bSymbolTableChecksumDetected, bF2ChecksumDetected);
            }
            return true;
        }

        private void CreateNewFooter(string a_filename, bool create9B, bool create9C, bool createF2)
        {
            if (!File.Exists(a_filename))
                return;
            FileStream fs = new FileStream(a_filename, FileMode.Open, FileAccess.ReadWrite);
            
            fs.Seek(0, SeekOrigin.End);
            long writePos;
            fileLength = fs.Position;

            writePos = fs.Position;

            FileHeaderField fhf = new FileHeaderField();

            fhf.m_fieldID = (byte)0x91;
            fhf.m_fieldLength = (byte)m_vehicleidnr.Length;
            setHeaderString(fhf, m_vehicleidnr);
            writeFieldIncludingDetails(fs, fhf);
           // fs.Position = writePos;

            fhf.m_fieldID = (byte)0x94;
            fhf.m_fieldLength = (byte)m_partNumber.Length;
            setHeaderString(fhf, m_partNumber);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0x95;
            fhf.m_fieldLength = (byte)m_softwareVersion.Length;
            setHeaderString(fhf, m_softwareVersion);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0x97;
            fhf.m_fieldLength = (byte)m_carDescription.Length;
            setHeaderString(fhf, m_carDescription);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0x9A;
            fhf.m_fieldLength = (byte)m_datemodified.Length;
            setHeaderString(fhf, m_datemodified);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;
            if (create9C)
            {
                fhf.m_fieldID = (byte)0x9C;
                fhf.m_fieldLength = 4;
                setHeaderIntValue(fhf, m_9cvalue);
                writeFieldIncludingDetails(fs, fhf);
            }
            //fs.Position = writePos;
            if (create9B)
            {
                fhf.m_fieldID = (byte)0x9B;
                fhf.m_fieldLength = 4;
                setHeaderIntValue(fhf, m_symboltableaddress);
                writeFieldIncludingDetails(fs, fhf);
            }
            //fs.Position = writePos;
            if (createF2)
            {
                fhf.m_fieldID = (byte)0xF2;
                fhf.m_fieldLength = 4;
                setHeaderIntValue(fhf, m_checksumF2);
                writeFieldIncludingDetails(fs, fhf);
            }
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0xFB;
            fhf.m_fieldLength = 4;
            setHeaderIntValue(fhf, m_checksumFB);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0xFC;
            fhf.m_fieldLength = 4;
            setHeaderIntValue(fhf, m_bottomOfFlash);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0xFD;
            fhf.m_fieldLength = 4;
            setHeaderIntValue(fhf, m_romChecksumType);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0xFE;
            fhf.m_fieldLength = 4;
            setHeaderIntValue(fhf, m_fwLength);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0xFA;
            fhf.m_fieldLength = (byte)5;
            setHeaderDateValue(fhf, m_LastModifiedBy);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0x92;
            fhf.m_fieldLength = (byte)m_immobilizerID.Length;
            setHeaderString(fhf, m_immobilizerID);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0x93;
            fhf.m_fieldLength = (byte)m_ecuhardwarenr.Length;
            setHeaderString(fhf, m_ecuhardwarenr);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0xF8;
            fhf.m_fieldLength = 2;
            setHeaderSmallIntValue(fhf, m_valueF8);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0xF7;
            fhf.m_fieldLength = 2;
            setHeaderSmallIntValue(fhf, m_valueF7);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0xF6;
            fhf.m_fieldLength = 2;
            setHeaderSmallIntValue(fhf, m_valueF6);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0xF5;
            fhf.m_fieldLength = 2;
            setHeaderSmallIntValue(fhf, m_valueF5);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0x90;
            fhf.m_fieldLength = (byte)m_chassisID.Length;
            setHeaderString(fhf, m_chassisID);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0x99;
            fhf.m_fieldLength = (byte)m_testserialnr.Length;
            setHeaderString(fhf, m_testserialnr);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0x98;
            fhf.m_fieldLength = (byte)m_enginetype.Length;
            setHeaderString(fhf, m_enginetype);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;

            fhf.m_fieldID = (byte)0xF9;
            fhf.m_fieldLength = 1;
            fhf.m_data = new byte[1];
            fhf.m_data.SetValue(m_RomChecksumError, 0);
            writeFieldIncludingDetails(fs, fhf);
            //fs.Position = writePos;
            fs.Close();
            Console.WriteLine("New header created");

        }

        private void ClearFooter(string a_filename)
        {
            if (!File.Exists(a_filename))
                return;
            FileStream fs = new FileStream(a_filename, FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(0x07FE00, SeekOrigin.Begin);
            int length = (int)fs.Length - (int)fs.Position;
            for (int i = 0; i < length ; i++)
            {
                fs.WriteByte((byte)0xFF);
            }
            fs.Close();
            Console.WriteLine("Footer cleared");
        }


        /// <summary>
        /// This method tranforms the data of a FileheaderField to a string.
        /// </summary>
        /// <param name="a_fileHeaderField">The FileHeaderField.</param>
        /// <returns>A string representing the information in the FileHeaderField.</returns>
        private string getHeaderString(FileHeaderField a_fileHeaderField)
        {
            Encoding ascii = Encoding.ASCII;
            ascii.GetChars(a_fileHeaderField.m_data, 0, a_fileHeaderField.m_fieldLength);
            return ascii.GetString(a_fileHeaderField.m_data, 0, a_fileHeaderField.m_fieldLength);
        }

        /// <summary>
        /// This method sets the data in a FileHeaderField to the values given by a string.
        /// </summary>
        /// <param name="a_fileHeaderField">The FileHeaderField.</param>
        /// <param name="a_string">The string to set.</param>
        private void setHeaderString(FileHeaderField a_fileHeaderField, string a_string)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(a_string);
            a_fileHeaderField.m_data = bytes;
        }

        /// <summary>
        /// This method transforms the information from a four byte field to a integer.
        /// </summary>
        /// <param name="a_fileHeaderField">The FileHeaderField.</param>
        /// <returns>The integer contained in the FileHeaderField.</returns>
        private int getHeaderIntValue(FileHeaderField a_fileHeaderField)
        {
            int intValue = 0;
            intValue |= a_fileHeaderField.m_data[0];
            intValue <<= 8;
            intValue |= a_fileHeaderField.m_data[1];
            intValue <<= 8;
            intValue |= a_fileHeaderField.m_data[2];
            intValue <<= 8;
            intValue |= a_fileHeaderField.m_data[3];
            return intValue;
        }

        private int getHeaderSmallIntValue(FileHeaderField a_fileHeaderField)
        {
            int intValue = 0;
            intValue |= a_fileHeaderField.m_data[0];
            intValue <<= 8;
            intValue |= a_fileHeaderField.m_data[1];
            return intValue;
        }

        /// <summary>
        /// This method sets the information in a four byte field to represent a integer value.
        /// </summary>
        /// <param name="a_fileHeaderField">The FileHeaderField.</param>
        /// <param name="a_value">The value that the field should contain.</param>
        private void setHeaderSmallIntValue(FileHeaderField a_fileHeaderField, int a_value)
        {
            a_fileHeaderField.m_data[1] = (byte)a_value;
            a_value >>= 8;
            a_fileHeaderField.m_data[0] = (byte)a_value;
        }

        private byte getHeaderByteValue(FileHeaderField a_fileHeaderField)
        {
            byte bValue = 0;
            bValue |= a_fileHeaderField.m_data[0];
            return bValue;
        }

        private byte[] getHeaderDateValue(FileHeaderField a_fileHeaderField)
        {
            byte[] value = new byte[5];
            value = a_fileHeaderField.m_data;
            return value;
        }

        private void setHeaderDateValue(FileHeaderField a_fileHeaderField, byte[] a_value)
        {
            a_fileHeaderField.m_data = a_value;
        }

        /// <summary>
        /// This method sets the information in a four byte field to represent a integer value.
        /// </summary>
        /// <param name="a_fileHeaderField">The FileHeaderField.</param>
        /// <param name="a_value">The value that the field should contain.</param>
        private void setHeaderIntValue(FileHeaderField a_fileHeaderField, int a_value)
        {
            a_fileHeaderField.m_data[3] = (byte) a_value;
            a_value >>= 8;
            a_fileHeaderField.m_data[2] = (byte) a_value;
            a_value >>= 8;
            a_fileHeaderField.m_data[1] = (byte) a_value;
            a_value >>= 8;
            a_fileHeaderField.m_data[0] = (byte) a_value;
        }

        /// <summary>
        /// This method consumes the file header and returns a new FileHeaderField each
        /// time it is called until all fields has been consumed. If the last field has been
        /// read a FileHeaderField with ID=0xFF is returned.
        /// </summary>
        /// <param name="a_fileStream">The FileStream to read from.</param>
        /// <returns>A FileHeaderField.</returns>
        private FileHeaderField readField(FileStream a_fileStream)
        {
            FileHeaderField fhf = new FileHeaderField();
            a_fileStream.Position -= 1;
            fhf.m_fieldLength = (byte) a_fileStream.ReadByte();
            a_fileStream.Position -= 2;
            fhf.m_fieldID = (byte) a_fileStream.ReadByte();
            if (fhf.m_fieldID == 0xFF)
                return fhf;
            for (int i = 0; i < fhf.m_fieldLength; i++)
            {
                a_fileStream.Position -= 2;
                fhf.m_data[i] = (byte)a_fileStream.ReadByte();

            }
            a_fileStream.Position -= 1;
            return fhf;
        }

        /// <summary>
        /// This method writes a FileHeaderField to the file header.
        /// </summary>
        /// <param name="a_fileStream">The FileStream to write to.</param>
        /// <param name="a_fhf">The FileHeaderField.</param>
        private void writeField(FileStream a_fileStream, FileHeaderField a_fhf)
        {
            a_fileStream.Position -= 3;         // Skip ID and length
            for (int i = 0; i < a_fhf.m_fieldLength; i++)
            {
                a_fileStream.WriteByte(a_fhf.m_data[i]);
                a_fileStream.Position -= 2;

            }
            a_fileStream.Position += 1;
        }

        private void writeFieldIncludingDetails(FileStream a_fileStream, FileHeaderField a_fhf)
        {
            a_fileStream.Position -= 1;
            // write length
            a_fileStream.WriteByte(a_fhf.m_fieldLength);
            a_fileStream.Position -= 2;
            // write ID
            a_fileStream.WriteByte(a_fhf.m_fieldID);
            a_fileStream.Position -= 2;         // Skip ID and length
            for (int i = 0; i < a_fhf.m_fieldLength; i++)
            {
                a_fileStream.WriteByte(a_fhf.m_data[i]);
                a_fileStream.Position -= 2;

            }
            a_fileStream.Position += 1;
        }

        /// <summary>
        /// Get chassis ID (VIN).
        /// </summary>
        /// <returns>Chassis ID.</returns>
        public string getChassisID()
        {
            return m_chassisID;
        }

        /// <summary>
        /// Set chassis ID (VIN).
        /// </summary>
        /// <param name="a_string">The chassis ID to write.</param>
        public void setChassisID(string a_string)
        {
             m_chassisID = a_string;
        }

        /// <summary>
        /// Get immobilizer ID.
        /// </summary>
        /// <returns>Immobilizer ID.</returns>
        public string getImmobilizerID()
        {
            return m_immobilizerID;
        }

        /// <summary>
        /// Get vehicle ID nr.
        /// </summary>
        /// <returns>Vehicle ID nr.</returns>
        public string getVehicleIDNr()
        {
            return m_vehicleidnr;
        }

        /// <summary>
        /// Set vehicle ID nr.
        /// </summary>
        /// <param name="a_string">Vehicle ID nr.</param>
        public void setVehicleIDNr(string a_string)
        {
            m_vehicleidnr = a_string;
        }


        /// <summary>
        /// Set immobilizer ID.
        /// </summary>
        /// <param name="a_string">Immobilizer ID.</param>
        public void setImmobilizerID(string a_string)
        {
            m_immobilizerID = a_string; 
        }

        /// <summary>
        /// Get software version.
        /// </summary>
        /// <returns>Software version.</returns>
        public string getSoftwareVersion()
        {
            return m_softwareVersion;
        }

        /// <summary>
        /// Set software version.
        /// </summary>
        /// <param name="a_string">Software version.</param>
        public void setSoftwareVersion(string a_string)
        {
            m_softwareVersion = a_string;
        }

        /// <summary>
        /// Get car description.
        /// </summary>
        /// <returns>Car descrption.</returns>
        public string getCarDescription()
        {
            return m_carDescription;
        }

        public string getPartNumber()
        {
            return m_partNumber;
        }

        /// <summary>
        /// Set car description.
        /// </summary>
        /// <param name="a_string">Car description.</param>
        public void setCarDescription(string a_string)
        {
            m_carDescription = a_string;
        }

        /// <summary>
        /// Get checksum F2.
        /// </summary>
        /// <returns>Checksum F2.</returns>
        public int getChecksumF2()
        {
            return m_checksumF2;
        }

        /// <summary>
        /// Set checksum F2.
        /// </summary>
        /// <param name="a_checksum">Checksum F2.</param>
        public void setChecksumF2(int a_checksum)
        {
            m_checksumF2 = a_checksum;
        }

        /// <summary>
        /// Set checksum FB.
        /// </summary>
        /// <param name="a_checksum">Checksum FB.</param>
        public void setChecksumFB(int a_checksum)
        {
            m_checksumFB = a_checksum;
        }

        /// <summary>
        /// Get checksum FB.
        /// </summary>
        /// <returns>Checksum FB.</returns>
        public int getChecksumFB()
        {
            return m_checksumFB;
        }

        /// <summary>
        /// Get firmware length (useful size of FW).
        /// </summary>
        /// <returns>Firmware length.</returns>
        public int getFWLength()
        {
            return m_fwLength;
        }
    }



}
