using AdventOfCode.Services;
using AdventOfCode.Solutions;
using AdventOfCode.Solutions.Y2024.Day01;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace AdventOfCode;

internal class Program
{
    private static void Main(string[] args)
    {
        var rootCommand = new RootCommand("Advent of Code Solution Runner")
        {
            new Option<int?>(
                aliases: ["--year", "-y"],
                description: "Specify the year. Defaults to the current year."
            ),
            new Option<int?>(
                aliases: ["--day", "-d"],
                description: "Specify the day number. Defaults to today's date."
            ),
            new Option<int?>(
                aliases: ["--solution", "-s"],
                description: "Specify the solution number (1 or 2). If omitted, runs both solutions."
            ),
            new Option<bool>(
                aliases: ["--test", "-t"],
                description: "Run solutions with test input files."
            )
        };

        rootCommand.Handler = CommandHandler.Create<int?, int?, int?, bool>((year, day, solution, test) =>
        {
            IHost host = CreateHostBuilder(args).Build();
            IRunner runner = host.Services.GetRequiredService<IRunner>();
            runner.Execute(year ?? DateTime.Now.Year, day ?? DateTime.Now.Day, solution ?? 0, test);
        });

        rootCommand.InvokeAsync(args);
    }

    static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<AppSettings>(context.Configuration.GetRequiredSection(nameof(AppSettings)));

                services.AddScoped<IFileReader, FileReader>();
                services.AddSingleton<IFactory<ISolution>, Factory<ISolution>>();
                services.AddTransient<IRunner, Runner>();
            });
    }
}
