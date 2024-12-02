using System.Runtime.CompilerServices;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions.Y2024.Day02;

public class Solution(IOptions<AppSettings> options,IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));
        long output = 0;

        foreach (string line in lines)
        {
            long[] level = Array.ConvertAll(line.Split(' '), long.Parse);

            bool isIncreasing = level[0] < level[1];

            if (Math.Abs(level[0] - level[1]) > 3 || level[0] == level[1])
            {
                continue;
            }

            if(IsLevelSafe(level))
            {
                output++;
            }
        }

        return output;
    }

    public override long GetSolution_2(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));
        long output = 0;

        int iToExclude;
        bool safe;

        foreach (string line in lines)
        {
            long[] level = Array.ConvertAll(line.Split(' '), long.Parse);
            iToExclude = -1;
            safe = false;

            while(iToExclude < level.Length && !safe)
            {
                safe = IsLevelSafe(level, iToExclude);
                iToExclude++;
            }

            if (safe)
            {
                output++;
            }
        }

        return output;
    }

    private static bool IsLevelSafe(long[] level, int exclude=-1)
    {
        int i = 1;
        int preI = 0;

        bool? isIncreasing = null;

        while (i < level.Length)
        {
            if(preI == exclude)
            {
                preI = i;
                i++;
                continue;
            }
            else if(i == exclude)
            {
                i++;
                continue;
            }

            if (isIncreasing is null && level[preI] != level[i])
            {
                isIncreasing = level[preI] < level[i];
            }

            if ((isIncreasing.HasValue &&
                (isIncreasing.Value && level[preI] > level[i] ||
                !((bool)isIncreasing!) && level[preI] < level[i])) ||
                level[preI] == level[i] ||
                Math.Abs(level[preI] - level[i]) > 3)
            {
                return false;
            }

            preI = i;
            i++;
        }

        return true;
    }
}
