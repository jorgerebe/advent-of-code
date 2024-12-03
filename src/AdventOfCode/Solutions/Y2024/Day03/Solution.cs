using System.Text.RegularExpressions;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions.Y2024.Day03;

public class Solution(IOptions<AppSettings> options,IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string text = _fileReader.Read(GetFullFilePath(fileName));

        string pattern = @"mul\(\d{1,3}\,\d{1,3}\)";

        IList<string> matches = Regex.Matches(text, pattern).Select(m => m.Value).ToList();

        return matches.Sum(Multiply);
    }

    public override long GetSolution_2(string fileName)
    {
        string text = _fileReader.Read(GetFullFilePath(fileName));

        string pattern = @"do\(\)|don\'t\(\)|mul\(\d{1,3}\,\d{1,3}\)";

        IList<string> matches = Regex.Matches(text, pattern).Select(m => m.Value).ToList();

        bool enabled = true;

        long output = 0;

        foreach (string match in matches)
        {
            if (match == "do()")
            {
                enabled = true;
            }
            else if (match == "don't()")
            {
                enabled = false;
            }
            else if (enabled)
            {
                output += Multiply(match);
            }
        }

        return output;
    }

    private static long Multiply(string match)
    {
        return match.Substring(4, match.Length - 5).Split(',').Select(long.Parse).Aggregate((a, b) => a * b);
    }
}
