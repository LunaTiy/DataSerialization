using System.Text;

namespace SerializationDeserialization.Extensions;

public static class DataExtensions
{
    private static readonly UTF8Encoding _encoding = new(true);
    public static byte[] ToBytes<T>(this T data) =>
        _encoding.GetBytes(data!.ToString()!);
}