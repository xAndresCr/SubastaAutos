using System.Security.Cryptography;
using System.Text;

namespace SubastaAutos.Web.Util
{
    public static class CryptoHelper
    {
        public static string HashPassword(string password, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
