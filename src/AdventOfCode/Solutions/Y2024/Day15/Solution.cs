using AdventOfCode.Services;
using Microsoft.Extensions.Options;

using static AdventOfCode.Solutions.Y2024.Day06.Solution;

namespace AdventOfCode.Solutions.Y2024.Day15;

public class Solution(IOptions<AppSettings> options, IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        WarehousePart1 warehouse = new WarehousePart1(lines);

        warehouse.MoveRobot();

        return warehouse.SumGpsCoordinates();
    }

    public override long GetSolution_2(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        WarehousePart2 warehouse = new WarehousePart2(lines);

        warehouse.MoveRobot();

        return warehouse.SumGpsCoordinates();
    }

    public abstract class Warehouse
    {
        public int Rows { get; set; }
        public int Columns { get; set; }

        public abstract char[,] Map { get; set; }
        public abstract Robot Robot { get; set; }

        public string MovementsToAttemp { get; set; } = string.Empty;

        public void PrintMap()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Console.Write(Map[i, j]);
                }
                Console.WriteLine();
            }
        }

        public void MoveRobot()
        {
            foreach (char move in MovementsToAttemp)
            {
                Robot.Move(move, Map);
            }
        }

        public abstract long SumGpsCoordinates();
    }

    public abstract class Robot(int x, int y)
    {
        public int X = x;
        public int Y = y;

        public abstract void Move(char move, char[,] map);
    }

    public class WarehousePart1 : Warehouse
    {
        public override char[,] Map { get; set; }
        public override Robot Robot { get; set; }

        public WarehousePart1(string[] lines)
        {
            Columns = lines[0].Length;
            int i = 0;

            while (lines[i].Length > 0)
            {
                i++;
            }

            Rows = i;

            Map = new char[Rows, Columns];

            WriteMap(lines);

            if (Robot is null)
            {
                throw new ArgumentException("No Robot found in the Warehouse");
            }

            i++;

            MovementsToAttemp = string.Concat(lines.Skip(i));
        }

        private void WriteMap(string[] lines)
        {
            char current;
            int i = 0;
            int j;

            while (i < Rows)
            {
                j = 0;
                while (j < Columns)
                {
                    current = lines[i][j];

                    if (current == '@')
                    {
                        Robot = new RobotPart1(j, i);
                    }

                    Map[i, j] = current;
                    j++;
                }
                i++;
            }
        }

        public override long SumGpsCoordinates()
        {
            long sum = 0;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Map[i, j] == 'O')
                    {
                        sum += 100 * i + j;
                    }
                }
            }

            return sum;
        }
    }

    public class RobotPart1(int x, int y) : Robot(x, y)
    {
        public override void Move(char move, char[,] map)
        {
            int offsetX;
            int offsetY;

            switch (move)
            {
                case '<':
                    offsetX = -1;
                    offsetY = 0;
                    break;
                case '>':
                    offsetX = 1;
                    offsetY = 0;
                    break;
                case '^':
                    offsetX = 0;
                    offsetY = -1;
                    break;
                case 'v':
                    offsetX = 0;
                    offsetY = 1;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            int currentX = X;
            int currentY = Y;

            char current = map[currentY, currentX];

            while (current != '#' && current != '.')
            {
                currentX += offsetX;
                currentY += offsetY;

                current = map[currentY, currentX];
            }

            if (current == '.')
            {
                while (currentY != Y || currentX != X)
                {
                    map[currentY, currentX] = map[currentY - offsetY, currentX - offsetX];

                    currentX -= offsetX;
                    currentY -= offsetY;
                }

                map[currentY, currentX] = '.';
                X += offsetX;
                Y += offsetY;
            }
        }
    }

    public class WarehousePart2 : Warehouse
    {
        public override char[,] Map { get; set; }
        public override Robot Robot { get; set; }

        public WarehousePart2(string[] lines)
        {
            Columns = lines[0].Length * 2;
            int i = 0;

            while (lines[i].Length > 0)
            {
                i++;
            }

            Rows = i;

            Map = new char[Rows, Columns];

            WriteMap(lines);

            if (Robot is null)
            {
                throw new ArgumentException("No Robot found in the Warehouse");
            }

            i++;

            MovementsToAttemp = string.Concat(lines.Skip(i));
        }

        private void WriteMap(string[] lines)
        {
            char current;
            int i = 0;
            int j;

            int currentLineElement;

            while (i < Rows)
            {
                j = 0;
                currentLineElement = 0;
                while (j < Columns)
                {
                    current = lines[i][currentLineElement];

                    if (current == '@')
                    {
                        Robot = new RobotPart2(j, i);
                        Map[i, j] = current;
                        Map[i, j + 1] = '.';
                    }
                    else if (current == '#')
                    {
                        Map[i, j] = current;
                        Map[i, j + 1] = current;
                    }
                    else if(current == '.')
                    {
                        Map[i, j] = current;
                        Map[i, j + 1] = current;
                    }
                    else
                    {
                        Map[i, j] = '[';
                        Map[i, j + 1] = ']';
                    }
                    j += 2;
                    currentLineElement++;
                }
                i++;
            }
        }

        public override long SumGpsCoordinates()
        {
            long sum = 0;

            int i = 0;
            int j = 0;

            while(i < Rows)
            {
                j = 0;
                while (j < Columns)
                {
                    if (Map[i, j] == '[')
                    {
                        sum += 100 * i + j;
                    }
                    j++;
                }
                i++;
            }

            return sum;
        }
    }

    public class RobotPart2(int x, int y) : Robot(x, y)
    {
        public override void Move(char move, char[,] map)
        {
            int offsetX;
            int offsetY;

            switch (move)
            {
                case '<':
                    offsetX = -1;
                    offsetY = 0;
                    break;
                case '>':
                    offsetX = 1;
                    offsetY = 0;
                    break;
                case '^':
                    offsetX = 0;
                    offsetY = -1;
                    break;
                case 'v':
                    offsetX = 0;
                    offsetY = 1;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            int currentX = X;
            int currentY = Y;

            Stack<Point> pointsToMove = new();
            Queue<Point> pointsToCheck = new();
            HashSet<Point> pointsChecked = [];
            Point currentPoint = new(currentX, currentY);

            pointsToCheck.Enqueue(currentPoint);

            Point current;
            Point? next = null;
            char currentElement;

            bool canMove = true;

            while (pointsToCheck.Count > 0 && canMove)
            {
                current = pointsToCheck.Dequeue();
                currentElement = map[current.Y, current.X];

                if(currentElement == '#')
                {
                    canMove = false;
                    break;
                }

                if(currentElement == '.')
                {
                    continue;
                }

                if(!pointsChecked.Contains(current))
                {
                    pointsToMove.Push(current);
                }

                pointsChecked.Add(current);


                if (move == '^' || move == 'v')
                {
                    if (currentElement == '[')
                    {
                        next = new Point(current.X + 1, current.Y);
                    }
                    else if (currentElement == ']')
                    {
                        next = new Point(current.X - 1, current.Y);
                    }

                    if(next is not null && !pointsChecked.Contains(next))
                    {
                        pointsToCheck.Enqueue(next);
                    }
                }

                next = new Point(current.X + offsetX, current.Y + offsetY);

                if (next is not null && !pointsChecked.Contains(next))
                {
                    pointsToCheck.Enqueue(next);
                }
            }

            if (canMove)
            {
                char tmp;
                foreach(Point point in pointsToMove)
                {
                    tmp = map[point.Y + offsetY, point.X + offsetX];
                    map[point.Y + offsetY, point.X + offsetX] = map[point.Y, point.X];
                    map[point.Y, point.X] = tmp;
                }

                X += offsetX;
                Y += offsetY;
            }
        }
    }
}
