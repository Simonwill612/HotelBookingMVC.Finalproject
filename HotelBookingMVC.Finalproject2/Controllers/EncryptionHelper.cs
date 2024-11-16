using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    private static readonly byte[] _encryptionKey = Encoding.UTF8.GetBytes("your-encryption-key-here");
    private static readonly byte[] _encryptionIV = Encoding.UTF8.GetBytes("your-encryption-iv-here");

    public static byte[] EncryptData(string plainText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = _encryptionKey;
            aes.IV = _encryptionIV;
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (var ms = new System.IO.MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new System.IO.StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                }
                return ms.ToArray();
            }
        }
    }

    public static string DecryptData(byte[] cipherText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = _encryptionKey;
            aes.IV = _encryptionIV;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (var ms = new System.IO.MemoryStream(cipherText))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var sr = new System.IO.StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}
