namespace AdventOfCode.Services;

public class AppSettings
{
    public string SolutionFullName { get; set; } = "AdventOfCode.Solutions.Y{year}.Day{day}.Solution";
    public string MethodPrefix { get; set; } = "GetSolution_";
    public string DataDirectory { get; set; } = "Data";
    public string InputFilePrefix { get; set; } = "input-";
    public string InputTestFilePrefix { get; set; } = "test-";
    public string InputFileSuffix { get; set; } = ".txt";
}
