using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions.Y2024.Day11;

public class Solution(IOptions<AppSettings> options, IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        const int nBlinks = 25;
        List<long> stones = _fileReader.ReadAllLines(GetFullFilePath(fileName))[0].Split(" ").Select(long.Parse).ToList();

        return GetStonesByNumberOfBlinks(stones, nBlinks);
    }

    public override long GetSolution_2(string fileName)
    {
        const int nBlinks = 75;
        List<long> stones = _fileReader.ReadAllLines(GetFullFilePath(fileName))[0].Split(" ").Select(long.Parse).ToList();

        long output = 0;

        foreach(long stone in stones)
        {
            output += GetStonesByNumberOfBlinks(stone, nBlinks, []);
        }

        return output;
    }

    //optimized solutions
    private static long GetStonesByNumberOfBlinks(long stone, int nBlinks, Dictionary<long, Dictionary<int, long>> preComputed)
    {
        if(nBlinks == 0)
        {
            return 1;
        }

        const int yearToMultiply = 2024;

        long output = 0;

        if(preComputed.TryGetValue(stone, out Dictionary<int, long>? dict) && dict.TryGetValue(nBlinks, out long value))
        {
            return value;
        }

        if (stone == 0)
        {
            output += GetStonesByNumberOfBlinks(1, nBlinks - 1, preComputed);
        }
        else if (EvenNumberOfDigits(stone, out int nDigits))
        {
            long tmp = long.Parse(stone.ToString().Substring(0, nDigits / 2));
            output += GetStonesByNumberOfBlinks(tmp, nBlinks - 1, preComputed);
            tmp = long.Parse(stone.ToString().Substring(nDigits / 2));
            output += GetStonesByNumberOfBlinks(tmp, nBlinks - 1, preComputed);
        }
        else
        {
            output += GetStonesByNumberOfBlinks(stone * yearToMultiply, nBlinks - 1, preComputed);
        }

        if (preComputed.TryGetValue(stone, out Dictionary<int, long>? precomputedBlink))
        {
            precomputedBlink.Add(nBlinks, output);
        }
        else
        {
            preComputed.Add(stone, new Dictionary<int, long> { { nBlinks, output } });
        }

        return output;
    }


    //brute force solution
    private static long GetStonesByNumberOfBlinks(List<long> stones, int nBlinks)
    {
        const int yearToMultiply = 2024;
        

        int n;
        int i = 0;
        int j;

        while (i < nBlinks)
        {
            n = stones.Count;
            j = 0;
            while (j < n)
            {
                if (stones[j] == 0)
                {
                    stones[j] = 1;
                }
                else if (EvenNumberOfDigits(stones[j], out int nDigits))
                {
                    long tmp = long.Parse(stones[j].ToString().Substring(0, nDigits / 2));
                    stones.Insert(j, tmp);
                    stones[j + 1] = long.Parse(stones[j + 1].ToString().Substring(nDigits / 2));
                    j++;
                    n = stones.Count;
                }
                else
                {
                    stones[j] *= yearToMultiply;
                }
                j++;
            }
            i++;
        }

        return stones.Count;
    }

    private static bool EvenNumberOfDigits(long n, out int nDigits)
    {
        nDigits = n == 0 ? 1 : (n > 0 ? 1 : 2) + (int)Math.Log10(Math.Abs((double)n));
        return nDigits % 2 == 0;
    }
}
