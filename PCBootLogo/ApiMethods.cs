using System;
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

    public static bool InitUnmanagedLibrary() {
      var dllPath = Path.Combine(Path.GetTempPath(), "AIToolAPI.dll");
      using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("PCBootLogo.AIToolAPI.dll")) {
        try {
          using (var outFile = File.Create(dllPath)) {
            const int sz = 4096;
            var buf = new byte[sz];
            while (true) {
              var nRead = resource.Read(buf, 0, sz);
              if (nRead < 1)
                break;
              outFile.Write(buf, 0, nRead);
            }
          }
          return LoadLibrary(dllPath) != IntPtr.Zero;
        }
        catch {
          return false;
        }
      }
    }

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern IntPtr LoadLibrary(string lpFileName);
    
    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
  }
}