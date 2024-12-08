using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions.Y2024.Day07;

public class Solution(IOptions<AppSettings> options, IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        List<Equation> equations = lines.Select(GetEquationFromLine).ToList();

        List<Operation> operationsToCheck = [Operation.Add, Operation.Multiply];

        List<Equation> list = equations.Where(eq => eq.IsValid(operationsToCheck)).ToList();

        return equations.Where(eq => eq.IsValid(operationsToCheck)).Sum(eq => eq.TestValue);
    }

    public override long GetSolution_2(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        List<Equation> equations = lines.Select(GetEquationFromLine).ToList();

        List<Operation> operationsToCheck = [Operation.Add, Operation.Multiply, Operation.Concatenation];

        List<Equation> list = equations.Where(eq => eq.IsValid(operationsToCheck)).ToList();

        return equations.Where(eq => eq.IsValid(operationsToCheck)).Sum(eq => eq.TestValue);
    }


    class Equation(long testValue, List<long> remainingNumbers)
    {
        public long TestValue { get; set; } = testValue;
        public List<long> RemainingNumbers { get; set; } = remainingNumbers;

        public bool IsValid(List<Operation> operations)
        {
            if(RemainingNumbers.Count == 0)
            {
                return TestValue == 0;
            }

            return IsValidBackTracking(1, RemainingNumbers[0], operations);
        }

        private bool IsValidBackTracking(int remainingNumberIndex, long currentValue, List<Operation> operations)
        {
            if(remainingNumberIndex >= RemainingNumbers.Count)
            {
                return currentValue == TestValue;
            }

            foreach (Operation operation in operations)
            {
                switch(operation)
                {
                    case Operation.Add:
                        if (IsValidBackTracking(remainingNumberIndex + 1, currentValue + RemainingNumbers[remainingNumberIndex], operations))
                        {
                            return true;
                        }
                        break;
                    case Operation.Multiply:
                        if (IsValidBackTracking(remainingNumberIndex + 1, currentValue * RemainingNumbers[remainingNumberIndex], operations))
                        {
                            return true;
                        }
                        break;
                    case Operation.Concatenation:
                        if (IsValidBackTracking(remainingNumberIndex + 1,
                                                long.Parse(currentValue.ToString() +  RemainingNumbers[remainingNumberIndex].ToString()),
                                                operations))
                        {
                            return true;
                        }
                        break;
                }
            }

            return false;
        }
    }

    enum Operation
    {
        Add,
        Multiply,
        Concatenation
    }

    static Equation GetEquationFromLine(string line)
    {
        string[] parts = line.Split(":");
        long testValue = long.Parse(parts[0]);
        List<long> remainingNumbers = parts[1].Trim().Split(" ").Select(long.Parse).ToList();
        return new Equation(testValue, remainingNumbers);
    }
}
