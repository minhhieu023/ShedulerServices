using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShedulerServices.Model
{
    public class TestSchedules
    {
        public int Id { get; set; }
        public String FilePath { get; set; }
        public String FunctionName { get; set; }
        public String ProjectName { get; set; }
        public String NotifyEmail { get; set; }
        public TimeSpan RunTime { get; set; } 
        public int RunType { get; set; } 
        public int FunctionTestingId { get; set; }
        public DateTime LastRun { get; set; }
        public DateTime ScheduleDateTime { get; set; }
    }
}
