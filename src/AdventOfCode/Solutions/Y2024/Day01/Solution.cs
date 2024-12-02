using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions.Y2024.Day01;

public class Solution(IOptions<AppSettings> options,IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string content = _fileReader.Read(GetFullFilePath(fileName));

        char[] delimiters = ['\n', '\r', ' '];

        string[] allEntries = content.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

        List<int> leftList = [];
        List<int> rightList = [];

        int i = 0;

        while (i < allEntries.Length)
        {
            leftList.Add(int.Parse(allEntries[i]));
            rightList.Add(int.Parse(allEntries[i + 1]));
            i += 2;
        }

        leftList = [.. leftList.OrderBy(x => x)];
        rightList = [.. rightList.OrderBy(x => x)];

        long output = 0;

        i = 0;

        while(i < leftList.Count)
        {
            output += Math.Abs(rightList[i] - leftList[i]);
            i++;
        }

        return output;
    }

    public override long GetSolution_2(string fileName)
    {
        string content = _fileReader.Read(GetFullFilePath(fileName));

        char[] delimiters = ['\n', '\r', ' '];

        string[] allEntries = content.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

        List<int> leftList = [];
        List<int> rightList = [];

        int i = 0;

        while (i < allEntries.Length)
        {
            leftList.Add(int.Parse(allEntries[i]));
            rightList.Add(int.Parse(allEntries[i + 1]));
            i += 2;
        }

        Dictionary<int, int> occurrencesRightList = new();

        i = 0;

        while(i < rightList.Count)
        {
            if (occurrencesRightList.ContainsKey(rightList[i]))
            {
                occurrencesRightList[rightList[i]]++;
            }
            else
            {
                occurrencesRightList[rightList[i]] = 1;
            }

            i++;
        }

        i = 0;
        long output = 0;

        while (i < leftList.Count)
        {
            if (occurrencesRightList.ContainsKey(leftList[i]))
            {
                output += leftList[i] * occurrencesRightList[leftList[i]];
            }
            i++;
        }

        return output;
    }
}
