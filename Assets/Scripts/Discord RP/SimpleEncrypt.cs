using System;
using System.Collections;
using System.Linq;

/// <summary>
/// Very inefficient and insecure way of encrypting data,
/// but it is at least, some way of encrypting data.
/// Not getting paid anyway.
/// </summary>
public class SimpleEncrypt
{
    private readonly BitArray key;
    
    public SimpleEncrypt(string key)
    {
        this.key = new BitArray(ToByteArray(key));
    }
    
    private static byte[] ToByteArray(string text)
    {
        var charArray= text.ToCharArray();

        return charArray.Select(Convert.ToByte).ToArray();
    }

    /// <summary>
    /// Encrypts some data in string form.
    /// </summary>
    /// <param name="text">String to encrypt</param>
    /// <returns>Encrypted data</returns>
    public byte[] Encrypt(string text)
    {
        var bytes = ToByteArray(text);
        var bits = new BitArray(bytes);
        var encryptedResult = new byte[text.Length];

        bits.Xor(key).CopyTo(encryptedResult, 0);
        
        return encryptedResult;
    }

    /// <summary>
    /// Decrypts some data that has been previously encrypted with this very class.
    /// </summary>
    /// <param name="mess">Encrypted data</param>
    /// <returns>Decrypted string</returns>
    public string Decrypt(byte[] mess)
    {
        var bits = new BitArray(mess);
        var decryptedBytes = new byte[mess.Length];

        bits.Xor(key).CopyTo(decryptedBytes, 0);

        var chars = decryptedBytes.Select(Convert.ToChar).ToArray();

        return new string(chars);
    }
}