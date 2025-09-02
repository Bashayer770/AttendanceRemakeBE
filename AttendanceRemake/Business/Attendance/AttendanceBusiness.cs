using AttendanceRemake.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRemake.Business.Attendance
{
    public class AttendanceBusiness
    {
        public Dictionary<DateTime, List<int>> LateCalcMonth(List<AttendanceRemake.Models.Attendance> AttendanceList, TimingPlan TimePlan)
        {
            List<AttendanceRemake.Models.Attendance> holder = new List<Models.Attendance>();
            Dictionary<DateTime, List<int>> lateRecord = new Dictionary<DateTime, List<int>>();

            TimeSpan inTime = TimeSpan.Parse(TimePlan.FromTime);
            TimeSpan outTime = TimeSpan.Parse(TimePlan.ToTime);

            foreach(var row in AttendanceList)
            {
                if (holder.IsNullOrEmpty())
                {
                    holder.Add(row);
                    Console.WriteLine(holder);
                    continue;
                }
                if ((holder[holder.Count - 1].IodateTime.Date.AddDays(1)) == row.IodateTime.Date)
                {
                    if (!lateRecord.ContainsKey(row.IodateTime.Date))
                    {
                        lateRecord[row.IodateTime.Date] = new List<int>();
                        lateRecord[row.IodateTime.Date].Add(5);
                    }
                    else
                    {
                        lateRecord[row.IodateTime.Date].Add(5);
                    }

                    holder.Clear();
                    holder.Add(row);
                    Console.WriteLine(holder);
                }
                if ( row.IodateTime.Date == holder[holder.Count - 1].IodateTime.Date)
                {
                    holder.Add(row);
                    if (holder[holder.Count-1].TrType == 1 && holder[holder.Count - 2].TrType == 0)
                    {
                        if (!lateRecord.ContainsKey(row.IodateTime.Date))
                        {
                            lateRecord[row.IodateTime.Date] = new List<int>();
                            lateRecord[row.IodateTime.Date].Add(LateCalc(holder[holder.Count - 2].IodateTime.TimeOfDay, holder[holder.Count - 1].IodateTime.TimeOfDay, inTime, outTime));
                            Console.WriteLine(holder);
                        }
                        else
                        {
                            lateRecord[row.IodateTime.Date].Add(LateCalc(holder[holder.Count - 2].IodateTime.TimeOfDay, holder[holder.Count - 1].IodateTime.TimeOfDay, inTime, outTime));
                            Console.WriteLine(holder);
                        }

                            holder.Clear();
                    }
                }
                if(holder.Count == 1)
                {
                    holder.Clear();
                    Console.WriteLine(holder);
                }
 
            }
            return lateRecord;
            
        }

        public int LateCalc(TimeSpan signInTime,TimeSpan signOutTime, TimeSpan inTime, TimeSpan outTime )
        {
            int resultLate = 0;
            if(signInTime > inTime)
            {
                resultLate += (int)(signInTime - inTime).TotalMinutes;
            }
            if (signOutTime < outTime)
            {
                resultLate += (int)(outTime - signOutTime).TotalMinutes;
            }
            return resultLate;
        }
        public IEnumerable<AttendanceLog> GetUserActivity(List<AttendanceRemake.Models.Attendance> AttendanceList, TimingPlan TimePlan)
        {

            TimeSpan inTime = TimeSpan.Parse(TimePlan.FromTime);
            TimeSpan outTime = TimeSpan.Parse(TimePlan.ToTime);

            var sorted = AttendanceList.GroupBy(row => row.IodateTime.Date)
                .Select(row => new AttendanceLog
                {
                    Date = DateOnly.Parse(row.Key.ToString().Split(' ')[0]),
                    Signs = row.Select(x => new String(x.IodateTime.TimeOfDay.ToString()+"Tr" + x.TrType.ToString())).ToList<string>()
                }).ToList<AttendanceLog>();

            return sorted;
        }
    }

    public class AttendanceLog
    {
        public DateOnly Date { get; set; }
        public List<string>? Signs { get; set; }
    }
}
