using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Http;

namespace Presentation.Common.Extensions;

internal static class SessionExtensions
{
    // Extension methods to store and retrieve objects in session
    public static void SetObject<T>(this ISession session, string key, T value)
    {
        //string jsonString = JsonSerializer.Serialize(value);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        session.Set(key, bytes);
    }

    public static T? GetObject<T>(this ISession session, string key)
    {

        //var value = session.GetString(key);
        var value = session.Get(key);
        if (value == null)
        {
            return default;
        }
        else
        {
            //T? type = JsonSerializer.Deserialize<T>(value);
            T? type = JsonSerializer.Deserialize<T>(value);
            return type;
        }
    }
}
