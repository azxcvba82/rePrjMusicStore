using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using StackExchange.Redis;

namespace Music_store_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //加载日志配置信息文件后去捕获所有的错误
            //var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            //try
            //{
            //    logger.Info("Init Log API Information");
                CreateHostBuilder(args).Build().Run();
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex, "Stop Log Information Because Of Exception");
            //}
            //finally
            //{
            //    LogManager.Shutdown();
            //}
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        //.ConfigureLogging(logging =>
        //{
        //    logging.ClearProviders();//移除其它已经注册的日志处理程序
        //    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);//记录最小日志级别
        //})
        //.UseNLog();//注入 NLog 服务;

    }
}
