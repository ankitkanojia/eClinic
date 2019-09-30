using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AyurvedOnCall.Helpers
{
    public static class Utilities
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime LocalToUtcTime(string timeZone, DateTime inputvalue)
        {
            if (string.IsNullOrWhiteSpace(timeZone)) return inputvalue;

            var timeZoneDetails = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            inputvalue = Convert.ToDateTime(inputvalue).Subtract(timeZoneDetails.BaseUtcOffset);
            return inputvalue;
        }

        public static DateTime UtcToLocalTime(string timeZone, DateTime inputvalue)
        {
            if (string.IsNullOrWhiteSpace(timeZone)) return inputvalue;

            var timeZoneDetails = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            inputvalue = Convert.ToDateTime(inputvalue).Add(timeZoneDetails.BaseUtcOffset);
            return inputvalue;
        }

        public static void CopyProperties<TSelf, TSource>(this TSelf self, TSource source)
        {
            var sourceAllProperties = source.GetType().GetProperties();

            foreach (var sourceProperty in sourceAllProperties)
            {
                var selfProperty = self.GetType().GetProperty(sourceProperty.Name);
                if (selfProperty == null) continue;
                var sourceValue = sourceProperty.GetValue(source, null);
                selfProperty.SetValue(self, sourceValue, null);
            }
        }

    }
}