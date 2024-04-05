using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace PCBootLogo {

  public class LogoModel {
    
    private enum FileExtension {
      [EnumDisplay(".jpg")] Jpg = 255216,
      // [EnumDisplay(".gif")] GIF = 7173,
      [EnumDisplay(".png")] Png = 13780,
      [EnumDisplay(".bmp")] Bmp = 19778,
      [EnumDisplay(".bmp")] Bmp2 = 6677,
      [EnumDisplay(".jpeg")] Jpeg = 65496
    }

    private int defaultHeight = 1080;

    private readonly string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/pc.png");

    private int defaultWidth = 1920;

    private string fileExtension = "";

    private string filter2 = "*.png;*.jpg;*.bmp";

    private string name;

    public string EFILogoPath { get; set; } = "EFI\\Lenovo\\Logo"; //todo: support other vendors

    public bool DisplayLoadingIco { get; set; } = true;

    public bool VisibleLoadingIco { get; set; }

    public string Filter { get; private set; } = "*.png,*.jpg,*.bmp";

    public double ImageHeight { get; set; } = 140.0;

    public double ImageWidth { get; set; } = 224.0;

    public bool ShowWarning { get; set; }

    public string ShowWarnInfo { get; set; }

    public bool ShowSuccessTip { get; set; }

    public string ShowSuccessText { get; set; } = "";

    public bool UiIsEnable { get; set; } = true;

    public bool FunEnable { get; set; } = true;

    public long DiskFreeSpace { get; private set; } = 54525952L;

    public bool CanRecovery { get; set; }

    public int DefaultHeight { get => defaultHeight; set => defaultHeight = value; }

    public int DefaultWidth { get => defaultWidth; set => defaultWidth = value; }

    public string ImagePath1 { get; set; }

    public void CreateViewData() {
      ImageHeight = 140.0;
      ImageWidth = 224.0;
      ShowSuccessTip = false;
      ShowWarning = false;
      GetLogoInfo();
      VisibleLoadingIco = GetLoadingCircle();
      if (VisibleLoadingIco) GetShowLoadingIco();
    }

    private static bool GetLoadingCircle() {
      var build = Environment.OSVersion.Version.Build;
      Console.WriteLine($"ostype = {build}");
      if (build <= 14393) return false;
      var num = ApiMethods.IsEditionCircle();
      Console.WriteLine($"IsEditionCircle = {num}");
      return num != 0;
    }

    private void GetShowLoadingIco() {
      var ptr = default(IntPtr);
      try {
        ApiMethods.Wow64DisableWow64FsRedirection(ref ptr);
        var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        process.StandardInput.WriteLine("bcdedit /enum all" + "&exit");
        process.StandardInput.AutoFlush = true;
        var standardOutput = process.StandardOutput;
        var flag = false;
        while (!standardOutput.EndOfStream) {
          var text2 = standardOutput.ReadLine();
          if (text2.Contains("identifier") && text2.Contains("{globalsettings}"))
            flag = true;
          if (!flag || !text2.Contains("bootuxdisabled")) continue;
          Console.WriteLine("gobalsettings: " + text2);
          var text3 = text2.Replace("bootuxdisabled", "").Trim();
          DisplayLoadingIco = text3.Contains("No");
          Console.WriteLine($"{text2}; DisplayLoadingIco = {DisplayLoadingIco}");
          flag = false;
        }

        process.WaitForExit();
        process.Close();
      }
      catch (Exception ex) {
        // ignored
      }
      finally {
        ApiMethods.Wow64RevertWow64FsRedirection(ptr);
      }
    }

    private static long GetHardDiskFreeSpace(string diskName) {
      try {
        diskName += ":\\";
        var drives = DriveInfo.GetDrives();
        foreach (var driveInfo in drives) {
          Console.WriteLine($"{driveInfo.Name}");
          if (driveInfo.Name == diskName) return driveInfo.TotalFreeSpace;
        }
      }
      catch (Exception) {
        // ignored
      }

      return 0L;
    }

    private void GetDiskFree(long freeM) {
      const long num = 33554432L;
      try {
        DiskFreeSpace = freeM - num;
        Console.WriteLine($"diskFreeSpace = {DiskFreeSpace}; freeTotal = {freeM};  _32M = {num} ");
      }
      catch (Exception ex) {
        Console.WriteLine(ex);
      }
    }

    private bool IsSizeExceed(string path) {
      Image image = null;
      try {
        // Path.GetExtension(path);
        image = Image.FromFile(path);
        double num = image.Width;
        double num2 = image.Height;
        return num > DefaultWidth || num2 > DefaultHeight;
      }
      catch (Exception) {
        return false;
      }
      finally {
        image?.Dispose();
      }
    }

    private bool SetImageSize(string path) {
      try {
        // Path.GetExtension(path);
        using (var image = Image.FromFile(path)) {
          double num = image.Width;
          double num2 = image.Height;
          Console.WriteLine($"img h = {num2}; w = {num}");
          ImageHeight = 140.0 * num2 / defaultHeight;
          ImageWidth = 224.0 * num / defaultWidth;
          return true;
        }
      }
      catch (Exception) {
        return false;
      }
    }

    private void SetImagePath(bool defaultImage) {
      var tempPath = Path.GetTempPath();
      try {
        var text = ChangeEfiDisk(true);
        var hardDiskFreeSpace = GetHardDiskFreeSpace(text);
        var path = Path.Combine(text + ":", EFILogoPath);
        if (!defaultImage && Directory.Exists(path)) {
          var files = Directory.GetFiles(path);
          if (files.Length != 0) {
            var text2 = files[0];
            tempPath = Path.Combine(tempPath, Path.GetFileName(text2));
            File.Copy(text2, tempPath, true);
            SetImageSize(tempPath);
            GetBitmapImage(tempPath);
          }
        }
        else {
          GetBitmapImage(defaultPath);
        }

        GetDiskFree(hardDiskFreeSpace);
      }
      catch (Exception ex) {
        Console.WriteLine($"SetImagePath error {ex.Message}");
      }
      finally {
        ChangeEfiDisk(false);
      }
    }

    public void ToRecovery() {
      try {
        name = ChangeEfiDisk(true);
        var text = Path.Combine(name + ":\\", EFILogoPath);
        DeleteOtherDirectory(text);
      }
      catch (Exception ex) {
        Console.WriteLine(ex);
      }
      finally {
        ChangeEfiDisk(false);
      }

      var num = ApiMethods.SetLogoDIYInfo(0);
      Console.WriteLine($"set logoinfo ret = {num}");
      if (num == 0) {
        Console.WriteLine($"set logoinfo error: ret = {num}");
        return;
      }

      ShowSuccessText = "Done! Restored to default settings.";
      ShowSuccessTip = true;
      FunEnable = false;
    }

    private void GetLogoInfo() {
      var format = 0u;
      byte enable = 0;
      var info = ApiMethods.GetLogoDIYInfo(ref enable, ref format, ref defaultHeight, ref defaultWidth);
      Console.WriteLine($"ret1 = {info}; Height = {defaultHeight}; Width = {defaultWidth}");
      if (info != 0) {
        ChangeSupportingFormat(format);
        if (defaultHeight < 0 || defaultWidth < 0) {
          Console.WriteLine("get height/width error, set default 1920*1080");
          DefaultWidth = 1920;
          DefaultHeight = 1080;
        }

        Console.WriteLine($"get logoinfo -> enable = {enable}; type = {format}; ");
        var flag = enable == 0;
        SetImagePath(flag);
        CanRecovery = !flag;
        FunEnable = !flag;
        UiIsEnable = true;
      }
      else {
        GetBitmapImage(defaultPath);
        Console.WriteLine("get logo_info error:");
        UiIsEnable = false;
        FunEnable = false;
        CanRecovery = false;
      }
    }

    public void SaveLogoClick(bool stretch = false) {
      try {
        Console.WriteLine("SaveLogoClick");
        var num = ApiMethods.SetLogoDIYInfo(1);
        Console.WriteLine($"set logoinfo ret = {num}");
        if (num == 0) {
          ShowSuccessTip = false;
          Console.WriteLine($"set logoinfo error: ret = {num}");
          return;
        }

        Console.WriteLine($"set logoinfo success:  ret = {num}");
        name = ChangeEfiDisk(true);
        string destPath;
        try {
          var extension = Path.GetExtension(ImagePath1);
          if (!string.IsNullOrWhiteSpace(fileExtension) && extension != fileExtension) extension = fileExtension;
          destPath = Path.Combine(name + ":\\", EFILogoPath, $"mylogo_{DefaultWidth}x{DefaultHeight}" + extension);
          Console.WriteLine($"path = {destPath}");
          DeleteOtherFile(destPath);
          Console.WriteLine($"source path = {ImagePath1}; dest path = {destPath}");
          File.Copy(ImagePath1, destPath);
        }
        catch (Exception ex) {
          Console.WriteLine("copy file error:" + ex.Message);
          return;
        }

        if (ApiMethods.SetLogDIYCRC(destPath) > 0) {
          Console.WriteLine("Set CRC success");
          ChangeEfiDisk(false);
          ShowSuccessText = "Success! You can restart to view the new boot logo now.";
          ShowSuccessTip = true;
        }
        else {
          Console.WriteLine("Set CRC ERROR");
          ShowSuccessTip = false;
        }
      }
      catch (Exception ex2) {
        Console.WriteLine(ex2);
        ShowSuccessTip = false;
      }
      finally {
        CanRecovery = true;
        ChangeEfiDisk(false);
      }
    }

    private static void DeleteOtherDirectory(string path) {
      if (Directory.Exists(path)) Directory.Delete(path, true);
    }

    private static void DeleteOtherFile(string path) {
      var directoryName = Path.GetDirectoryName(path);
      Console.WriteLine($"delete directory {directoryName}");
      if (Directory.Exists(directoryName)) {
        Console.WriteLine("Delete directory error, to delete file");
        var files = Directory.GetFiles(directoryName);
        foreach (var text in files) {
          Console.WriteLine($"delete other file {text}");
          File.Delete(text);
        }
      }
      else {
        Console.WriteLine($"Create new directory {directoryName}");
        Directory.CreateDirectory(directoryName);
      }
    }

    private static string GetDiskName() {
      var logicalDrives = Directory.GetLogicalDrives();
      for (var i = 65; i < 91; i++) {
        var aSciiEncoding = new ASCIIEncoding();
        var bytes = new byte[] { (byte) i };
        var @string = aSciiEncoding.GetString(bytes);
        if (!logicalDrives.Contains(@string)) return @string;
      }
      return "";
    }

    private static string ChangeEfiDisk(bool mount) {
      var diskName = GetDiskName();
      try {
        var arg = mount ? "/s" : "/d";
        var text = $"mountvol {diskName}: {arg}";
        Console.WriteLine(text);
        var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        process.StandardInput.WriteLine(text + "&exit");
        process.StandardInput.AutoFlush = true;
        process.WaitForExit();
        process.Close();
        return diskName;
      }
      catch (Exception ex) {
        Console.WriteLine(ex);
        return diskName;
      }
    }

    public bool ChangeLodingIco(bool isShow) {
      var ptr = default(IntPtr);
      try {
        ApiMethods.Wow64DisableWow64FsRedirection(ref ptr);
        string text;
        text = !isShow
          ? "bcdedit.exe -set {globalsettings} bootuxdisabled on"
          : "bcdedit.exe -set {globalsettings} bootuxdisabled off";
        Console.WriteLine(text);
        var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        process.StandardInput.WriteLine(text + "&exit");
        process.StandardInput.AutoFlush = true;
        process.WaitForExit();
        process.Close();
        return true;
      }
      catch (Exception ex) {
        Console.WriteLine(ex);
        return false;
      }
      finally {
        ApiMethods.Wow64RevertWow64FsRedirection(ptr);
      }
    }

    private bool ImageCheck(string path) {
      using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
        using (var binaryReader = new BinaryReader(fileStream)) {
          var empty = string.Empty;
          if (!IsImage(path)) return false;
          try {
            // Path.GetExtension(path);
            empty += binaryReader.ReadByte();
            empty += binaryReader.ReadByte();
            var extension = (FileExtension) Enum.Parse(typeof(FileExtension), empty);
            switch (extension) {
              case FileExtension.Bmp2:
              case FileExtension.Png:
              case FileExtension.Bmp:
              case FileExtension.Jpeg:
              case FileExtension.Jpg:
                fileExtension = extension.Display();
                return true;
              default:
                return false;
            }
          }
          catch (Exception ex) {
            Console.WriteLine(ex);
            return false;
          }
        }
      }
    }

    private static bool IsImage(string path) {
      try {
        var text = Path.GetExtension(path).ToLower();
        if (text == ".tga" || text == ".pcx") return true;
        using (var image = Image.FromFile(path)) {
          return true;
        }
      }
      catch (Exception) {
        return false;
      }
    }

    public void SelectedImageClick() {
      ShowSuccessTip = false;
      var openFileDialog = new OpenFileDialog();
      openFileDialog.Multiselect = false;
      openFileDialog.Title = "Please select an image";
      openFileDialog.Filter = $"Image({Filter})|{filter2}";
      var res = openFileDialog.ShowDialog();
      if (res != DialogResult.OK) return;
      CanRecovery = false;
      FunEnable = false;
      var fileName = openFileDialog.FileName;
      var fileInfo = new FileInfo(fileName);
      if (!ImageCheck(fileName)) {
        ShowWarning = true;
        ShowWarnInfo = "The selected file is not an image!";
        return;
      }

      if (fileInfo.Length > DiskFreeSpace) {
        var num = (int) (DiskFreeSpace / 1024 / 1024);
        ShowWarnInfo = $"Image must not exceed {num}MB!";
        ShowWarning = true;
        return;
      }

      if (IsSizeExceed(fileName)) {
        ShowWarnInfo = "The image exceeds the maximum resolution!";
        ShowWarning = true;
        return;
      }

      GetBitmapImage(fileName);
      SetImageSize(fileName);
      FunEnable = true;
      ShowWarning = false;
      Console.WriteLine($"SelectedImage: height = {ImageHeight}; width = {ImageWidth}");
    }

    private void GetBitmapImage(string imagePath) {
      try {
        ImagePath1 = imagePath;
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.UriSource = new Uri(imagePath);
        bitmapImage.EndInit();
        bitmapImage.Freeze();
      }
      catch (Exception ex) {
        Console.WriteLine(ex);
      }
    }

    private void ChangeSupportingFormat(uint format) {
      Filter = "";
      filter2 = "";
      if ((1u & format) != 0) {
        Filter += "*.jpg,";
        filter2 += "*.jpg;";
      }

      if ((0x10u & format) != 0) {
        Filter += "*.bmp,";
        filter2 += "*.bmp;";
      }

      if ((0x20u & format) != 0) {
        Filter += "*.png,";
        filter2 += "*.png;";
      }

      Filter = Filter.Substring(0, Filter.Length - 1);
      filter2 = filter2.Substring(0, filter2.Length - 1);
    }

    // private enum ButtonStyle {
    //   JPG = 1,
    //   TGA = 2,
    //   PCX = 4,
    //   GIF = 8,
    //   BMP = 0x10,
    //   PNG = 0x20
    // }
  }
}