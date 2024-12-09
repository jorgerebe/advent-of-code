using System;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions.Y2024.Day06;

public class Solution(IOptions<AppSettings> options, IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        Guard guard = new(lines);

        return guard.GetDistinctPositionsBeforeLeaving().Count;
    }



    public override long GetSolution_2(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        Guard guard = new (lines);

        HashSet<Point> distinctPositionsBeforeLeaving = guard.GetDistinctPositionsBeforeLeaving().Except([guard.Position]).ToHashSet();

        return distinctPositionsBeforeLeaving.Count(guard.IsInLoopIfAddedObstacle);
    }

    class Guard
    {
        public Point Position { get; set; }
        public Direction Direction { get; set; }
        public string[] Map { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public Guard(Point position, Direction direction, string[] map)
        {
            Position = position;
            Direction = direction;
            Map = map;
            Rows = map.Length;
            Columns = map[0].Length;
        }

        public Guard(string[] map)
        {
            Map = map;
            Position = GetGuardPosition(map)!;
            Rows = map.Length;
            Columns = map[0].Length;

            char guard = map[Position.Y][Position.X];

            Direction = guard switch
            {
                '^' => Direction.Up,
                'v' => Direction.Down,
                '<' => Direction.Left,
                '>' => Direction.Right,
                _ => throw new InvalidDataException("Invalid guard direction"),
            };
        }

        public HashSet<Point> GetDistinctPositionsBeforeLeaving()
        {
            HashSet<Point> distinctPositions = [];
            Point currentPosition = new(Position.X, Position.Y);
            Direction currentDirection = Direction;
            Point nextPosition;


            while (currentPosition.IsValid(Rows, Columns))
            {
                distinctPositions.Add(currentPosition);
                nextPosition = Move(currentPosition, currentDirection);

                if (nextPosition.IsValid(Rows, Columns) && Map[nextPosition.Y][nextPosition.X] == '#')
                {
                    currentDirection = currentDirection switch
                    {
                        Direction.Up => Direction.Right,
                        Direction.Right => Direction.Down,
                        Direction.Down => Direction.Left,
                        Direction.Left => Direction.Up,
                        _ => throw new InvalidOperationException(),
                    };
                    continue;
                }

                currentPosition = nextPosition;
            }

            return distinctPositions;
        }

        public bool IsInLoopIfAddedObstacle(Point obstacle)
        {
            Point currentPosition = new(Position.X, Position.Y);
            Point nextPosition;

            Dictionary<Point, List<Direction>> visited = [];
            Direction currentDirection = Direction;

            while (currentPosition.IsValid(Rows, Columns))
            {
                if (visited.TryGetValue(currentPosition, out List<Direction>? directions))
                {
                    if (directions.Contains(currentDirection))
                    {
                        return true;
                    }
                    directions.Add(currentDirection);
                }
                else
                {
                    visited[currentPosition] = [currentDirection];
                }

                nextPosition = Move(currentPosition, currentDirection);

                if (nextPosition.IsValid(Rows, Columns) && (Map[nextPosition.Y][nextPosition.X] == '#' || obstacle == nextPosition))
                {
                    currentDirection = currentDirection switch
                    {
                        Direction.Up => Direction.Right,
                        Direction.Right => Direction.Down,
                        Direction.Down => Direction.Left,
                        Direction.Left => Direction.Up,
                        _ => throw new InvalidOperationException(),
                    };
                    continue;
                }

                currentPosition = nextPosition;
            }

            return false;
        }

        static Point? GetGuardPosition(string[] lines)
        {
            int i = 0;
            int j;

            int rows = lines.Length;
            int cols = lines[0].Length;
            char current;

            while (i < rows)
            {
                j = 0;
                while (j < cols)
                {
                    current = lines[i][j];
                    if (current != '#' && current != '.')
                    {
                        return new Point(j, i);
                    }
                    j++;
                }
                i++;
            }

            throw new ArgumentException("There is no guard in the given input");
        }
    }

    public record Point(int X, int Y)
    {
        public int X { get; set; } = X;
        public int Y { get; set; } = Y;

        public bool IsValid(int rows, int columns)
        {
            return X >= 0 && X < columns && Y >= 0 && Y < rows;
        }
    }

    private static Point Move(Point point, Direction direction)
    {
        return direction switch
        {
            Direction.Right => new Point(point.X + 1, point.Y),
            Direction.Down => new Point(point.X, point.Y + 1),
            Direction.Left => new Point(point.X - 1, point.Y),
            Direction.Up => new Point(point.X, point.Y - 1),
            _ => throw new InvalidOperationException()
        };
    }

    enum Direction
    {
        Left,
        Up,
        Right,
        Down,
    }
}
