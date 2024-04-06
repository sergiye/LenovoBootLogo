using System;
using System.Windows.Forms;

namespace PCBootLogo {

  internal static class Program {

    [STAThread]
    private static void Main() {

      if (!ApiMethods.InitUnmanagedLibrary()) {
        MessageBox.Show("Unable to initialize api library.", LogoModel.AppTitle, MessageBoxButtons.OK,
          MessageBoxIcon.Warning);
        Environment.Exit(-1);
        return;
      }

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm());
    }
  }
}