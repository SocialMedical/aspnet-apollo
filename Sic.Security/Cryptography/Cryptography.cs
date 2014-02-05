using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Sic.Security
{
    public partial class Cryptography
    {                        
        private static byte[] GetBytes(string text)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            return ByteConverter.GetBytes(text);
        }

        private static string GetString(byte[] bytes)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            return ByteConverter.GetString(bytes);
        }

        public static string Encrypt(String plaintext)
        {            
            return Encrypt(KeyFileName,plaintext);
        }

        public static string Decrypt(String plaintext)
        {            
            return Decrypt(KeyFileName,plaintext);
        }

        public static string Encrypt(string keyfileName, String plaintext)
        {
            string key = DecryptKeyFromFile(keyfileName);
            return Encrypt(plaintext,GetBytes(key), GetBytes(bi));
        }

        public static string Decrypt(string keyfileName, String plaintext)
        {
            string key = DecryptKeyFromFile(keyfileName);
            return Decrypt(plaintext, GetBytes(key), GetBytes(bi));
        }

        public static string Encrypt(String text, byte[] Key, byte[] IV)
        {            
            Rijndael RijndaelAlg = Rijndael.Create();             
            MemoryStream memoryStream = new MemoryStream();            
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         RijndaelAlg.CreateEncryptor(Key, IV),
                                                         CryptoStreamMode.Write);            
            byte[] plainMessageBytes = UTF8Encoding.UTF8.GetBytes(text);            
            cryptoStream.Write(plainMessageBytes, 0, plainMessageBytes.Length);
            cryptoStream.FlushFinalBlock();            
            byte[] cipherMessageBytes = memoryStream.ToArray();              
            memoryStream.Close();
            cryptoStream.Close();            
            return Convert.ToBase64String(cipherMessageBytes);
        }

        public static string Decrypt(String text, byte[] Key, byte[] IV)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(text);            
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];            
            Rijndael RijndaelAlg = Rijndael.Create();            
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);            
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         RijndaelAlg.CreateDecryptor(Key, IV),
                                                         CryptoStreamMode.Read);            
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);            
            memoryStream.Close();
            cryptoStream.Close();            
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        public static string GenerateKey()
        {
            Rijndael RijndaelAlg = Rijndael.Create();
            RijndaelAlg.KeySize = 256;
            RijndaelAlg.IV = GetBytes(bi);
            RijndaelAlg.GenerateKey();            

            return GetString(RijndaelAlg.Key);            
        }

        public static Rijndael InitRijndael()
        {
            Rijndael RijndaelAlg = Rijndael.Create();
            RijndaelAlg.KeySize = 256;
            RijndaelAlg.Key = GetBytes(fl);
            RijndaelAlg.IV = GetBytes(bi);
            return RijndaelAlg;
        }

        public static void EncryptToFile(String plainMessage, string fileName)
        {
            Rijndael RijndaelAlg = InitRijndael();
            EncryptToFile(plainMessage,fileName,RijndaelAlg.Key,RijndaelAlg.IV);
        }

        public static void EncryptToFile(String plainMessage, String filename, byte[] Key, byte[] IV)
        {
            FileStream fileStream = System.IO.File.Open(filename, FileMode.OpenOrCreate);            
            Rijndael RijndaelAlg = Rijndael.Create(); 
            
            CryptoStream cryptoStream = new CryptoStream(fileStream,
                                                         RijndaelAlg.CreateEncryptor(Key, IV),
                                                         CryptoStreamMode.Write);            
            StreamWriter streamWriter = new StreamWriter(cryptoStream);
            streamWriter.WriteLine(plainMessage);            
            streamWriter.Close();
            cryptoStream.Close();
            fileStream.Close();
        }

        public static string DecryptKeyFromFile(String filename)
        {
            Rijndael RijndaelAlg = InitRijndael();
            return DecryptKeyFromFile(filename, RijndaelAlg.Key, RijndaelAlg.IV);
        }

        public static string DecryptKeyFromFile(String filename, byte[] Key, byte[] IV)
        {
            if (!System.IO.File.Exists(filename))
                throw new Exception("File Key no found");

            FileStream fileStream = System.IO.File.Open(filename, FileMode.OpenOrCreate);
            Rijndael RijndaelAlg = Rijndael.Create();
            CryptoStream cryptoStream = new CryptoStream(fileStream,
                                                         RijndaelAlg.CreateDecryptor(Key, IV),
                                                         CryptoStreamMode.Read);

            StreamReader streamReader = new StreamReader(cryptoStream);
            string plainMessage = streamReader.ReadLine();
            streamReader.Close();
            cryptoStream.Close();
            fileStream.Close();
            return plainMessage;
        }                

        private static CspParameters GetKeyFromContainer(string ContainerName)
        {            
            CspParameters cp = new CspParameters();            
            cp.KeyContainerName = ContainerName;
            return cp;            
        }

        private static void DeleteKeyFromContainer(string ContainerName)
        {
            // Create the CspParameters object and set the key container 
            // name used to store the RSA key pair.
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = ContainerName;

            // Create a new instance of RSACryptoServiceProvider that accesses
            // the key container.
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);

            // Delete the key entry in the container.
            rsa.PersistKeyInCsp = false;

            // Call Clear to release resources and delete the key from the container.
            rsa.Clear();

            Console.WriteLine("Key deleted.");
        }
    }
}
