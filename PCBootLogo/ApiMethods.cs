using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PCBootLogo {

  public static class ApiMethods {
  
    [DllImport("AIToolAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetLogoDIYInfo(ref byte enable, ref uint format, ref int height, ref int width);

    [DllImport("AIToolAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetLogoDIYInfo(byte enable);

    [DllImport("AIToolAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetLogDIYCRC(string filepath);

    [DllImport("AIToolAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int IsEditionCircle();

    private static string ApiLibraryDllPath = Path.Combine(Path.GetTempPath(), "AIToolAPI.dll");
    private static IntPtr ApiLibraryDllHandler = IntPtr.Zero;

    public static bool InitUnmanagedLibrary() {
      using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("PCBootLogo.AIToolAPI.dll")) {
        try {
          using (var outFile = File.Create(ApiLibraryDllPath)) {
            const int sz = 4096;
            var buf = new byte[sz];
            while (true) {
              var nRead = resource.Read(buf, 0, sz);
              if (nRead < 1)
                break;
              outFile.Write(buf, 0, nRead);
            }
          }

          ApiLibraryDllHandler = LoadLibrary(ApiLibraryDllPath);
          return ApiLibraryDllHandler != IntPtr.Zero;
        }
        catch {
          return false;
        }
      }
    }

    public static void ReleaseUnmanagedLibrary() {
      //foreach (ProcessModule mod in Process.GetCurrentProcess().Modules) {
      //  if (mod.ModuleName == ApiLibraryDllPath) FreeLibrary(mod.BaseAddress);
      //}
      if (ApiLibraryDllHandler != IntPtr.Zero) {
        FreeLibrary(ApiLibraryDllHandler);
        FreeLibrary(ApiLibraryDllHandler); //twice to decrease the reference count to 0
      }
      if (!string.IsNullOrEmpty(ApiLibraryDllPath)) {
        try {
          File.Delete(ApiMethods.ApiLibraryDllPath);
        }
        catch {
          //ignore
        }
      }
    }

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern IntPtr LoadLibrary(string lpFileName);
    
    [DllImport("kernel32", SetLastError = true)]
    internal static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
  }
}