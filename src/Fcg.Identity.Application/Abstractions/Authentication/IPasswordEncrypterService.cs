namespace Fcg.Identity.Application.Abstractions.Authentication;

public interface IPasswordEncrypterService
{
    string Encrypt(string password);
    bool IsValid(string password, string passwordHash);
}
