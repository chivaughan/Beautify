using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Beautify
{
    public class MyAppSecurity
    {
        #region MD5
        public static string HashMD5(string phrase)
        {

            if (phrase == null)
                return null;
            var encoder = new UTF8Encoding();
            var md5Hasher = new MD5CryptoServiceProvider();
            var hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(phrase));
            return ByteArrayToHexString(hashedDataBytes);
        }
        #endregion
        #region SHA
        public static string HashSHA1(string phrase)
        {

            if (phrase == null)
                return null;
            var encoder = new UTF8Encoding();
            var sha1Hasher = new SHA1CryptoServiceProvider();
            var hashedDataBytes = sha1Hasher.ComputeHash(encoder.GetBytes(phrase));
            return ByteArrayToHexString(hashedDataBytes);
        }
        public static string HashSHA256(string phrase)
        {

            if (phrase == null)
                return null;
            var encoder = new UTF8Encoding();
            var sha256Hasher = new SHA256CryptoServiceProvider();
            var hashedDataBytes = sha256Hasher.ComputeHash(encoder.GetBytes(phrase));
            return ByteArrayToHexString(hashedDataBytes);
        }
        public static string HashSHA384(string phrase)
        {

            if (phrase == null)
                return null;
            var encoder = new UTF8Encoding();
            var sha384Hasher = new SHA384CryptoServiceProvider();
            var hashedDataBytes = sha384Hasher.ComputeHash(encoder.GetBytes(phrase));
            return ByteArrayToHexString(hashedDataBytes);
        }
        public static string HashSHA512(string phrase)
        {

            if (phrase == null)
                return null;
            var encoder = new UTF8Encoding();
            var sha512Hasher = new SHA512CryptoServiceProvider();
            var hashedDataBytes = sha512Hasher.ComputeHash(encoder.GetBytes(phrase));
            return ByteArrayToHexString(hashedDataBytes);
        }
        #endregion
        /*
    #region AES
    public static string EncryptAES(string phrase, string key, bool hashKey = true)
    {

        if (phrase == null || key == null)
            return null;
        var keyArray = HexStringToByteArray(hashKey ? HashMD5(key) : key);
        var toEncryptArray = Encoding.UTF8.GetBytes(phrase);
        byte[] result;
        using (var aes = new AesCryptoServiceProvider
        {

            Key = keyArray,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        })
        {

            var cTransform = aes.CreateEncryptor();
            result = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            aes.Clear();
        }
        return ByteArrayToHexString(result);
    }
    public static string DecryptAES(string hash, string key, bool hashKey = true)
    {

        if (hash == null || key == null)
            return null;
        var keyArray = HexStringToByteArray(hashKey ? HashMD5(key) : key);
        var toEncryptArray = HexStringToByteArray(hash);
        var aes = new AesCryptoServiceProvider
        {

            Key = keyArray,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };
        var cTransform = aes.CreateDecryptor();
        var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        aes.Clear();
        return Encoding.UTF8.GetString(resultArray);
    }
    #endregion
    #region 3DES
    public static string EncryptTripleDES(string phrase, string key, bool hashKey = true)
    {

        var keyArray = HexStringToByteArray(hashKey ? HashMD5(key) : key);
        var toEncryptArray = Encoding.UTF8.GetBytes(phrase);
        var tdes = new TripleDESCryptoServiceProvider
        {

            Key = keyArray,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };
        var cTransform = tdes.CreateEncryptor();
        var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        tdes.Clear();
        return ByteArrayToHexString(resultArray);
    }
    public static string DecryptTripleDES(string hash, string key, bool hashKey = true)
    {

        var keyArray = HexStringToByteArray(hashKey ? HashMD5(key) : key);
        var toEncryptArray = HexStringToByteArray(hash);
        var tdes = new TripleDESCryptoServiceProvider
        {

            Key = keyArray,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };
        var cTransform = tdes.CreateDecryptor();
        var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        tdes.Clear();
        return Encoding.UTF8.GetString(resultArray);
    }
    #endregion
     * */
        #region Helpers
        internal static string ByteArrayToHexString(byte[] inputArray)
        {

            if (inputArray == null)
                return null;
            var o = new StringBuilder("");
            for (var i = 0; i < inputArray.Length; i++)
                o.Append(inputArray[i].ToString("X2"));
            return o.ToString();
        }
        internal static byte[] HexStringToByteArray(string inputString)
        {

            if (inputString == null)
                return null;
            if (inputString.Length == 0)
                return new byte[0];
            if (inputString.Length % 2 != 0)
                throw new Exception("Hex strings have an even number of characters and you have got an odd number of characters!");
            var num = inputString.Length / 2;
            var bytes = new byte[num];
            for (var i = 0; i < num; i++)
            {

                var x = inputString.Substring(i * 2, 2);
                try
                {

                    bytes[i] = Convert.ToByte(x, 16);
                }
                catch (Exception ex)
                {

                    throw new Exception("Part of your \"hex\" string contains a non-hex value.", ex);
                }
            }
            return bytes;
        }
        #endregion

        #region GenerateUniqueCode
        internal static string GenerateUniqueAlphaNumericString(int length)
        {
            string a = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            char[] chars = new char[a.Length];
            chars = a.ToCharArray();
            byte[] data = new byte[length];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            string generatedString;
            StringBuilder strGeneratedString = new StringBuilder();
            // Generate the unique string
            foreach (byte b in data)
            {
                strGeneratedString.Append(chars[b % (chars.Length - 1)]);
            }
            // The generated unique alpahnumeric string         
            generatedString = strGeneratedString.ToString();
            // Return the generated string
            return generatedString;
        }
        #endregion

        #region Password Bytes for Encryption and Decryption
        // This method gets the bytes of the password used for encryption and decryption
        public static byte[] GetPasswordBytes()
        {
            // The real password characters is stored in System.SecureString
            // Below code demonstrates on converting System.SecureString into Byte[]
            // Credit: http://social.msdn.microsoft.com/Forums/vstudio/en-US/f6710354-32e3-4486-b866-e102bb495f86/converting-a-securestring-object-to-byte-array-in-net

            byte[] ba = null;

            //if (secureTextBox1.Text.Length == 0)
            ba = new byte[] { 75, 42, 87, 74, 21, 14, 7, 26 };
            /*else
            {
                // Convert System.SecureString to Pointer
                IntPtr unmanagedBytes = Marshal.SecureStringToGlobalAllocAnsi(secureTextBox1.SecureText);
                try
                {
                    // You have to mark your application to allow unsafe code
                    // Enable it at Project's Properties > Build
                    unsafe
                    {
                        byte* byteArray = (byte*)unmanagedBytes.ToPointer();

                        // Find the end of the string
                        byte* pEnd = byteArray;
                        while (*pEnd++ != 0) { }
                        // Length is effectively the difference here (note we're 1 past end) 
                        int length = (int)((pEnd - byteArray) - 1);

                        ba = new byte[length];

                        for (int i = 0; i < length; ++i)
                        {
                            // Work with data in byte array as necessary, via pointers, here
                            byte dataAtIndex = *(byteArray + i);
                            ba[i] = dataAtIndex;
                        }
                    }
                }
                finally
                {
                    // This will completely remove the data from memory
                    Marshal.ZeroFreeGlobalAllocAnsi(unmanagedBytes);
                }
            }*/

            return System.Security.Cryptography.SHA256.Create().ComputeHash(ba);
        }
        #endregion

        #region AES from Codeproject
        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            byte[] saltBytes = passwordBytes;
            // Example:
            //saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC; // This is the recommended mode

                    using (CryptoStream cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;
            // Set your salt here to meet your flavor:
            byte[] saltBytes = passwordBytes;
            // Example:
            //saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC; // This is the recommended mode

                    using (CryptoStream cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public static string Encrypt(string text, byte[] passwordBytes)
        {
            byte[] originalBytes = Encoding.UTF8.GetBytes(text);
            byte[] encryptedBytes = null;

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            // Getting the salt size
            int saltSize = GetSaltSize(passwordBytes);
            // Generating salt bytes
            byte[] saltBytes = GetRandomBytes(saltSize);

            // Appending salt bytes to original bytes
            byte[] bytesToBeEncrypted = new byte[saltBytes.Length + originalBytes.Length];
            for (int i = 0; i < saltBytes.Length; i++)
            {
                bytesToBeEncrypted[i] = saltBytes[i];
            }
            for (int i = 0; i < originalBytes.Length; i++)
            {
                bytesToBeEncrypted[i + saltBytes.Length] = originalBytes[i];
            }

            encryptedBytes = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string decryptedText, byte[] passwordBytes)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(decryptedText);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] decryptedBytes = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            // Getting the size of salt
            int saltSize = GetSaltSize(passwordBytes);

            // Removing salt bytes, retrieving original bytes
            byte[] originalBytes = new byte[decryptedBytes.Length - saltSize];
            for (int i = saltSize; i < decryptedBytes.Length; i++)
            {
                originalBytes[i - saltSize] = decryptedBytes[i];
            }

            return Encoding.UTF8.GetString(originalBytes);
        }

        public static int GetSaltSize(byte[] passwordBytes)
        {
            var key = new Rfc2898DeriveBytes(passwordBytes, passwordBytes, 1000);
            byte[] ba = key.GetBytes(2);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ba.Length; i++)
            {
                sb.Append(Convert.ToInt32(ba[i]).ToString());
            }
            int saltSize = 0;
            string s = sb.ToString();
            foreach (char c in s)
            {
                int intc = Convert.ToInt32(c.ToString());
                saltSize = saltSize + intc;
            }

            return saltSize;
        }

        public static byte[] GetRandomBytes(int length)
        {
            byte[] ba = new byte[length];
            RNGCryptoServiceProvider.Create().GetBytes(ba);
            return ba;
        }


        #endregion
    }
}