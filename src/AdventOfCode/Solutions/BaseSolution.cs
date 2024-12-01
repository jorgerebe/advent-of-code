using System.Reflection;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public abstract class BaseSolution(IOptions<AppSettings> options, IFileReader fileReader) : ISolution
{
    protected string SolutionPath
    {
        get
        {
            // Get the year and day from the namespace (e.g., Y2024.Day01)
            string? namespaceName = GetType().Namespace;

            // Example: "AdventOfCode.Solutions.Y2024.Day01"
            string[]? parts = namespaceName?.Split('.');

            // Assuming the format is "AdventOfCode.Solutions.YYYYY.DD"
            if (parts != null && parts.Length >= 4)
            {
                // Construct the correct path like: Data\Y2024\Day01
                string year = parts[2];   // "Y2024"
                string day = parts[3];    // "Day01"
                string solutionFolder = Path.Combine(options.Value.DataDirectory, year, day);

                // Combine with the base directory for the assembly
                string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(assemblyPath!, solutionFolder);
            }
            else
            {
                throw new DirectoryNotFoundException("Invalid namespace format");
            }
        }
    }

    protected readonly IFileReader _fileReader = fileReader;

    public abstract long GetSolution_1(string fileName);
    public abstract long GetSolution_2(string fileName);

    protected string GetFullFilePath(string fileName)
    {
        return Path.Combine(SolutionPath, fileName);
    }
}
