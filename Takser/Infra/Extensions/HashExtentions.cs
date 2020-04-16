using System.Security.Cryptography;
using System.Text;

namespace Takser.Infra.Extensions
{
    public static class HashExtentions
    {
        public static string ToSha256(this string password)
        {
            using SHA256 sha256Algo = SHA256.Create();
            byte[] hashedPassword = sha256Algo.ComputeHash(Encoding.ASCII.GetBytes(password));
            return Encoding.ASCII.GetString(hashedPassword);
        }
    }
}