namespace AdventOfCode.Services;

public interface IFactory<T>
{
    public T CreateInstance(string name);
}
