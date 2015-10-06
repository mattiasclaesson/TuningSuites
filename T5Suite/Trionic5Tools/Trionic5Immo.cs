using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Security.Cryptography;



namespace Trionic5Tools
{
    public class Trionic5Immo
    {
        public bool ImmoValid(string HWID)
        {
            string immo = GetValueFromRegistry("ImmoID");
            if (immo != "")
            {
                try
                {
                    string decodedimmo = DecodeImmo(immo);
                    if (decodedimmo == HWID) return true;
                }
                catch (Exception decodeE)
                {
                    Console.WriteLine("d: " + decodeE.Message);
                }
            }
            // TEST
            //Console.WriteLine("Valid ID would be:" + EncodeImmo(HWID));
            return false;
        }

        private string EncodeImmo(string immo)
        {
            return EncryptString(immo, this.GetType().ToString());
        }

        private string DecodeImmo(string codedimmo)
        {
            // decode the string
            return DecryptString(codedimmo, this.GetType().ToString());
        }

        public static string EncryptString(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }

        public static string DecryptString(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(Message);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }

        private string GetValueFromRegistry(string symbolname)
        {
            RegistryKey TempKey = null;
            string retval = "";
            bool overruleCode = false;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");
            using (RegistryKey Settings = TempKey.CreateSubKey("T5SuitePro"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            if (a == symbolname)
                            {
                                retval = Settings.GetValue(a).ToString();
                            }
                            if (a == "EnableAll")                   //<GS-25032010> als we all licenties willen laten verlopen dan moet
                                                                    // een nieuwe versie deze registersleutel aanmaken.
                                                                    // oudere versies willen dan ook niet meer.
                            {
                                overruleCode = true;
                            }
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                    }
                }
            }
            if (overruleCode) retval = "";
            return retval;
        }

        private string SaveValueToRegistry(string symbolname, string value)
        {
            RegistryKey TempKey = null;
            string retval = "";
            TempKey = Registry.CurrentUser.CreateSubKey("Software");
            using (RegistryKey Settings = TempKey.CreateSubKey("T5SuitePro"))
            {
                if (Settings != null)
                {
                    Settings.SetValue(symbolname, value);
                }
            }
            return retval;
        }

        public void SaveLicense(string code)
        {
            SaveValueToRegistry("ImmoID", code);
        }
    }
}
