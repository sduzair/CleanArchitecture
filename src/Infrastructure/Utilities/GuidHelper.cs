using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Utilities;

public static class GuidHelper
{
    public static Guid GenerateDeterministicGuid(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        var guidBytes = new byte[16];
        Array.Copy(hash, guidBytes, 16);
        guidBytes[7] = (byte)((guidBytes[7] & 0x0F) | 0x50); // Set the version to 5.
        guidBytes[8] = (byte)((guidBytes[8] & 0x3F) | 0x80); // Set the variant.
        return new Guid(guidBytes);
    }
}
