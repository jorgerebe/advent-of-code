using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions.Y2024.Day05;

public class Solution(IOptions<AppSettings> options,IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        Dictionary<int, IList<int>> postToPriorOrder = new();

        int i = 0;
        char[] separator = ['|'];
        int[] pair;

        while (lines[i].Length > 0)
        {
            pair = lines[i].Split(separator).Select(int.Parse).ToArray();

            if (!postToPriorOrder.ContainsKey(pair[1]))
            {
                postToPriorOrder.Add(pair[1], [pair[0]]);
            }
            else
            {
                postToPriorOrder[pair[1]].Add(pair[0]);
            }
            i++;
        }

        i++;
        char[] updatesSeparator = [','];
        HashSet<int> elementsInUpdate = new();
        List<IList<int>> rightUpdates = new();
        int j = 0;

        while (i < lines.Length)
        {
            IList<int> update = lines[i].Split(updatesSeparator).Select(int.Parse).ToList();
            elementsInUpdate.Clear();
            j = 0;

            while (j < update.Count)
            {
                if(postToPriorOrder.TryGetValue(update[j], out IList<int>? prior))
                {
                    IList<int> elementsThatHaveToBeBefore = prior.Intersect(update).ToList();

                    if (elementsThatHaveToBeBefore.Count > 0 && !elementsThatHaveToBeBefore.All(elementsInUpdate.Contains))
                    {
                        break;
                    }
                }

                elementsInUpdate.Add(update[j]);
                j++;
            }

            if(j == update.Count)
            {
                rightUpdates.Add(update);
            }

            i++;
        }

        return rightUpdates.Sum(x => x[x.Count/2]);
    }

    public override long GetSolution_2(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        Dictionary<int, IList<int>> postToPriorOrder = new();

        int i = 0;
        char[] separator = ['|'];
        int[] pair;

        while (lines[i].Length > 0)
        {
            pair = lines[i].Split(separator).Select(int.Parse).ToArray();

            if (!postToPriorOrder.ContainsKey(pair[1]))
            {
                postToPriorOrder.Add(pair[1], [pair[0]]);
            }
            else
            {
                postToPriorOrder[pair[1]].Add(pair[0]);
            }
            i++;
        }

        i++;
        char[] updatesSeparator = [','];
        HashSet<int> elementsInUpdate = new();
        List<IList<int>> incorrectUpdates = new();
        int j = 0;
        bool added = false;

        while (i < lines.Length)
        {
            IList<int> update = lines[i].Split(updatesSeparator).Select(int.Parse).ToList();
            elementsInUpdate.Clear();
            j = 0;

            while (j < update.Count)
            {
                if (postToPriorOrder.TryGetValue(update[j], out IList<int>? prior))
                {
                    IList<int> elementsThatHaveToBeBefore = prior.Intersect(update).ToList();

                    if (elementsThatHaveToBeBefore.Count > 0 && !elementsThatHaveToBeBefore.All(elementsInUpdate.Contains))
                    {
                        if(!added)
                        {
                            added = !added;
                            incorrectUpdates.Add(update);
                        }

                        int nChanged = 0;
                        int k = j;
                        int indexOfCurrent = j;

                        while(k < update.Count && nChanged < elementsThatHaveToBeBefore.Count)
                        {
                            if(elementsThatHaveToBeBefore.Contains(update[k]))
                            {
                                (update[indexOfCurrent], update[k]) = (update[k], update[indexOfCurrent]);
                                nChanged++;
                                indexOfCurrent = k;
                            }
                            k++;
                        }
                        continue;
                    }
                }

                elementsInUpdate.Add(update[j]);
                j++;
            }
            added = false;
            i++;
        }

        return incorrectUpdates.Sum(x => x[x.Count / 2]);
    }
}
