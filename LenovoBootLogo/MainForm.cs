using System;
using System.Windows.Forms;

namespace LenovoBootLogo {
  
  public partial class MainForm : Form {
    
    private readonly LogoModel model = new LogoModel();

    public MainForm() {

      InitializeComponent();

      Load += (s, e) => {
        model.CreateViewData();
        if (!model.UiIsEnable) {
          MessageBox.Show("Unsupported BIOS!\nThis application only supports Lenovo computers with newer BIOS versions.", 
            "Lenovo boot logo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          Application.Exit();
          return;
        }

        if (model.VisibleLoadingIco) {
          cbxShowLoadingIcon.Checked = model.DisplayLoadingIco;
          cbxShowLoadingIcon.Enabled = true;
        }

        lblFormat.Text = $"Format: {model.Filter} / Max: {model.DefaultWidth}x{model.DefaultHeight}";
      };
    }

    private void btnSelectImage_Click(object sender, EventArgs e) {
      model.SelectedImageClick();
      showTip();
    }

    private void btnApply_Click(object sender, EventArgs e) {
      model.SaveLogoClick();
      showTip();
    }

    private void showTip() {
      if (model.ShowWarning) {
        MessageBox.Show(model.ShowWarnInfo, "Lenovo boot logo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        model.ShowWarning = false;
      }

      if (model.ShowSuccessTip) {
        MessageBox.Show(model.ShowSuccessText, "Lenovo boot logo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        model.ShowSuccessTip = false;
      }

      btnApply.Enabled = model.FunEnable;
      btnRecovery.Enabled = model.CanRecovery;
    }

    private void btnRecovery_Click(object sender, EventArgs e) {
      model.ToRecovery();
      showTip();
    }

    private void cbxShowLoadingIcon_CheckedChanged(object sender, EventArgs e) {
      if (!cbxShowLoadingIcon.Enabled) return;
      MessageBox.Show(model.ChangeLodingIco(cbxShowLoadingIcon.Checked) ? "Done!" : "Failed!");
    }
  }
}