using Infrastructure.Shared.Extensions;
using Infrastructure.Shared.Services.Abstractions;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Shared.Services.Implementations;

public class SymmetricEncryptionService : ISymmetricEncryptionService
{
  private readonly ILogger<ISymmetricEncryptionService> _logger;

  public SymmetricEncryptionService(ILogger<ISymmetricEncryptionService> logger)
  {
    _logger = logger;
  }

  public string EncryptData(string plainText, string encryptionKey)
  {
    try
    {
      var secretKey = Encoding.UTF8.GetBytes(encryptionKey);
      // var iv = new byte[16];
      var iv = Encoding.UTF8.GetBytes("8080808080808080");
      byte[] array;

      using (var aes = Aes.Create())
      {
        aes.Key = secretKey;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.FeedbackSize = 128;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using (var memoryStream = new MemoryStream())
        {
          using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
          {
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
              streamWriter.Write(plainText);
            }

            array = memoryStream.ToArray();
          }
        }
      }

      return Convert.ToBase64String(array);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex.GetFullMessage());
      return string.Empty;
    }
  }

  public string DecryptData(string cipherText, string encryptionKey)
  {
    try
    {
      var secretKey = Encoding.UTF8.GetBytes(encryptionKey);
      // var iv = new byte[16];
      var iv = Encoding.UTF8.GetBytes("8080808080808080");
      var buffer = Convert.FromBase64String(cipherText);

      using (var aes = Aes.Create())
      {
        aes.Key = secretKey;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.FeedbackSize = 128;
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using (var memoryStream = new MemoryStream(buffer))
        {
          using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
          {
            using (var streamReader = new StreamReader(cryptoStream))
            {
              return streamReader.ReadToEnd();
            }
          }
        }
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex.GetFullMessage());
      return string.Empty;
    }
  }
}
