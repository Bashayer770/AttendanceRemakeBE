using AttendanceRemake.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Transactions;

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
        public List<AttendanceResponse> CalculateLate(List<Models.Attendance> attendance, TimingPlan TimePlan)
        {
            TimeSpan inTime = TimeSpan.Parse(TimePlan.FromTime);
            TimeSpan outTime = TimeSpan.Parse(TimePlan.ToTime);
            var attendanceLog = GetUserActivity(attendance, TimePlan);
            List<AttendanceResponse> result = new List<AttendanceResponse>();
            foreach(var a in attendanceLog)
            {
                result.Add(new AttendanceResponse
                {
                    Day = a.Date,
                    In = (TimeSpan)GetActualInTime(a.Signs),
                    Excuses = GetDuringWorkLeaveTime(a.Signs),
                    Out = (TimeSpan)GetActualLeave(a.Signs)

                });
                //result.Add(GetActualInTime(a.Signs));
                //result.Add(GetDuringWorkLeaveTime(a.Signs));
                //result.Add(GetActualLeave(a.Signs));
                
            }
            return result;
        }

        private TimeSpan? GetActualInTime(List<string> signs)
        {
            for(var i =0; i<signs.Count; i++)
            {
                var transaction = signs[i].Split("Tr");
                if (Int32.Parse(transaction[1]) == 0)
                {
                    return TimeSpan.Parse(transaction[0]);
                }
            }
            return null;
        }
        private TimeSpan? GetActualLeave(List<string> signs)
        {
            for (var i = signs.Count - 1; i >= 0; i--)
            {
                var transaction = signs[i].Split("Tr");
                if (Int32.Parse(transaction[1]) == 1)
                {
                    return TimeSpan.Parse(transaction[0]);
                }
            }
            return null;
        }
        private List<TimeSpan[]> GetDuringWorkLeaveTime(List<string> signs)
        {
            var ActualInTime = GetActualInTime(signs);
            var ActualOutTime = GetActualLeave(signs);
            List<TimeSpan[]> Excuses = new List<TimeSpan[]>();
            var excuseList = signs.Where( 
                sign => TimeSpan.Parse(sign.Split("Tr")[0]) > ActualInTime &&
                        TimeSpan.Parse(sign.Split("Tr")[0]) < ActualOutTime).ToList();
            string[] container = ["", ""];
            for (int i = 0; i<excuseList.Count; i++)
            {

                if(i+1 != excuseList.Count)
                {
                    var current = excuseList[i].Split("Tr");
                    var next = excuseList[i + 1].Split("Tr");
                    if (container[0] == "")
                    {
                        if (Int32.Parse(current[1]) == 1) // && Int32.Parse(next[1]) == 1
                        {
                            container[0] = current[0];
                        }
                    }

                    if(Int32.Parse(current[1]) == 0)//  && Int32.Parse(next[1]) == 1
                    {
                        container[1] = current[0];
                    }
                    //else
                    //{
                    //    container[1] = current[0];
                    //}
                    if (!container.Contains(""))
                    {
                        Excuses.Add(
                            [TimeSpan.Parse(container[0]), TimeSpan.Parse(container[1])]
                            );
                        container = ["", ""];
                    }
                }

            }
            return Excuses;
            
        }

        public List<Deductions> CalculateDeductions(List<Models.Attendance> attendance, TimingPlan TimePlan)
        {
            TimeSpan inTime = TimeSpan.Parse(TimePlan.FromTime);
            TimeSpan outTime = TimeSpan.Parse(TimePlan.ToTime);

            List<AttendanceResponse> Lates = CalculateLate(attendance,TimePlan);
            List<Deductions> deductions = new List<Deductions>();
            foreach (var row in Lates)
            {
                Deductions deduct = new Deductions();
                deduct.Day = row.Day;

                if (row.In != null)
                {
                    if (inTime < row.In )
                    {
                        deduct.Late = (int)(row.In - inTime).TotalMinutes;
                    }
                    
                }
                else
                {
                    deduct.ForgetIn = 5;
                }
                if (row.Out != null)
                {
                    if (outTime > row.Out)
                    {
                        deduct.Late = (int)(outTime - row.Out).TotalMinutes;
                    }

                }
                else
                {
                    deduct.ForgetOut = 5;
                }
                if (row.Excuses.Any())
                {
                    foreach (var excuse in row.Excuses)
                    {
                        int duringWorkExcuse =  (int)(excuse[1] - excuse[0]).TotalMinutes;
                        if(duringWorkExcuse < 15)
                        {
                            deduct.DuringWorkExcuse += duringWorkExcuse;
                        }
                    }
                }
                deductions.Add(deduct);
            }
            return deductions;
        }
    }

    public class AttendanceLog
    {
        public DateOnly Date { get; set; }
        public List<string>? Signs { get; set; }
    }
    public class AttendanceResponse
    {
        public DateOnly Day { get; set; }
        public TimeSpan In { get; set; }
        public TimeSpan Out {  get; set; }
        public List<TimeSpan[]> Excuses { get; set; }
    }
    public class Deductions
    {
        public DateOnly Day { get; set; }
        public int Late { get; set; } = 0;
        public int EndWorkExcuse { get; set; } = 0;
        public int DuringWorkExcuse { get; set; } = 0;
        public int ForgetIn { get; set; } = 0;
        public int ForgetOut { get; set; } = 0;
    }
}
