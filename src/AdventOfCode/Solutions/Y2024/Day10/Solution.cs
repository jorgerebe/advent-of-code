using AdventOfCode.Services;
using Microsoft.Extensions.Options;

using static AdventOfCode.Solutions.Y2024.Day06.Solution;

namespace AdventOfCode.Solutions.Y2024.Day10;

public class Solution(IOptions<AppSettings> options, IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] map = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        return GetSumOfTrailHeadsScore(map);
    }

    public override long GetSolution_2(string fileName)
    {
        string[] map = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        return GetSumOfTrailHeadsRating(map);
    }

    private static long GetSumOfTrailHeadsScore(string[] map)
    {
        return GetReachableNineHeightPositionsByCollection(map, new HashSet<Point>());
    }

    private static long GetSumOfTrailHeadsRating(string[] map)
    {
        return GetReachableNineHeightPositionsByCollection(map, new List<Point>());
    }

    private static long GetReachableNineHeightPositionsByCollection(string[] map, ICollection<Point> points)
    {
        long output = 0;

        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[i].Length; j++)
            {
                if (map[i][j] == '0')
                {
                    ExploreTrailFromPosition(map, i, j, map[i][j], points);
                    output += points.Count;
                    points.Clear();
                }
            }
        }

        return output;
    }

    private static void ExploreTrailFromPosition(string[] map, int i, int j, char current, ICollection<Point> positions)
    {
        if (map[i][j] == '9')
        {
            positions.Add(new Point(i, j));
        }

        char toSearch = (char)(current + 1);

        if (i > 0 && map[i - 1][j] == toSearch)
        {
            ExploreTrailFromPosition(map, i - 1, j, toSearch, positions);
        }
        if (i + 1 < map.Length && map[i + 1][j] == toSearch)
        {
            ExploreTrailFromPosition(map, i + 1, j, toSearch, positions);
        }
        if (j > 0 && map[i][j - 1] == toSearch)
        {
            ExploreTrailFromPosition(map, i, j - 1, toSearch, positions);
        }
        if (j + 1 < map[i].Length && map[i][j + 1] == toSearch)
        {
            ExploreTrailFromPosition(map, i, j + 1, toSearch, positions);
        }
    }
}
