using AdventOfCode.Services;
using Microsoft.Extensions.Options;

using static AdventOfCode.Solutions.Y2024.Day06.Solution;

namespace AdventOfCode.Solutions.Y2024.Day16;

public class Solution(IOptions<AppSettings> options, IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        Maze maze = new(lines);
        return maze.GetLowestScore(optimized:true);
    }

    public override long GetSolution_2(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        Maze maze = new(lines);
        return maze.GetNumberOfTilesInBestPaths();
    }

    public class Maze
    {
        public string[] Map;
        public Point Start;
        public Point End;

        public record State(Point Point, Direction Direction);

        public Maze(string[] map)
        {
            Map = map;

            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == 'S')
                    {
                        Start = new Point(j, i);
                    }
                    else if (map[i][j] == 'E')
                    {
                        End = new Point(j, i);
                    }
                }
            }

            if(Start == null || End == null)
            {
                throw new ArgumentException("Invalid maze");
            }
        }

        public long GetLowestScore(bool optimized = true)
        {
            if (optimized)
            {
                return GetLowestScoreDijkstra(Start, Direction.Right, End);
            }
            else
            {
                long lowestScore = long.MaxValue;

                GetLowestScoreBruteForce(Start, Direction.Right, 0, ref lowestScore, []);

                return lowestScore;
            }
        }

        public long GetNumberOfTilesInBestPaths()
        {
            return GetNumberOfTilesInBestPathsDijkstra(Start, Direction.Right, End);
        }

        //Part 1 brute force solution
        public void GetLowestScoreBruteForce(Point current, Direction direction, long currentScore, ref long lowestScore,
            Dictionary<(Point, Direction), long> costByState)
        {
            if (Map[current.Y][current.X] == '#')
            {
                return;
            }

            if (currentScore >= lowestScore)
            {
                return;
            }

            if (costByState.TryGetValue((current, direction), out long smallestScore))
            {
                if (smallestScore <= currentScore)
                {
                    return;
                }
                costByState[(current, direction)] = currentScore;
            }
            else
            {
                costByState.Add((current, direction), currentScore);
            }


            if (current == End)
            {
                lowestScore = Math.Min(lowestScore, currentScore);
                return;
            }

            GetLowestScoreBruteForce(Move(current, direction), direction, currentScore + 1, ref lowestScore, costByState);
            GetLowestScoreBruteForce(current, RotateLeft(direction), currentScore + 1000, ref lowestScore, costByState);
            GetLowestScoreBruteForce(current, RotateRight(direction), currentScore + 1000, ref lowestScore, costByState);
        }

        //Part 1 optimized solution - dijkstra's algorithm
        public long GetLowestScoreDijkstra(Point start, Direction startDirection, Point end)
        {
            PriorityQueue<(Point Point, Direction Direction, long Cost), long> priorityQueue = new();
            Dictionary<(Point, Direction), long> costByState = [];

            priorityQueue.Enqueue((start, startDirection, 0), 0);

            while(priorityQueue.Count > 0)
            {
                (Point Point, Direction Direction, long Cost) = priorityQueue.Dequeue();

                if (Map[Point.Y][Point.X] == '#')
                {
                    continue;
                }

                if (Point == end)
                {
                    return Cost;
                }

                if (costByState.TryGetValue((Point, Direction), out long existingCost))
                {
                    if (existingCost <= Cost)
                    {
                        continue;
                    }
                    costByState[(Point, Direction)] = Cost;
                }
                else
                {
                    costByState.Add((Point, Direction), Cost);
                }

                priorityQueue.Enqueue((Move(Point, Direction), Direction, Cost + 1), Cost + 1);
                priorityQueue.Enqueue((Point, RotateLeft(Direction), Cost + 1000), Cost + 1000);
                priorityQueue.Enqueue((Point, RotateRight(Direction), Cost + 1000), Cost + 1000);
            }

            return -1;
        }

        //Part 2 solution
        public long GetNumberOfTilesInBestPathsDijkstra(Point start, Direction startDirection, Point end)
        {
            PriorityQueue< (State current, State prev, long cost), long> priorityQueue = new();
            Dictionary<State, long> costByState = [];

            Dictionary<State, HashSet<State>> predecessors = [];

            priorityQueue.Enqueue((new State(start, startDirection), null!, 0), 0);

            State current;
            State prev;
            long cost;

            while (priorityQueue.Count > 0)
            {
                (current, prev, cost) = priorityQueue.Dequeue();

                if (Map[current.Point.Y][current.Point.X] == '#')
                {
                    continue;
                }

                if (costByState.TryGetValue(current, out long existingCost))
                {
                    if (existingCost < cost)
                    {
                        continue;
                    }
                    else if(cost < existingCost)
                    {
                        predecessors[current].Clear();
                        costByState[current] = cost;
                    }
                    predecessors[current].Add(prev);
                }
                else
                {
                    costByState.Add(current, cost);

                    if(prev == null)
                    {
                        predecessors.Add(current, []);
                    }
                    else
                    {
                        predecessors.Add(current, [prev]);
                    }
                }

                if(current.Point == end)
                {
                    continue;
                }

                State newState;

                newState = new State(Move(current.Point, current.Direction), current.Direction);
                priorityQueue.Enqueue((newState, current, cost + 1), cost + 1);

                newState = new State(current.Point, RotateLeft(current.Direction));
                priorityQueue.Enqueue((newState, current, cost + 1000), cost + 1000);

                newState = new State(current.Point, RotateRight(current.Direction));
                priorityQueue.Enqueue((newState, current, cost + 1000), cost + 1000);
            }

            HashSet<Point> visitedTiles = [];
            Queue<State> statesToCheck = [];

            IList<State> statesWithEndReached = costByState.Where(x => x.Key.Point == end).Select(x => x.Key).ToList();

            long minCost = statesWithEndReached.Min(x => costByState[x]);

            foreach(State state in statesWithEndReached)
            {
                if (costByState[state] != minCost)
                {
                    continue;
                }

                statesToCheck.Enqueue(state);

                while (statesToCheck.Count > 0)
                {
                    current = statesToCheck.Dequeue();

                    visitedTiles.Add(current.Point);

                    if(predecessors.TryGetValue(current, out HashSet<State>? currentPredecessors))
                    {
                        foreach (State predecessor in currentPredecessors)
                        {
                            statesToCheck.Enqueue(predecessor);
                        }
                    }
                }
            }

            return visitedTiles.Count;
        }

        private static Direction RotateLeft(Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Left,
                Direction.Left => Direction.Down,
                Direction.Down => Direction.Right,
                Direction.Right => Direction.Up,
                _ => throw new InvalidOperationException()
            };
        }

        private static Direction RotateRight(Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                _ => throw new InvalidOperationException()
            };
        }
    }
}
