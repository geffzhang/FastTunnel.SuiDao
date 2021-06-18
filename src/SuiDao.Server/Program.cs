﻿using FastTunnel.Core.Extensions;
using FastTunnel.Core.Global;
using FastTunnel.Core.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;

namespace SuiDao.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // -------------------FastTunnel START------------------
                    services.AddFastTunnelServer();
                    // -------------------FastTunnel EDN--------------------
                    FastTunnelGlobal.AddCustomHandler<IConfigHandler, SuiDaoConfigHandler>(new SuiDaoConfigHandler());
                })
                .ConfigureLogging((HostBuilderContext context, ILoggingBuilder logging) =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    //ILoggingBuilder.AddFilter("System", LogLevel.Warning);
                    //ILoggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                    //ILoggingBuilder.AddLog4Net();
                });
                //.UseNLog();
    }
}
