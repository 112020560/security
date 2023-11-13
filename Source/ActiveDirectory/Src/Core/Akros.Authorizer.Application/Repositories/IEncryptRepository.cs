namespace Akros.Authorizer.Application.Repositories;

public interface IEncryptRepository
{
    string EncryptString(string text, string keyString);
    string DecryptString(string cipherText, string keyString);
}
