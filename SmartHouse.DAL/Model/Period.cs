using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model
{
    [Serializable]
    public class Period
    {
        public DateTime Start { set; get; }
        public DateTime End { set; get; }

        public Period(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public Period() : this(DateTime.Now, DateTime.Now)
        {
            
        }

        public TimeSpan Duration()
        {
            return End - Start;
        }

        public void MoveTo(DateTime date)
        {
            var duration = Duration();
            Start = new DateTime(Start.Year, Start.Month, Start.Day, date.Hour, date.Minute, date.Second);
            End = Start.AddSeconds(duration.TotalSeconds);
        }

        public TimeSpan Distance(Period period)
        {
            TimeSpan start = ToTimeSpan(Start);
            TimeSpan end = ToTimeSpan(End);
            TimeSpan periodStart = ToTimeSpan(period.Start);
            TimeSpan periodEnd = ToTimeSpan(period.End);
            if (start > periodEnd) return start - periodEnd;
            if (end < periodStart) return periodStart - end;
            return start - start;
        }

        public static TimeSpan ToTimeSpan(DateTime date)
        {
            return new TimeSpan(0, date.Hour, date.Minute, date.Second);
        }

        public static Period SetZone(TimeSpan start, TimeSpan end)
        {
            return new Period(new DateTime(2012, 10, 1, start.Hours, start.Minutes, start.Seconds), 
                new DateTime(2012, 10, 1, end.Hours, end.Minutes, end.Seconds));
        }

        public bool IsIntersect(Period period)
        {
            return (Start >= period.Start && Start <= period.End) ||
                   (End >= period.Start && End <= period.End) ||
                   (period.Start >= Start && period.Start <= End);
        }
    }
}
