using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeFunctionLoader;

/// <summary>
/// This exception is thrown when a function fails to inject
/// <remarks>
/// <b>Authors:</b>Marlon Klaus<br></br>
/// <b>Created by:</b>Marlon Klaus<br></br>
/// <b>Created on:</b>10/27/2021<br></br>
/// <b>Since:</b> 1.0.0.0
/// </remarks>
/// </summary>
public sealed class NativeFunctionLoadingException : Exception {
    private static string BuildExceptionMessage(string[] libraryNames, string functionName) {
        var builder = new StringBuilder().Append(
            $"Failed to load function {functionName} from one of this following libraries: ");
        foreach (var libraryName in libraryNames) {
            builder.Append($"{libraryName} ");
        }

        return builder.ToString();
    }

    internal NativeFunctionLoadingException(string[] libraryNames, string functionName) : base(
        BuildExceptionMessage(libraryNames, functionName)) {
    }
}

/// <summary>
/// This function loader can inject native functions into unmanaged delegate pointers
/// <remarks>
/// <b>Authors:</b>Marlon Klaus<br></br>
/// <b>Created by:</b>Marlon Klaus<br></br>
/// <b>Created on:</b>10/27/2021<br></br>
/// <b>Since:</b> 1.0.0.0
/// </remarks>
/// </summary>
public static class NativeFunctionLoader {
    private static readonly Platforms CurrentPlatform = DetectPlatform();
    private static readonly Dictionary<string, IntPtr> Libraries = new();

    private static Platforms DetectPlatform() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return Platforms.Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return Platforms.Mac;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return Platforms.Linux;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) return Platforms.BSD;
        throw new Exception("Unsupported platform");
    }

    private static IntPtr GetFunction(string[] libraryNames, string functionName) {
        var library = IntPtr.Zero;
        foreach (var libraryName in libraryNames) {
            if (Libraries.TryGetValue(libraryName, out library)) break;
        }

        if (library == IntPtr.Zero) {
            foreach (var libraryName in libraryNames) {
                if (NativeLibrary.TryLoad(libraryName, out library)) {
                    Libraries.Add(libraryName, library);
                    break;
                }
            }
        }

        if (library == IntPtr.Zero) return IntPtr.Zero;

        NativeLibrary.TryGetExport(library, functionName, out IntPtr function);
        return function;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidField(FieldInfo field) {
        return field.IsPublic && field.IsStatic && field.IsInitOnly;
    }

    private static void LoadFunction(FieldInfo field, NativeFunction attribute) {
        if (!attribute.Platforms.HasFlag(CurrentPlatform)) return;
        var functionName = attribute.CustomFunctionName ?? field.Name;
        var function = GetFunction(attribute.LibraryNames, functionName);
        if (function == IntPtr.Zero && attribute.HasToExist) {
            throw new NativeFunctionLoadingException(attribute.LibraryNames, functionName);
        }

        field.SetValue(null, function);
    }

    /// <summary>
    /// Injects native functions into all fields of the given type containing an NativeFunction attribute
    /// This method has to be called from the static constructor of the given type
    /// </summary>
    /// <remarks>
    /// <b>Authors:</b>Marlon Klaus<br></br>
    /// <b>Created by:</b>Marlon Klaus<br></br>
    /// <b>Created on:</b>10/27/2021<br></br>
    /// <b>Since:</b> 1.0.0.0
    /// </remarks>
    /// <param name="type">The type</param>
    public static void Load(Type type) {
        foreach (var field in type.GetFields()) {
            var attribute = field.GetCustomAttribute<NativeFunction>();
            if (attribute == null) continue;
            if (!IsValidField(field)) continue;

            LoadFunction(field, attribute);
        }
    }

    /// <summary>
    /// Frees all native libraries
    /// <remarks>
    /// <b>Authors:</b>Marlon Klaus<br></br>
    /// <b>Created by:</b>Marlon Klaus<br></br>
    /// <b>Created on:</b>10/27/2021<br></br>
    /// <b>Since:</b> 1.0.0.0
    /// </remarks>
    /// </summary>
    public static void Free() {
        foreach (var library in Libraries.Values) {
            NativeLibrary.Free(library);
        }
    }
}