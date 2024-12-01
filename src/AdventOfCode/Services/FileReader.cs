namespace AdventOfCode.Services;

public class FileReader : IFileReader
{
    public string Read(string fileName)
    {
        string path = Path.Combine(fileName);
        return File.ReadAllText(path);
    }

    public string[] ReadAllLines(string fileName)
    {
        string path = Path.Combine(fileName);
        return File.ReadAllLines(path);
    }
}
