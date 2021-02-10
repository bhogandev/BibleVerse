using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BibleVerseAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            } 
            catch(Exception ex)
            {
                // Do something with exception here
                try
                {
                    if (!EventLog.SourceExists("BV.Exceptions", "."))
                    {
                        EventLog.CreateEventSource("BV.Exceptions", "Application");
                    }
                }catch(Exception x)
                {
                    EventLog.CreateEventSource("BV.Exceptions", "Application");
                }

                EventLog.WriteEntry("BV.Exceptions", ex.Message, EventLogEntryType.Error);
            }
            finally
            {
                // Last operation
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseStartup<Startup>();
                });
    }
}
