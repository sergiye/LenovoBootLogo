using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PCBootLogo {

  public class LogoModel {

    public const string AppTitle = "PC Boot Logo";

    private string GetTempLogoFilePath(string extension) {
      return $"mylogo_{DefaultWidth}x{DefaultHeight}{extension}";
    }

    private enum FileExtension {
      [EnumDisplay(".jpg")] Jpg = 255216,
      // [EnumDisplay(".gif")] GIF = 7173,
      [EnumDisplay(".png")] Png = 13780,
      [EnumDisplay(".bmp")] Bmp = 19778,
      [EnumDisplay(".bmp")] Bmp2 = 6677,
      [EnumDisplay(".jpeg")] Jpeg = 65496
    }

    private string fileExtension = "";
    
    private string name;

    private string EFILogoPath { get; set; } = "EFI\\Lenovo\\Logo"; //todo: support other vendors

    public bool DisplayLoadingIco { get; private set; } = true;

    public bool VisibleLoadingIco { get; private set; }

    public string Filter { get; private set; } = "*.png,*.jpg,*.bmp";

    private double ImageHeight { get; set; } = 140.0;

    private double ImageWidth { get; set; } = 224.0;

    public bool ShowWarning { get; set; }

    public string ShowWarnInfo { get; private set; }

    public bool ShowSuccessTip { get; set; }

    public string ShowSuccessText { get; private set; } = "";

    public bool UiIsEnable { get; private set; } = true;

    public bool FunEnable { get; private set; } = true;

    private long DiskFreeSpace { get; set; } = 54525952L;

    public bool CanRecovery { get; private set; }

    private int defaultHeight = 1080;
    public int DefaultHeight { 
      get => defaultHeight;
      private set => defaultHeight = value; 
    }

    private int defaultWidth = 1920;
    public int DefaultWidth { 
      get => defaultWidth;
      private set => defaultWidth = value; 
    }

    public string ImagePath { get; private set; }

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
          var line = standardOutput.ReadLine();
          if (line.Contains("identifier") && line.Contains("{globalsettings}"))
            flag = true;
          if (!flag || !line.Contains("bootuxdisabled")) continue;
          Console.WriteLine("gobalsettings: " + line);
          var text3 = line.Replace("bootuxdisabled", "").Trim();
          DisplayLoadingIco = text3.Contains("No");
          Console.WriteLine($"{line}; DisplayLoadingIco = {DisplayLoadingIco}");
          flag = false;
        }

        process.WaitForExit();
        process.Close();
      }
      catch (Exception ex) {
        Console.WriteLine(ex.Message);
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
      catch (Exception ex) {
        Console.WriteLine(ex.Message);
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
      try {
        using (var image = Image.FromFile(path))
          return image.Width > DefaultWidth || image.Height > DefaultHeight;
      }
      catch (Exception) {
        return false;
      }
    }

    private void SetCurrentImage(string path) {
      if (File.Exists(path)) {
        using (var image = Image.FromFile(path)) {
          Console.WriteLine($"img h = {image.Height}; w = {image.Width}");
          ImageHeight = 140.0 * image.Height / defaultHeight;
          ImageWidth = 224.0 * image.Width / defaultWidth;
        }
      }
      ImagePath = path;
    }

    private void SetImagePath(bool defaultImage) {
      try {
        var efiDisk = ChangeEfiDisk(true);
        var hardDiskFreeSpace = GetHardDiskFreeSpace(efiDisk);
        var path = Path.Combine(efiDisk + ":", EFILogoPath);
        if (!defaultImage && Directory.Exists(path)) {
          var fileName = Directory.GetFiles(path).FirstOrDefault();
          if (!string.IsNullOrEmpty(fileName)) {
            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(fileName));
            File.Copy(fileName, tempPath, true);
            SetCurrentImage(tempPath);
          }
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
        SetCurrentImage(null);
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
        Console.WriteLine("get logo_info error:");
        UiIsEnable = false;
        FunEnable = false;
        CanRecovery = false;
      }
    }
    
    public void SaveLogoClick() {
      try {

        if (string.IsNullOrEmpty(ImagePath)) {
          ShowWarning = true;
          ShowWarnInfo = "No image selected";
          return;
        }

        if (!File.Exists(ImagePath)) {
          ShowWarning = true;
          ShowWarnInfo = $"File '{ImagePath}' is not found.";
          return;
        }
        
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
          var extension = Path.GetExtension(ImagePath);
          if (!string.IsNullOrWhiteSpace(fileExtension) && extension != fileExtension) 
            extension = fileExtension;
          destPath = Path.Combine(name + ":\\", EFILogoPath, GetTempLogoFilePath(extension));
          Console.WriteLine($"path = {destPath}");
          DeleteOtherFile(destPath);
          Console.WriteLine($"source path = {ImagePath}; dest path = {destPath}");
          
          File.Copy(ImagePath, destPath);
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
        var bytes = new[] { (byte) i };
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

    public static bool ChangeLoadingIco(bool isShow) {
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
      openFileDialog.Filter = $"Image({Filter})|{Filter.Replace(',', ';')}";
      if (openFileDialog.ShowDialog() != DialogResult.OK) return;

      CanRecovery = false;
      FunEnable = false;
      var fileName = openFileDialog.FileName;
      if (!ImageCheck(fileName)) {
        ShowWarning = true;
        ShowWarnInfo = "The selected file is not an image!";
        return;
      }

      var tempFile = Path.Combine(Path.GetTempPath(), GetTempLogoFilePath(Path.GetExtension(fileName)));
      File.Copy(fileName, tempFile, true);
      fileName = tempFile; //todo: delete tempFile on app exit
      CheckResizeRequired(fileName, DefaultWidth, DefaultHeight, false);

      var fileInfo = new FileInfo(fileName);
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

      SetCurrentImage(fileName);
      FunEnable = true;
      ShowWarning = false;
      Console.WriteLine($"SelectedImage: height = {ImageHeight}; width = {ImageWidth}");
    }
    
    private static string GetMimeType(ImageFormat imageFormat) {
      var codecs = ImageCodecInfo.GetImageEncoders();
      return codecs.First(codec => codec.FormatID == imageFormat.Guid).MimeType;
    }

    private static ImageFormat GetImageFormatFromMimeType(string contentType, ImageFormat defaultResult) {
      if (GetMimeType(ImageFormat.Jpeg).Equals(contentType, StringComparison.OrdinalIgnoreCase)) {
        return ImageFormat.Jpeg;
      }
      if (GetMimeType(ImageFormat.Bmp).Equals(contentType, StringComparison.OrdinalIgnoreCase)) {
        return ImageFormat.Bmp;
      }
      if (GetMimeType(ImageFormat.Png).Equals(contentType, StringComparison.OrdinalIgnoreCase)) {
        return ImageFormat.Png;
      }

      // foreach (var codecInfo in ImageCodecInfo.GetImageEncoders()) {
      //   if (codecInfo.MimeType.Equals(contentType, StringComparison.OrdinalIgnoreCase)) {
      //     return codecInfo.FormatID;
      //   }
      // }

      return defaultResult;
    }

    private static void CheckResizeRequired(string filePath, int width, int height, bool proportional) {
      Bitmap destImage = null;
      try {
        ImageFormat imgFormat;
        using (var image = Image.FromFile(filePath)) {
          if (proportional) {
            var factor = Math.Min((double)width / image.Width, (double)height / image.Height);
            width = (int)Math.Round(image.Width * factor);
            height = (int)Math.Round(image.Height * factor);
          }

          if (width != image.Width || height != image.Height) {
            if (MessageBox.Show("Resize image to fill screed?", AppTitle, MessageBoxButtons.YesNo,
                  MessageBoxIcon.Question) == DialogResult.No)
              return;
          }
          
          imgFormat = GetImageFormatFromMimeType(GetMimeType(image.RawFormat), ImageFormat.Png);
          var destRect = new Rectangle(0, 0, width, height);
          destImage = new Bitmap(width, height);
          destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
          using (var graphics = Graphics.FromImage(destImage)) {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            using (var wrapMode = new ImageAttributes()) {
              wrapMode.SetWrapMode(WrapMode.TileFlipXY);
              graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }
          }
        }
        destImage.Save(filePath, imgFormat);
      }
      finally {
        destImage?.Dispose();
      }
    }
    
    private void ChangeSupportingFormat(uint format) {
      Filter = "";
      if ((1u & format) != 0) {
        Filter += "*.jpg,";
      }

      if ((0x10u & format) != 0) {
        Filter += "*.bmp,";
      }

      if ((0x20u & format) != 0) {
        Filter += "*.png,";
      }

      Filter = Filter.Substring(0, Filter.Length - 1);
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