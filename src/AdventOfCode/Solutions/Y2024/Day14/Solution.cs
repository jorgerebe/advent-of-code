using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions.Y2024.Day14;

public class Solution(IOptions<AppSettings> options, IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        const int width = 101;
        const int height = 103;
        const int secondsToElapse = 100;

        List<Guard> guards = ParseInput(lines, width, height);

        guards.ForEach(g => g.ElapseTime(secondsToElapse));

        int middleX = width / 2;
        int middleY = height / 2;

        int robotsFirstQuadrant = guards.Count(x => x.X < middleX && x.Y < middleY);
        int robotsSecondQuadrant = guards.Count(x => x.X > middleX && x.Y < middleY);
        int robotsThirdQuadrant = guards.Count(x => x.X < middleX && x.Y > middleY);
        int robotsFourthQuadrant = guards.Count(x => x.X > middleX && x.Y > middleY);

        return robotsFirstQuadrant * robotsSecondQuadrant * robotsThirdQuadrant * robotsFourthQuadrant;
    }

    public override long GetSolution_2(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        const int width = 101;
        const int height = 103;

        List<Guard> guards = ParseInput(lines, width, height);

        double minVarianceMultiplied = int.MaxValue;
        double tmpVarianceMultiplied;
        double varianceX;
        double varianceY;
        int secondsToMinVarianceMultiplied = -1;

        int[] robotsInQuadrant = new int[4];

        for(int i = 1; i <= width*height; i++)
        {
            guards.ForEach(g => g.ElapseTime(1));

            varianceX = Variance(guards.Select(g => g.X).ToList());
            varianceY = Variance(guards.Select(g => g.Y).ToList());

            tmpVarianceMultiplied = varianceX * varianceY;

            if(tmpVarianceMultiplied < minVarianceMultiplied)
            {
                minVarianceMultiplied = tmpVarianceMultiplied;
                secondsToMinVarianceMultiplied = i;
            }
        }

        guards = ParseInput(lines, width, height);

        guards.ForEach(g => g.ElapseTime(secondsToMinVarianceMultiplied));

        guards = guards.OrderBy(g => g.Y).ThenBy(g => g.X).ToList();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (guards.Any(g => g.X == j && g.Y == i))
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
        }

        return secondsToMinVarianceMultiplied;
    }

    public class Guard(int x, int y, int speedX, int speedY, int width, int height)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;

        public int SpeedX { get; set; } = speedX;
        public int SpeedY { get; set; } = speedY;

        public int Width { get; set; } = width;
        public int Height { get; set; } = height;

        public void ElapseTime(int seconds)
        {
            X = ((X + SpeedX * seconds) % Width + Width) % Width;
            Y = ((Y + SpeedY * seconds) % Height + Height) % Height;
        }
    }

    public static List<Guard> ParseInput(string[] lines, int width, int height)
    {
        List<Guard> guards = [];

        foreach (string line in lines)
        {
            string[] parts = line.Split(' ');
            string[] position = parts[0].Substring(2).Split(',');
            string[] speed = parts[1].Substring(2).Split(',');

            guards.Add(new Guard(int.Parse(position[0]), int.Parse(position[1]),
                int.Parse(speed[0]), int.Parse(speed[1]), width, height));
        }

        return guards;
    }

    public static double Variance(List<int> values)
    {
        double mean = values.Average();
        return values.Average(v => Math.Pow(v - mean, 2));
    }
}
