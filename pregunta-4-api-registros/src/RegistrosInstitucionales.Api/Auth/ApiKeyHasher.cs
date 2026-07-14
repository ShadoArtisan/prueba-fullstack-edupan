using System.Security.Cryptography;
using System.Text;

namespace RegistrosInstitucionales.Api.Auth;

// Las API Keys nunca se guardan en texto plano; solo se compara el hash SHA-256.
public static class ApiKeyHasher
{
    public static string Hash(string apiKey)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToHexString(bytes);
    }
}
