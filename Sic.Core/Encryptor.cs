using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Sic.Security.Encryptor
{
    public class Services
    {
        /// <summary>
        /// Determines the default text encoding.
        /// </summary>
        public static System.Text.Encoding DefaultEncoding = System.Text.Encoding.ASCII;
        /// <summary>
        /// Determines the text encoding used by latin american countries.
        /// </summary>

        public static System.Text.Encoding LatinEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");
        public static string Encrypt(string textToBeEncrypted)
        {
            return Encrypt(textToBeEncrypted, Constants.Security.DefaultEncryptionKey);
        }

        public static string Encrypt(string textToBeEncrypted, string encryptionKey)
        {
            return Encrypt(textToBeEncrypted, encryptionKey, new byte[] {
				121,
				241,
				10,
				1,
				132,
				74,
				11,
				39,
				255,
				91,
				45,
				78,
				14,
				211,
				22,
				62
			}, DefaultEncoding);
        }

        public static string Encrypt(string textToBeEncrypted, string encryptionKey, byte[] initializationVector, System.Text.Encoding encoding)
        {

            byte[] bytValue = null;
            byte[] bytKey = null;
            byte[] bytEncoded = { 0 };
            byte[] bytIV = initializationVector;
            int intLength = 0;
            int intRemaining = 0;
            MemoryStream objMemoryStream = new MemoryStream();
            CryptoStream objCryptoStream = default(CryptoStream);
            RijndaelManaged objRijndaelManaged = default(RijndaelManaged);

            if (encoding == null)
                encoding = DefaultEncoding;

            //   **********************************************************************
            //   ******  Strip any null character from string to be encrypted    ******
            //   **********************************************************************

            textToBeEncrypted = StripNullCharacters(textToBeEncrypted);
            bytValue = encoding.GetBytes(textToBeEncrypted.ToCharArray());

            //   ********************************************************************
            //   ******   Encryption Key must be 256 bits long (32 bytes)      ******
            //   ******   If it is longer than 32 bytes it will be truncated.  ******
            //   ******   If it is shorter than 32 bytes it will be padded     ******
            //   ******   with upper-case Xs.                                  ****** 
            //   ********************************************************************

            encryptionKey = GetHashKey(encryptionKey);
            intLength = encryptionKey.Length;

            if (intLength >= 32)
            {
                encryptionKey = Microsoft.VisualBasic.Strings.Left(encryptionKey, 32);
            }
            else
            {
                intLength = encryptionKey.Length;
                intRemaining = 32 - intLength;
                encryptionKey = encryptionKey + Microsoft.VisualBasic.Strings.StrDup(intRemaining, "X");
            }

            bytKey = Encoding.ASCII.GetBytes(encryptionKey.ToCharArray());

            objRijndaelManaged = new RijndaelManaged();

            //   ***********************************************************************
            //   ******  Create the encryptor and write value to it after it is   ******
            //   ******  converted into a byte array                              ******
            //   ***********************************************************************


            try
            {
                objCryptoStream = new CryptoStream(objMemoryStream, objRijndaelManaged.CreateEncryptor(bytKey, bytIV), CryptoStreamMode.Write);
                objCryptoStream.Write(bytValue, 0, bytValue.Length);

                objCryptoStream.FlushFinalBlock();

                bytEncoded = objMemoryStream.ToArray();
                objMemoryStream.Close();
                objCryptoStream.Close();

            }
            catch
            {
            }

            //   ***********************************************************************
            //   ******   Return encryptes value (converted from  byte Array to   ******
            //   ******   a base64 string).  Base64 is MIME encoding)             ******
            //   ***********************************************************************

            return Convert.ToBase64String(bytEncoded).Replace(" ", "+");

        }

        private static string GetHashKey(string originalKey)
		{
			string[] baseArray = originalKey.Split('0');
			Array.Reverse(baseArray);
			string returnValue = "";
			//string value = null;
			foreach (string value in baseArray) {
				returnValue += value;
			}
			return returnValue;
		}

        public static string Decrypt(string textToBeDecrypted)
        {
            return Decrypt(textToBeDecrypted, Constants.Security.DefaultEncryptionKey);
        }

        public static string Decrypt(string textToBeDecrypted, string encryptionKey)
        {
            return Decrypt(textToBeDecrypted, encryptionKey, new byte[] {
				121,
				241,
				10,
				1,
				132,
				74,
				11,
				39,
				255,
				91,
				45,
				78,
				14,
				211,
				22,
				62
			}, DefaultEncoding);
        }

        public static string Decrypt(string textToBeDecrypted, string decryptionKey, byte[] initializationVector, System.Text.Encoding encoding)
        {
            byte[] bytDataToBeDecrypted = null;
            byte[] bytTemp = null;
            byte[] bytIV = initializationVector;
            RijndaelManaged objRijndaelManaged = new RijndaelManaged();
            MemoryStream objMemoryStream = default(MemoryStream);
            CryptoStream objCryptoStream = default(CryptoStream);
            byte[] bytDecryptionKey = null;

            int intLength = 0;
            int intRemaining = 0;
            string strReturnString = string.Empty;

            if (encoding == null)
                encoding = DefaultEncoding;

            //   *****************************************************************
            //   ******   Convert base64 encrypted value to byte array      ******
            //   *****************************************************************
            bytDataToBeDecrypted = Convert.FromBase64String(textToBeDecrypted.Replace(" ", "+"));

            //   ********************************************************************
            //   ******   Encryption Key must be 256 bits long (32 bytes)      ******
            //   ******   If it is longer than 32 bytes it will be truncated.  ******
            //   ******   If it is shorter than 32 bytes it will be padded     ******
            //   ******   with upper-case Xs.                                  ****** 
            //   ********************************************************************
            decryptionKey = GetHashKey(decryptionKey);
            intLength = decryptionKey.Length;

            if (intLength >= 32)
            {
                decryptionKey = decryptionKey.Substring(0, 32);
            }
            else
            {
                intLength = decryptionKey.Length;
                intRemaining = 32 - intLength;
                decryptionKey = decryptionKey + Microsoft.VisualBasic.Strings.StrDup(intRemaining, "X");
            }

            bytDecryptionKey = Encoding.ASCII.GetBytes(decryptionKey.ToCharArray());

            bytTemp = new byte[bytDataToBeDecrypted.Length];

            objMemoryStream = new MemoryStream(bytDataToBeDecrypted);

            //   ***********************************************************************
            //   ******  Create the decryptor and write value to it after it is   ******
            //   ******  converted into a byte array                              ******
            //   ***********************************************************************


            try
            {
                objCryptoStream = new CryptoStream(objMemoryStream, objRijndaelManaged.CreateDecryptor(bytDecryptionKey, bytIV), CryptoStreamMode.Read);

                objCryptoStream.Read(bytTemp, 0, bytTemp.Length);

                objCryptoStream.FlushFinalBlock();
                objMemoryStream.Close();
                objCryptoStream.Close();


            }
            catch
            {
            }

            //   *****************************************
            //   ******   Return decypted value     ******
            //   *****************************************

            return StripNullCharacters(encoding.GetString(bytTemp));

        }

        private static string StripNullCharacters(string stringWithNulls)
        {

            int intPosition = 0;
            string strStringWithOutNulls = null;

            intPosition = 1;
            strStringWithOutNulls = stringWithNulls;

            while (intPosition > 0)
            {
                intPosition = Microsoft.VisualBasic.Strings.InStr(intPosition, stringWithNulls, Microsoft.VisualBasic.Constants.vbNullChar);

                if (intPosition > 0)
                {
                    strStringWithOutNulls = Microsoft.VisualBasic.Strings.Left(strStringWithOutNulls, intPosition - 1) + Microsoft.VisualBasic.Strings.Right(strStringWithOutNulls, Microsoft.VisualBasic.Strings.Len(strStringWithOutNulls) - intPosition);                   
                }

                if (intPosition > strStringWithOutNulls.Length)
                {
                    break; // TODO: might not be correct. Was : Exit Do
                }
            }

            return strStringWithOutNulls;
        }
    }
}

namespace Sic.Constants
{
    public sealed class Security
    {
        public static string DefaultUser
        {
            get { return "sa"; }
        }

        public static string DefaultPassword
        {
            get { return "12345678"; }
        }

        public static string InitSession
        {
            get { return "INITSESSION"; }
        }

        public static int DefaultTimeout
        {
            get { return 120; }
        }

        public static string DefaultEncryptionKey
        {
            get { return "17026011075072006028017"; }
        }
    }
}