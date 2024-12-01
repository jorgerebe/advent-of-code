namespace AdventOfCode.Services;

public interface IFileReader
{
    string Read(string fileName);
    string[] ReadAllLines(string fileName);
}
