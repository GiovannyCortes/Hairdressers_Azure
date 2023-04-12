using Hairdressers_Azure.Helpers;

namespace Hairdressers_Azure.Extensions {
    public static class SessionExtension {

        public static T GetObject<T>(this ISession session, string key) {
            string json = session.GetString(key);
            if (json == null) {
                return default;
            } else {
                T data = HelperJson.DeserializeObject<T>(json);
                return data;
            }
        }

        public static void SetObject(this ISession session, string key, object value) {
            string data = HelperJson.SerializeObject(value);
            session.SetString(key, data);
        }

    }
}
