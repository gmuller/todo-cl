using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Todo
{
    class DateParser
    {
        /// <summary>
        /// Parses the given date/time string with
        /// the ability to recognize relative day words
        /// </summary>
        public static DateTime? Parse(string input)
        {
            // default to null
            DateTime? result;

            // is it yesterday, today or tomorrow?
            result = ParseRelativeDay(input);

            // is it a day of the week?
            if (result == null)
                result = ParseDayOfWeekDate(input);

            // ugh. just parse it with DateTime.
            if (result == null)
            {
                DateTime dt;
                if (DateTime.TryParse(input, out dt))
                    result = dt;
            }

            return result;
        }

        private static Dictionary<string, int> _relativeDayWords;
        protected static Dictionary<string, int> RelativeDayWords
        {
            get
            {
                if (_relativeDayWords == null)
                {
                    _relativeDayWords = new Dictionary<string, int>();

                    _relativeDayWords.Add("today", 0);
                    _relativeDayWords.Add("tod", 0);
                    _relativeDayWords.Add("yesterday", -1);
                    _relativeDayWords.Add("tomorrow", 1);
                    _relativeDayWords.Add("tom", 1);
                }

                return _relativeDayWords;
            }
        }

        public static DateTime? ParseRelativeDay(string input)
        {
            DateTime? value = null;

            string key = input.ToLower().Trim();
            if (RelativeDayWords.ContainsKey(key))
                value = DateTime.Today.AddDays(RelativeDayWords[key]);

            return value;
        }

        public enum Tense
        {
            Future,
            Past
        }

        protected static Tense GetTense(string input)
        {
            var value = Tense.Future;

            if (input.ToLower().Contains("next"))
                value = Tense.Future;
            else if (input.ToLower().Contains("last"))
                value = Tense.Past;

            return value;
        }

        protected static DayOfWeek? ParseDayOfWeek(string input)
        {
            // cleanse input
            string i = input
                .ToLower()
                .Replace("next", string.Empty)
                .Replace("last", string.Empty)
                .Trim();

            // query it for a match
            DayOfWeek? result = null;
            var dow = (from p in System.Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>()
                       where p.ToString().ToLower().StartsWith(i)
                       select p);

            if (dow.Any())
                result = dow.First();

            return result;
        }

        public static DateTime? ParseDayOfWeekDate(string input)
        {
            DateTime? result = null;

            var day = ParseDayOfWeek(input);
            if (day.HasValue)
            {
                var dir = GetTense(input);

                int daysToAdd = 0;
                int currentDayOfWeek = (int)DateTime.Today.DayOfWeek;
                int inputDayOfWeek = (int)day;
                int n;

                switch (dir)
                {
                    case Tense.Past:
                        n = (inputDayOfWeek - (7 + currentDayOfWeek));
                        daysToAdd = (n < -7) ? n % 7 : n;
                        break;

                    case Tense.Future:
                        n = (7 - currentDayOfWeek + inputDayOfWeek);
                        daysToAdd = (n > 7) ? n % 7 : n;
                        break;
                }

                result = DateTime.Today.AddDays(daysToAdd);
            }

            return result;
        }

        /// <summary>
        /// Gets a string representing the given date.
        /// If the date is within seven days of now, the
        /// result is a relative word.
        /// </summary>
        public static string ToString(DateTime value)
        {
            string result = string.Empty;

            // if the date is within 7 days, give it a friendly name
            const int daysPerWeek = 7;
            var difference = DateTime.Today - value;
            if (System.Math.Abs(difference.Days) <= daysPerWeek)
            {
                // relative
                result = (from p in RelativeDayWords
                          where DateTime.Today.AddDays(p.Value) == value
                          select p.Key).FirstOrDefault();

                // or dow
                if (string.IsNullOrEmpty(result))
                {
                    result = value.DayOfWeek.ToString();

                    if (value > DateTime.Today)
                        result = string.Format("next {0}");
                    else
                        result = string.Format("last {0}");
                }
            }

            // or native short date format
            if (string.IsNullOrEmpty(result))
                result = value.ToShortDateString();

            return result;
        }
    }
}
