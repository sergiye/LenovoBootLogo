using System;
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

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
  }
}