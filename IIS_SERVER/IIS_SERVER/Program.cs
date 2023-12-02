/**
* @file Program.cs
* author { Martin Kubicka (xkubic45) }
* @date 17.12.2023
* @brief Main startup file where things are initialized
*/


using Serilog;
using IIS_SERVER;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args).UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
        