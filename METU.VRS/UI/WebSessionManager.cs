using System;
using System.Web;

namespace METU.VRS.UI
{
    public static class WebSessionManager
    {
        public static T GetFromSession<T>(String key, T defaultValue = default(T))
        {
            if (HttpContext.Current != null)
            {
                var value = HttpContext.Current.Session[key] ?? defaultValue;
                return (T)value;
            }
            else
            {
                return defaultValue;
            }

        }

        public static void SetToSession<T>(String key, T value)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session[key] = value;
            }
        }
    }
}