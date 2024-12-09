using AdventOfCode.Services;
using Microsoft.Extensions.Options;

using static AdventOfCode.Solutions.Y2024.Day06.Solution;

namespace AdventOfCode.Solutions.Y2024.Day08;

public class Solution(IOptions<AppSettings> options, IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        int i;
        int j;

        int rows = lines.Length;
        int columns = lines[0].Length;

        Dictionary<char, List<Point>> antennaToLocations = GetLocationsByAntennaFrequencies(lines);

        List<Point> points;
        HashSet<Point> antinodesLocations = [];

        foreach(char antenna in antennaToLocations.Keys)
        {
            points = antennaToLocations[antenna];

            Point currentAntenna;
            Point nextAntenna;
            Point antinode;

            i = 0;

            while(i < points.Count)
            {
                currentAntenna = points[i];
                j = i+1;
                while(j < points.Count)
                {
                    nextAntenna = points[j];

                    antinode = GetAntinode(currentAntenna, nextAntenna);

                    if(antinode.IsValid(rows, columns))
                    {
                        antinodesLocations.Add(antinode);
                    }

                    antinode = GetAntinode(nextAntenna, currentAntenna);

                    if (antinode.IsValid(rows, columns))
                    {
                        antinodesLocations.Add(antinode);
                    }

                    j++;
                }
                i++;
            }
        }

        return antinodesLocations.Count;
    }

    public override long GetSolution_2(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        int i;
        int j;

        int rows = lines.Length;
        int columns = lines[0].Length;

        Dictionary<char, List<Point>> antennaToLocations = GetLocationsByAntennaFrequencies(lines);

        List<Point> points;
        HashSet<Point> antinodesLocations = [];

        foreach (char antenna in antennaToLocations.Keys)
        {
            points = antennaToLocations[antenna];

            Point currentAntenna;
            Point nextAntenna;
            Point antinode;
            int multiplier;

            i = 0;

            while (i < points.Count)
            {
                currentAntenna = points[i];
                j = i + 1;
                while (j < points.Count)
                {
                    nextAntenna = points[j];

                    multiplier = 0;
                    antinode = GetAntinode(currentAntenna, nextAntenna, multiplier);

                    while(antinode.IsValid(rows, columns))
                    {
                        antinodesLocations.Add(antinode);
                        multiplier++;
                        antinode = GetAntinode(currentAntenna, nextAntenna, multiplier);
                    }

                    multiplier = 0;
                    antinode = GetAntinode(nextAntenna, currentAntenna, multiplier);

                    while (antinode.IsValid(rows, columns))
                    {
                        antinodesLocations.Add(antinode);
                        multiplier++;
                        antinode = GetAntinode(nextAntenna, currentAntenna, multiplier);
                    }

                    j++;
                }
                i++;
            }
        }

        return antinodesLocations.Count;
    }

    static Dictionary<char, List<Point>> GetLocationsByAntennaFrequencies(string[] lines)
    {
        Dictionary<char, List<Point>> antennaToLocations = [];
        int i = 0;
        int j;
        char current;

        while (i < lines.Length)
        {
            j = 0;
            while (j < lines[i].Length)
            {
                current = lines[i][j];

                if (!char.IsAsciiLetterOrDigit(current))
                {
                    j++;
                    continue;
                }

                if (antennaToLocations.ContainsKey(lines[i][j]))
                {
                    antennaToLocations[current].Add(new Point(j, i));
                }
                else
                {
                    antennaToLocations.Add(current, [new Point(j, i)]);
                }
                j++;
            }
            i++;
        }

        return antennaToLocations;
    }


    static Point GetAntinode(Point p1, Point p2, int multiplier=1)
    {
        int newX = p2.X + ((p2.X - p1.X) * multiplier);
        int newY = p2.Y + ((p2.Y - p1.Y) * multiplier);

        return new Point(newX, newY);
    }
}
