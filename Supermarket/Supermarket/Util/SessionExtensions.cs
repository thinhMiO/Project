using Newtonsoft.Json;

namespace Supermarket.Util
{
    public static class SessionExtensions
    {
        //public static void Set<T>(this ISession session, string key, T value)
        //{
        //    session.SetString(key, JsonSerializer.Serialize(value));
        //}

        //public static T? Get<T>(this ISession session, string key)
        //{
        //    var value = session.GetString(key);
        //    return value == null ? default : JsonSerializer.Deserialize<T>(value);
        //}

        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
