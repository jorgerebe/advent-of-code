using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions.Y2024.Day04;

public class Solution(IOptions<AppSettings> options,IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        Point point = new Point(0, 0);
        string wordToSearch = "XMAS";

        long output = 0;

        while (point.I < lines.Length)
        {
            point.J = 0;
            while (point.J < lines[point.I].Length)
            {
                if(lines[point.I][point.J] == wordToSearch[0])
                {
                    output += Enum.GetValues<Direction>().Count(d => CheckIfWordExist(lines, wordToSearch, new Point(point.I, point.J), d));
                }
                point.J++;
            }
            point.I++;
        }

        return output;
    }

    public override long GetSolution_2(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        Point point = new (0, 0);
        string wordToSearch = "MAS";

        long output = 0;

        while (point.I < lines.Length)
        {
            point.J = 0;
            while (point.J < lines[point.I].Length)
            {
                if (lines[point.I][point.J] == 'A')
                {
                    if((CheckIfWordExist(lines, wordToSearch, Move(point, Direction.DiagonalUpLeft), Direction.DiagonalDownRight)
                            || CheckIfWordExist(lines, wordToSearch, Move(point, Direction.DiagonalDownRight), Direction.DiagonalUpLeft)) &&
                            (CheckIfWordExist(lines, wordToSearch, Move(point, Direction.DiagonalDownLeft), Direction.DiagonalUpRight)
                            || CheckIfWordExist(lines, wordToSearch, Move(point, Direction.DiagonalUpRight), Direction.DiagonalDownLeft)))
                    {
                        output++;
                    }
                }
                point.J++;
            }
            point.I++;
        }

        return output;
    }

    class Point(int i, int j)
    {
        public int I { get; set; } = i;
        public int J { get; set; } = j;
    }

    enum Direction
    {
        Right,
        Down,
        Left,
        Up,
        DiagonalUpRight,
        DiagonalDownRight,
        DiagonalDownLeft,
        DiagonalUpLeft
    }

    private static Point Move(Point point, Direction direction)
    {
        return direction switch
        {
            Direction.Right => new Point(point.I, point.J + 1),
            Direction.Down => new Point(point.I + 1, point.J),
            Direction.Left => new Point(point.I, point.J - 1),
            Direction.Up => new Point(point.I - 1, point.J),
            Direction.DiagonalUpRight => new Point(point.I - 1, point.J + 1),
            Direction.DiagonalDownRight => new Point(point.I + 1, point.J + 1),
            Direction.DiagonalDownLeft => new Point(point.I + 1, point.J - 1),
            Direction.DiagonalUpLeft => new Point(point.I - 1, point.J - 1),
            _ => throw new InvalidOperationException()
        };
    }

    private static bool CheckIfWordExist(string[] lines, string wordToSearch, Point point, Direction direction)
    {
        int i = 0;

        while (i < wordToSearch.Length)
        {
            if (point.I < 0 || point.I >= lines.Length || point.J < 0 || point.J >= lines[point.I].Length)
            {
                return false;
            }
            if (lines[point.I][point.J] != wordToSearch[i])
            {
                return false;
            }
            point = Move(point, direction);
            i++;
        }
        return true;
    }
}
