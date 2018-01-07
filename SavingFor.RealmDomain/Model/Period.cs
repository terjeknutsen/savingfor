using System;

namespace SavingFor.RealmDomain.Model
{
    public struct Period
    {
        public Period(DateTimeOffset start, DateTimeOffset end)
        {
            Start = start;
            End = end;
        }

        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }
        public bool IsActive => new DateTimeOffset(DateTime.Now) < End;

        public double MinutesLeft => !IsActive ? 0 : (End.DateTime - DateTime.Now).TotalMinutes;

        public double TotalMinutes => (End.DateTime - Start.DateTime).TotalMinutes;

        public double TotalMinutesWithinMonth(DateTimeOffset dateTime)
        {
            if (dateTime.DateTime < Start.DateTime && dateTime.Year < Start.Year || (dateTime.Year == Start.Year && dateTime.Month < Start.Month))
                return 0d;

            if (dateTime.Year == End.Year && dateTime.Month > End.Month)
                return 0d;
            if (dateTime.Year > End.Year)
                return 0d;

            if (End.Year == Start.Year && End.Month == Start.Month)
                return (End - Start).TotalMinutes;

            var total = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            var lastDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, total, 23, 59, 59);
            if (Start.Year == dateTime.Year && Start.Month == dateTime.Month)
                return (lastDayOfMonth - Start).TotalMinutes;

            var firstDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
            if (End.Year == dateTime.Year && End.Month == dateTime.Month)
                return (End - firstDayOfMonth).TotalMinutes;
            return (lastDayOfMonth - firstDayOfMonth).TotalMinutes;
        }

        public double MinutesPassed
        {
            get
            {
                double minutesPassed;
                CalculateMinutesPassed(out minutesPassed);
                return minutesPassed;
            }
        }

        private void CalculateMinutesPassed(out double value)
        {
            var minutesPassed =

                !IsActive ?
                (End - Start).TotalMinutes :
                (DateTime.Now - Start).TotalMinutes;
            value = minutesPassed < 1 ? 1 : minutesPassed;
        }
    }
}
