# NativeFunctionLoader
A library loader for loading native functions as unmanaged delegate pointers to archive the best performance

Platforms supported:
- Windows
- MacOS
- Linux

Supported NET Version:
- NET 6

Code example:

```CSharp
 public static unsafe class Program {
        [NativeFunction(Platforms.Windows, "user32.dll")]
        public static readonly delegate* unmanaged<void*, char*, char*, uint, int> MessageBoxW;

        static Program() {
            NativeFunctionLoader.Load(typeof(Program));
        }
        
        public static void Main(string[] args) {
            var caption = (char*)Marshal.StringToHGlobalUni("Information").ToPointer();
            var message = (char*)Marshal.StringToHGlobalUni("Hello world!");
            
            MessageBoxW(null, message, caption, 0);
            
            Marshal.FreeHGlobal(new IntPtr(caption));
            Marshal.FreeHGlobal(new IntPtr(message));
            
            NativeFunctionLoader.Free();
        }
    }
```