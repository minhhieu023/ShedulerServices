using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShedulerServices.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShedulerServices.Common;
using Quartz;
using ShedulerServices.Model;

namespace ShedulerServices
{
    [DisallowConcurrentExecution]

    public class TestJob : IJob
    {
        private readonly ILogger<TestJob> _logger;
        private readonly TestDbContext  _context;
        private readonly IWebHostEnvironment _env;
        public TestJob(ILogger<TestJob> logger, TestDbContext context, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _env = env;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            
            var jobList = _context.TestSchedules.Where(x => x.LastRun.Day != DateTime.Now.Day).ToList();
            
             Console.WriteLine(DateTime.Now);
            foreach(var job in jobList)
            {
                if (job.ScheduleDateTime.ToString("dd/mm/yyyy HH:mm") == DateTime.Now.ToString("dd/mm/yyyy HH:mm") 
                    && job.LastRun.ToString("dd/mm/yyyy") != DateTime.Now.ToString("dd/mm/yyyy"))
                {
                    var guid = Guid.NewGuid();
                    string fileName = job.ProjectName  + job.FunctionName  + guid;
                    string path =String.Format( @".\FileScheduler\TestFile\{0}.csv", fileName );
                    var reports = String.Format(@".\FileScheduler\Report\" +"report_{0}.html", guid.ToString());
                    var logs = String.Format(@".\FileScheduler\Logs\" + "logs_{0}", guid.ToString());
                    try
                    {       
                        // Create the file, or overwrite if the file exists.
                        using (FileStream fs = File.Create(path))
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(job.FilePath);
                            // Add some information to the file.
                            fs.Write(info, 0, info.Length);
                        }
                       
                        DateTime startDate = DateTime.Now;
                        Console.WriteLine("Run test");
                        CommandProccess.Command(path,reports,logs);
                        DateTime endDate = DateTime.Now;
                        //Update row in database
                        var updateJob = await _context.TestSchedules.FindAsync(job.Id);

                        if(updateJob != null)
                        {
                            updateJob.LastRun = endDate;

                            updateJob.ScheduleDateTime = updateJob.ScheduleDateTime.AddDays(1);
                            updateJob.LastRun = DateTime.Now;
                            var update = _context.TestSchedules.Update(updateJob);
                            if (update.State == EntityState.Modified)
                                await _context.SaveChangesAsync();
                            CommandProccess.SendEmail(job.NotifyEmail, fileName, job.ProjectName, job.FunctionName, startDate, endDate, reports);
                        }
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }    
                 
            }
          
        }
    }
}
