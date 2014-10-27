using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace inti2008.Data.Common
{
    public static class TimeSpanExtension
    {
        
        /// <summary>
        /// Dictionary with localized names for time units
        /// </summary>
        private static readonly IDictionary<string, IDictionary<TimeUnit, string[]>> TimeUnitStrings =
            new Dictionary<string, IDictionary<TimeUnit, string[]>> { 
                            { "SE-sv", new Dictionary<TimeUnit, string[]>
                              {
                                  {TimeUnit.Year, new[]{"år", "år"}},
                                  {TimeUnit.Month, new[]{"månad", "månader"}},
                                  {TimeUnit.Day, new[]{"dag", "dagar"}},
                                  {TimeUnit.Hour, new[]{"timme", "timmar"}},
                                  {TimeUnit.Minute, new[]{"minut", "minuter"}},
                                  {TimeUnit.Second, new[]{"sekund", "sekunder"}}
                              } 
                            },
                            { "en", new Dictionary<TimeUnit, string[]>
                              {
                                  {TimeUnit.Year, new[]{"year", "years"}},
                                  {TimeUnit.Month, new[]{"month", "months"}},
                                  {TimeUnit.Day, new[]{"day", "days"}},
                                  {TimeUnit.Hour, new[]{"hour", "hours"}},
                                  {TimeUnit.Minute, new[]{"minute", "minutes"}},
                                  {TimeUnit.Second, new[]{"second", "seconds"}}
                              } 
                            },
                            { "da", new Dictionary<TimeUnit, string[]>
                              {
                                  {TimeUnit.Year, new[]{"år", "år"}},
                                  {TimeUnit.Month, new[]{"måned", "måneder"}},
                                  {TimeUnit.Day, new[]{"dag", "dage"}},
                                  {TimeUnit.Hour, new[]{"time", "timer"}},
                                  {TimeUnit.Minute, new[]{"minut", "minutter"}},
                                  {TimeUnit.Second, new[]{"sekund", "sekunder"}}
                              } 
                            },
                            { "nn", new Dictionary<TimeUnit, string[]>
                              {
                                  {TimeUnit.Year, new[]{"år", "år"}},
                                  {TimeUnit.Month, new[]{"måned", "måneder"}},
                                  {TimeUnit.Day, new[]{"dag", "dager"}},
                                  {TimeUnit.Hour, new[]{"time", "timer"}},
                                  {TimeUnit.Minute, new[]{"minutt", "minutter"}},
                                  {TimeUnit.Second, new[]{"sekund", "sekunder"}}
                              } 
                            },
                            { "no", new Dictionary<TimeUnit, string[]>
                              {
                                  {TimeUnit.Year, new[]{"år", "år"}},
                                  {TimeUnit.Month, new[]{"måned", "måneder"}},
                                  {TimeUnit.Day, new[]{"dag", "dager"}},
                                  {TimeUnit.Hour, new[]{"time", "timer"}},
                                  {TimeUnit.Minute, new[]{"minutt", "minutter"}},
                                  {TimeUnit.Second, new[]{"sekund", "sekunder"}}
                              } 
                            },
                            { "fi", new Dictionary<TimeUnit, string[]>
                              {
                                  {TimeUnit.Year, new[]{"year", "years"}},
                                  {TimeUnit.Month, new[]{"month", "months"}},
                                  {TimeUnit.Day, new[]{"day", "days"}},
                                  {TimeUnit.Hour, new[]{"hour", "hours"}},
                                  {TimeUnit.Minute, new[]{"minute", "minutes"}},
                                  {TimeUnit.Second, new[]{"second", "seconds"}}
                              } 
                            }
            };

        private const string DefaultCulture = "SE-sv";

        private enum TimeUnit
        {
            Year,
            Month,
            Day,
            Hour,
            Minute,
            Second
        }

        /// <summary>
        /// Transform a timspan to a readable localized string
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="includeSeconds"></param>
        /// <returns></returns>
        public static string ToFriendlyLocalizedString(this TimeSpan timeSpan, bool includeSeconds)
        {
            return ToFriendlyLocalizedString(timeSpan, includeSeconds, CultureInfo.CurrentCulture);
        }


        /// <summary>
        /// Transform a timspan to a readable localized string
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="includeSeconds"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static string ToFriendlyLocalizedString(this TimeSpan timeSpan, bool includeSeconds, CultureInfo cultureInfo)
        {

            const string separator = " ";

            var cultureName = cultureInfo.Name;
            //do we have this culture translated?
            if (!TimeUnitStrings.ContainsKey(cultureName))
            {
                //try with just the language string
                cultureName = cultureInfo.TwoLetterISOLanguageName;
                if (!TimeUnitStrings.ContainsKey(cultureName))
                    cultureName = DefaultCulture;
            }


            var outPutString = "";
            if (timeSpan.Days > 0)
                outPutString = outPutString.AddWithSeparator(timeSpan.Days + " " +
                                                             TimeUnitStrings[cultureName][TimeUnit.Day][
                                                                 PluralCheck(timeSpan.Days)], separator);
            if (timeSpan.Hours > 0)
                outPutString = outPutString.AddWithSeparator(timeSpan.Hours + " " +
                                                             TimeUnitStrings[cultureName][TimeUnit.Hour][
                                                                 PluralCheck(timeSpan.Hours)], separator);


            if (timeSpan.Minutes > 0)
                outPutString = outPutString.AddWithSeparator(timeSpan.Minutes + " " +
                                                             TimeUnitStrings[cultureName][TimeUnit.Minute][
                                                                 PluralCheck(timeSpan.Minutes)], separator);

            if (includeSeconds && timeSpan.Seconds > 0)
                outPutString = outPutString.AddWithSeparator(timeSpan.Seconds + " " +
                                                             TimeUnitStrings[cultureName][TimeUnit.Second][
                                                                 PluralCheck(timeSpan.Seconds)], separator);

            return outPutString;
        }

        private static string AddWithSeparator(this string input, string stringAddition, string separator)
        {
            if (String.IsNullOrEmpty(input))
                return stringAddition;

            return input + separator + stringAddition;
        }

        private static int PluralCheck(int value)
        {
            if (value == 1) return 0;

            return 1;
        }
    }
}
