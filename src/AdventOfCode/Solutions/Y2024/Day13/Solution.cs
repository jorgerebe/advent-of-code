using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions.Y2024.Day13;

public class Solution(IOptions<AppSettings> options, IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        int i = 0;
        double[] firstEquationCoefficients;
        double[] secondEquationCoefficients;
        double[] prices;

        const long tokensForButtonA = 3;
        const long tokensForButtonB = 1;

        List<EquationSystem> equationsSystems = [];

        while(i < lines.Length)
        {
            firstEquationCoefficients = lines[i].Split(":")[1].Split(",").Select(x => double.Parse(x.Trim().Substring(2))).ToArray();
            secondEquationCoefficients = lines[i+1].Split(":")[1].Split(",").Select(x => double.Parse(x.Trim().Substring(2))).ToArray();
            prices = lines[i + 2].Split(":")[1].Split(",").Select(x => double.Parse(x.Trim().Substring(2))).ToArray();

            equationsSystems.Add
            (
                new EquationSystem
                (
                    [new Equation([firstEquationCoefficients[0], secondEquationCoefficients[0]], prices[0]),
                    new Equation([firstEquationCoefficients[1], secondEquationCoefficients[1]], prices[1])]
                )
            );

            i += 4;
        }

        double numberButtonA;
        double numberButtonB;
        Equation firstEquation;
        Equation secondEquation;

        long output = 0;
        i = 0;

        foreach (EquationSystem equationSystem in equationsSystems)
        {
            equationSystem.Solve();

            double error = 0.01;

            firstEquation = equationSystem.Equations[0];
            secondEquation = equationSystem.Equations[1];
            numberButtonB = secondEquation.IndependentTerm / secondEquation.Coefficients[1];
            numberButtonA =
                (firstEquation.IndependentTerm - firstEquation.Coefficients[1] * numberButtonB) /
                            firstEquation.Coefficients[0];

            if (Math.Abs(numberButtonA - Math.Round(numberButtonA)) < error &&
                Math.Abs(numberButtonB - Math.Round(numberButtonB)) < error &&
                numberButtonA >= 0 && numberButtonB >= 0)
            {
                output += (long)(Math.Round(numberButtonA) * tokensForButtonA +
                                Math.Round(numberButtonB) * tokensForButtonB);
            }

            i++;
        }

        return output;
    }

    public override long GetSolution_2(string fileName)
    {
        string[] lines = _fileReader.ReadAllLines(GetFullFilePath(fileName));

        int i = 0;
        double[] firstEquationCoefficients;
        double[] secondEquationCoefficients;
        double[] prices;

        const long tokensForButtonA = 3;
        const long tokensForButtonB = 1;

        List<EquationSystem> equationsSystems = [];

        while (i < lines.Length)
        {
            firstEquationCoefficients = lines[i].Split(":")[1].Split(",")
                .Select(x => double.Parse(x.Trim().Substring(2))).ToArray();

            secondEquationCoefficients = lines[i + 1].Split(":")[1].Split(",")
                .Select(x => double.Parse(x.Trim().Substring(2))).ToArray();

            prices = lines[i + 2].Split(":")[1].Split(",")
                .Select(x => double.Parse(x.Trim().Substring(2)) + 10000000000000).ToArray();

            equationsSystems.Add
            (
                new EquationSystem
                (
                    [new Equation([firstEquationCoefficients[0], secondEquationCoefficients[0]], prices[0]),
                    new Equation([firstEquationCoefficients[1], secondEquationCoefficients[1]], prices[1])]
                )
            );

            i += 4;
        }

        double numberButtonA;
        double numberButtonB;
        Equation firstEquation;
        Equation secondEquation;

        long output = 0;
        i = 0;

        foreach (EquationSystem equationSystem in equationsSystems)
        {
            equationSystem.Solve();

            double error = 0.01;

            firstEquation = equationSystem.Equations[0];
            secondEquation = equationSystem.Equations[1];
            numberButtonB = secondEquation.IndependentTerm / secondEquation.Coefficients[1];
            numberButtonA =
                (firstEquation.IndependentTerm - firstEquation.Coefficients[1] * numberButtonB) /
                            firstEquation.Coefficients[0];

            if (Math.Abs(numberButtonA - Math.Round(numberButtonA)) < error &&
                Math.Abs(numberButtonB - Math.Round(numberButtonB)) < error &&
                numberButtonA >= 0 && numberButtonB >= 0)
            {
                output += (long)(Math.Round(numberButtonA) * tokensForButtonA +
                                Math.Round(numberButtonB) * tokensForButtonB);
            }

            i++;
        }

        return output;
    }

    public class Equation(List<double> Coefficients, double IndependentTerm)
    {
        public List<double> Coefficients = Coefficients;
        public double IndependentTerm = IndependentTerm;
    }

    public class EquationSystem
    {
        public List<Equation> Equations { get; set; }

        public EquationSystem(List<Equation> equations)
        {
            if (equations.DistinctBy(x => x.Coefficients.Count).Distinct().Count() > 1)
            {
                throw new ArgumentException("All equations must have the same number of coefficients");
            }

            Equations = equations;
        }

        public bool IsReduced()
        {
            int prevIndexFirstNotNull = -1;

            for (int i = 0; i < Equations.Count; i++)
            {
                int firstIndexNotNull = Equations[i].Coefficients.FindIndex(x => x != 0);
                if (firstIndexNotNull == -1 && i < Equations.Count - 1)
                {
                    return false;
                }
                if (prevIndexFirstNotNull != -1 && firstIndexNotNull <= prevIndexFirstNotNull)
                {
                    return false;
                }
                prevIndexFirstNotNull = firstIndexNotNull;
            }

            return true;
        }

        public void Solve()
        {
            int pivotRow = 0;
            int pivotCol = 0;

            int nCoefficients = Equations[0].Coefficients.Count;

            while(pivotCol < nCoefficients)
            {
                Equations = Equations.OrderBy(equation => equation.Coefficients.TakeWhile(x => x == 0).Count()).ToList();
                if (IsReduced())
                {
                    return;
                }

                int lastIndexOfNotNullElement = Equations.FindLastIndex(eq => eq.Coefficients[pivotCol] != 0);

                if (Equations[pivotRow].Coefficients[pivotCol] == 0 && lastIndexOfNotNullElement > pivotRow)
                {
                    (Equations[pivotRow], Equations[lastIndexOfNotNullElement]) =
                        (Equations[lastIndexOfNotNullElement], Equations[pivotRow]);
                    continue;
                }

                double multiplier;

                for(int equationIndex = pivotRow+1; equationIndex < Equations.Count; equationIndex++)
                {
                    multiplier = -Equations[equationIndex].Coefficients[pivotCol] /
                        Equations[pivotRow].Coefficients[pivotCol];

                    for (int element = 0; element < nCoefficients; element++)
                    {
                        Equations[lastIndexOfNotNullElement].Coefficients[element] +=
                            Equations[pivotRow].Coefficients[element] * multiplier;
                    }
                    Equations[lastIndexOfNotNullElement].IndependentTerm +=
                        Equations[pivotRow].IndependentTerm * multiplier;
                }

                pivotRow++;
                pivotCol++;
            }
        }
    }
}
