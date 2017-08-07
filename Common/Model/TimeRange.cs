using System;
using System.Data.SqlTypes;
using System.Runtime.Serialization;

namespace Common.Model
{
    /// <summary>
    /// Class for storing and end and a start date and comparing these
    /// </summary>
    [DataContract]
    public class TimeRange : IEquatable<TimeRange>
    {
        /// <summary>
        /// Start Date of the TimeRange
        /// </summary>
        [DataMember]
        public DateTime Start { get; }

        /// <summary>
        /// End Date of the TimeRange
        /// </summary>
        [DataMember]
        public DateTime End { get; }

        /// <summary>
        /// Define Readonly Properys at the start
        /// </summary>
        public TimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gives back the TimeSpan implementation: End - Start
        /// </summary>
        public TimeSpan TimeSpan => End - Start;

        /// <summary>
        /// Time range from minimal possible sqldatetime to now
        /// </summary>
        public static TimeRange Always => new TimeRange(SqlDateTime.MinValue.Value, DateTime.UtcNow);

        /// <summary>
        /// Calculates time range from given days to now
        /// </summary>
        /// <param name="days">given days you want to go back in time !</param>
        public static TimeRange FromDays(int days)
            => new TimeRange(DateTime.UtcNow - TimeSpan.FromDays(days), DateTime.UtcNow);

        /// <summary>
        /// Calculates time range from given months to now
        /// </summary>
        /// <param name="months">given months you want to go back in time !</param>
        public static TimeRange FromMonths(int months)
            => new TimeRange(DateTime.UtcNow - TimeSpan.FromDays(months * 28), DateTime.UtcNow);

        /// <summary>
        /// Calculates time range from given months to now
        /// </summary>
        /// <param name="weeks">given weeks you want to go back in time !</param>
        public static TimeRange FromWeeks(int weeks)
            => new TimeRange(DateTime.UtcNow - TimeSpan.FromDays(weeks * 7), DateTime.UtcNow);

        /// <summary>
        /// Calculates time range from given date to the duration of days after
        /// </summary>
        /// <param name="date">Date the Timerange will start</param>
        /// <param name="days">Length of the timerange</param>=
        public static TimeRange FromDaysFromDate(DateTime date, int days)
            => new TimeRange(date - TimeSpan.FromDays(days), date);

        /// <summary>
        /// Checks if given DateTime is between end and start date of timerange object
        /// </summary>
        /// <param name="dateTime">DateTime which will be checked if in between</param>
        /// <returns>true for is in between</returns>
        public bool IsBetween(DateTime? dateTime) => dateTime.HasValue
                                                     && dateTime.Value.Ticks >= Start.Ticks &&
                                                     dateTime.Value.Ticks <= End.Ticks;

        /// <summary>
        /// Checks if given DateTime is between end and start date of timerange object
        /// </summary>
        /// <param name="dateTime">DateTime which will be checked if in between</param>
        /// <returns>true for is in between</returns>
        public bool IsBetween(DateTime dateTime) => dateTime.Ticks >= Start.Ticks &&
                                                    dateTime.Ticks <= End.Ticks;

        /// <summary>
        /// ToString Implementation
        /// </summary>
        /// <returns>gives back start and end date in "d" format</returns>
        public override string ToString() => Start.ToString("dd/MM") + " - " + End.ToString("dd/MM");

        #region IEquatable Implementation

        public override bool Equals(object value)
        {
            if (value == null) return false;
            var timeRange = value as TimeRange;
            return timeRange != null && Equals(timeRange);
        }

        public bool Equals(TimeRange value)
        {
            if (value == null) return false;
            return value.Start == Start && value.End == End;
        }

        public static bool operator ==(TimeRange timeRange1, TimeRange timeRange2)
        {
            if ((object) timeRange1 == null
                || (object) timeRange2 == null) return Equals(timeRange1, timeRange2);
            return timeRange1.Equals(timeRange2);
        }

        public static bool operator !=(TimeRange timeRange1, TimeRange timeRange2)
        {
            if ((object) timeRange1 == null
                || (object) timeRange2 == null) return !Equals(timeRange1, timeRange2);
            return !(timeRange1.Equals(timeRange2));
        }

        public override int GetHashCode() => Start.GetHashCode() ^ End.GetHashCode();

        #endregion
    }
}