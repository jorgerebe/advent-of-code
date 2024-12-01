using AdventOfCode.Solutions;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace AdventOfCode.Services;

public class Runner(IOptions<AppSettings> options, IFactory<ISolution> factory) : IRunner
{
    private readonly AppSettings _appSettings = options.Value;
    private readonly IFactory<ISolution> _factory = factory;

    public void Execute(int year, int day, int solution, bool test)
    {
        ISolution dayInstance;
        try
        {
            string fullName = _appSettings.SolutionFullName
                .Replace("{year}", year.ToString())
                .Replace("{day}", day.ToString("D2"));
            dayInstance = _factory.CreateInstance(fullName);
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"There are no solutions for Day {day} in Year {year}");
            return;
        }

        Console.WriteLine($"Solutions for Day {day} in Year {year}:");

        if (solution > 0)
        {
            // Run the specified solution method
            RunSolution(dayInstance, solution, test);
        }
        else
        {
            // Run both solutions if available
            RunSolution(dayInstance, 1, test);
            RunSolution(dayInstance, 2, test);
        }
    }

    private void RunSolution(ISolution dayInstance, int solutionNum, bool test)
    {
        string prefixInputFile = test ? _appSettings.InputTestFilePrefix : _appSettings.InputFilePrefix;

        string inputFile = $"{prefixInputFile}{_appSettings.InputFileSuffix}";
        string methodName = $"{_appSettings.MethodPrefix}{solutionNum}";

        MethodInfo? method = dayInstance.GetType().GetMethod(methodName);
        if (method != null)
        {
            try
            {
                long result = (long)method.Invoke(dayInstance, [inputFile])!;
                Console.WriteLine($"Solution {solutionNum} Output: {result}");
            }
            catch (TargetInvocationException tie)
            {
                switch (tie.InnerException)
                {
                    case DirectoryNotFoundException or FileNotFoundException:
                        Console.WriteLine($"Input file '{inputFile}' not found");
                        break;
                    case NotImplementedException:
                        Console.WriteLine($"Solution {solutionNum} is not implemented");
                        break;
                    default:
                        Console.WriteLine($"Error running Solution {solutionNum}: {tie.InnerException?.Message}");
                        break;
                }
            }
        }
    }
}
