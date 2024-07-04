namespace Infrastructure.Shared.Services.Abstractions;

// https://damienbod.com/2020/08/19/symmetric-and-asymmetric-encryption-in-net-core/
// https://www.c-sharpcorner.com/article/encryption-and-decryption-using-a-symmetric-key-in-c-sharp/
// https://vmsdurano.com/-net-core-3-1-signing-jwt-with-rsa/
// https://newbedev.com/csharp-c-rsa-encrypt-with-public-key-code-example
public interface ISymmetricEncryptionService
{
  string EncryptData(string plainText, string encryptionKey);

  string DecryptData(string cipherText, string encryptionKey);
}
