using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Services;

/// <summary>
/// A factory to instantiate classes of a certain type
/// </summary>
/// <typeparam name="T">The type of the class that is going to be instantiated</typeparam>
public class Factory<T>(IServiceProvider serviceProvider) : IFactory<T> where T : class
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    /// <summary>
    /// Creates an instance of a certain class given its name, using Reflection.
    /// </summary>
    /// <param name="name">Name of the class to be instantiated</param>
    /// <returns>An instance with the specified </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="name"/> is null or empty or if the class with name equal <paramref name="name"/>
    /// can not be instantiated
    /// </exception>
    public virtual T CreateInstance(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        T factory = Instantiate(name) ?? throw new ArgumentException("The specified class can not be instantiated");
        return factory;
    }

    /// <summary>
    /// Search in the assembly of the type T, and if exists a class of type T with the given name, 
    /// an instance is created and returned
    /// </summary>
    /// <param name="className">Name of the class to be instantiated</param>
    /// <returns>An instance of type T of the class with name <paramref name="className"/>or
    /// null if no class of type T with the name <paramref name="className"/> exists
    /// </returns>
    private T? Instantiate(string className)
    {
        Type typeImplemented = typeof(T);

        //Get assembly of type T and get Types of the assembly, then search for a type with the specified name
        Type? selectedType = Assembly.GetAssembly(typeof(T))?
                .GetTypes()
                .FirstOrDefault(t => typeImplemented.IsAssignableFrom(t) &&
                                    t.FullName == className);

        if (selectedType is null)
        {
            return default;
        }

        return (T?)ActivatorUtilities.CreateInstance(_serviceProvider, selectedType);
    }
}
