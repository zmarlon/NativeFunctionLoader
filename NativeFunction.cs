namespace NativeFunctionLoader;

/// <summary>
/// An attribute which marks fields for the function injection
/// The field has to be public static readonly
/// <remarks>
/// <b>Authors:</b> Marlon Klaus<br></br>
/// <b>Created by:</b> Marlon Klaus<br></br>
/// <b>Created on:</b> 10/27/2021<br></br>
/// <b>Since:</b> 1.0.0.0
/// </remarks>
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class NativeFunction : Attribute {
    internal Platforms Platforms { get; }
    internal string[] LibraryNames { get; }
    internal string? CustomFunctionName { get; }
    internal bool HasToExist { get; }

    /// <summary>
    /// The NativeFunction attribute constructor
    /// <remarks>
    /// <b>Authors:</b> Marlon Klaus<br></br>
    /// <b>Created by:</b> Marlon Klaus<br></br>
    /// <b>Created on:</b> 10/27/2021<br></br>
    /// <b>Since:</b> 1.0.0.0
    /// </remarks>
    /// </summary>
    /// <param name="platforms">This function should be injected on this platforms</param>
    /// <param name="libraryName">The name of the library</param>
    /// <param name="customFunctionName">A custom name for the native function. Set this to null to use the name of the field</param>
    /// <param name="hasToExist">If this is set to false, no exception will be thrown when a function fails to inject</param>
    public NativeFunction(Platforms platforms, string libraryName, string? customFunctionName = null,
        bool hasToExist = true) : this(platforms, new[] { libraryName }, customFunctionName, hasToExist) {
    }

    /// <summary>
    /// The NativeFunction attribute constructor
    /// <remarks>
    /// <b>Authors:</b> Marlon Klaus<br></br>
    /// <b>Created by:</b> Marlon Klaus<br></br>
    /// <b>Created on:</b> 10/27/2021<br></br>
    /// <b>Since:</b> 1.0.0.0
    /// </remarks>
    /// </summary>
    /// <param name="platforms">This function should be injected on this platforms</param>
    /// <param name="libraryNames">Possible names of the library</param>
    /// <param name="customFunctionName">A custom name for the native function. Set this to null to use the name of the field</param>
    /// <param name="hasToExist">If this is set to false, no exception will be thrown when a function fails to inject</param>
    public NativeFunction(Platforms platforms, string[] libraryNames, string? customFunctionName = null,
        bool hasToExist = true) {
        Platforms = platforms;
        LibraryNames = libraryNames;
        CustomFunctionName = customFunctionName;
        HasToExist = hasToExist;
    }
}