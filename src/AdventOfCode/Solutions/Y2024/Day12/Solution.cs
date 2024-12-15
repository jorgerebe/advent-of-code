using AdventOfCode.Services;
using Microsoft.Extensions.Options;

using static AdventOfCode.Solutions.Y2024.Day06.Solution;

namespace AdventOfCode.Solutions.Y2024.Day12;

public class Solution(IOptions<AppSettings> options, IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] map = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        long output = 0;

        int rows = map.Length;
        int columns = map[0].Length;

        HashSet<Point> visited = [];

        List<Direction> directions = [Direction.Up, Direction.Down, Direction.Left, Direction.Right];
        Point current;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                current = new Point(j, i);
                if (!visited.Contains(current))
                {
                    int currentArea = 0;
                    int currentPerimeter = 0;
                    output +=
                        VisitRegionAndGetPricePart1(map, rows, columns, map[i][j],current, visited, ref currentArea, ref currentPerimeter, directions);
                }
            }
        }

        return output;
    }

    public override long GetSolution_2(string fileName)
    {
        string[] map = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        long output = 0;

        int rows = map.Length;
        int columns = map[0].Length;

        HashSet<Point> visited = [];

        List<Direction> directions = [Direction.Up, Direction.Down, Direction.Left, Direction.Right];
        Point current;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                current = new Point(j, i);
                if (!visited.Contains(current))
                {
                    int currentArea = 0;
                    int sides = 0;
                    output +=
                        VisitRegionAndGetPricePart2(map, rows, columns, map[i][j], current, visited, ref currentArea, ref sides, directions);
                }
            }
        }

        return output;
    }

    public static long VisitRegionAndGetPricePart1(string[] map, int rows, int columns, char region, Point current, HashSet<Point> visited,
        ref int currentArea, ref int currentPerimeter, List<Direction> directions)
    {
        if(visited.Contains(current))
        {
            return 0;
        }

        visited.Add(current);

        List<Point> neighbors = [];

        foreach(Direction direction in directions)
        {
            Point candidate = Move(current, direction);
            if(candidate.IsValid(rows, columns) && map[candidate.Y][candidate.X] == region)
            {
                neighbors.Add(candidate);
                VisitRegionAndGetPricePart1(map, rows, columns, region, candidate, visited, ref currentArea, ref currentPerimeter, directions);
            }
        }

        currentArea++;
        currentPerimeter += directions.Count - neighbors.Count;

        return currentArea * currentPerimeter;
    }

    public static long VisitRegionAndGetPricePart2(string[] map, int rows, int columns, char region, Point current, HashSet<Point> visited,
        ref int currentArea, ref int sides, List<Direction> directions)
    {
        if (visited.Contains(current))
        {
            return 0;
        }

        visited.Add(current);

        List<Point> neighbors = [];

        foreach (Direction direction in directions)
        {
            Point candidate = Move(current, direction);
            if (candidate.IsValid(rows, columns) && map[candidate.Y][candidate.X] == region)
            {
                neighbors.Add(candidate);
                VisitRegionAndGetPricePart2(map, rows, columns, region, candidate, visited, ref currentArea, ref sides, directions);
            }
        }

        currentArea++;

        List<Point> verticalNeighbors = neighbors.Where(n => n.X == current.X).ToList();
        List<Point> horizontalNeighbors = neighbors.Where(n => n.Y == current.Y).ToList();

        for (int diffX = -1; diffX <= 1; diffX += 2)
        {
            for(int diffY = -1; diffY <= 1; diffY += 2)
            {
                if(verticalNeighbors.Any(n => n.Y == current.Y+diffY) &&
                    horizontalNeighbors.Any(n => n.X == current.X+diffX))
                {
                    if (map[current.Y + diffY][current.X + diffX] != region)
                    {
                        sides++;
                    }
                }
            }
        }

        if(verticalNeighbors.Count < 2 && horizontalNeighbors.Count < 2)
        {
            sides++;
            if(verticalNeighbors.Count == 0 ||
                horizontalNeighbors.Count == 0)
            {
                sides++;
            }
        }

        if (neighbors.Count == 0)
        {
            sides = 4;
        }

        return currentArea * sides;
    }
}
