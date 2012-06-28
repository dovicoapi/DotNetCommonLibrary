using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dovico.CommonLibrary
{
    public class CEncryption
    {
        // Exposed method for encrypting a string
        //
        // NOTE: Like a normal password, sPassword should be difficult to guess!
        //
        // Returns a Base64 encoded string
        public static string Encrypt(string sValue, string sPassword, string sSalt) 
        {
            // Encrypt the value passed in and then return the encrypted string as a base64 encoded value to the caller
            byte[] bValue = Crypt(Encoding.Unicode.GetBytes(sValue), sPassword, sSalt, true);
            return Convert.ToBase64String(bValue);
        }


        // Exposed method for decrypting a string
        //
        // sValue is expected to be a base64 encoded byte array
        public static string Decrypt(string sValue, string sPassword, string sSalt)
        {
            // Decrypt the base64 value passed in and then return the resulting string to the caller
            byte[] bValue = Crypt(Convert.FromBase64String(sValue), sPassword, sSalt, false);
            return Encoding.Unicode.GetString(bValue);
        }



        // Internal helper function to do the encryption/decryption
        protected static byte[] Crypt(byte[] bValue, string sPassword, string sSalt, bool bEncrypt)
        {
            byte[] bReturnVal = null;

            AesManaged aesTransform = null;
            MemoryStream msStream = null;
            CryptoStream csCryptoStream = null;

            try
            {
                // Rfc2898DeriveBytes (PBKDF2) is similar to using PasswordDeriveBytes (PBKDF1) but the algorithm is stronger apparently
                Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes(sPassword, Encoding.Unicode.GetBytes(sSalt), 1000);

                // Create our AES Key and Initialization Vector (IV) values
                aesTransform = new AesManaged();
                aesTransform.Key = pwdGen.GetBytes(aesTransform.KeySize / 8);
                aesTransform.IV = pwdGen.GetBytes(aesTransform.BlockSize / 8);

                // Depending on if we're encrypting or decrypting create the proper transform object
                ICryptoTransform ctTransform = (bEncrypt ? aesTransform.CreateEncryptor() : aesTransform.CreateDecryptor());

                // Create our memory stream and encryption/decryption stream object
                msStream = new MemoryStream();
                csCryptoStream = new CryptoStream(msStream, ctTransform, CryptoStreamMode.Write);

                // Encrypt/Decrypt the value
                csCryptoStream.Write(bValue, 0, bValue.Length);
                csCryptoStream.FlushFinalBlock();

                // Turn our encrypted/decrypted memory stream value into a byte array
                bReturnVal = msStream.ToArray();
            }
            finally
            {
                if (csCryptoStream != null) { csCryptoStream.Clear(); csCryptoStream.Close(); }
                if (msStream != null) { msStream.Close(); }
                if (aesTransform != null) { aesTransform.Clear(); }
            }


            // Return the encrypted/decrypted value
            return bReturnVal;
        }

    }
}
