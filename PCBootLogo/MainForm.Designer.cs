using System.Windows.Forms;

namespace PCBootLogo
{
    partial class MainForm {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent() {
      this.btnSelectImage = new System.Windows.Forms.Button();
      this.btnApply = new System.Windows.Forms.Button();
      this.btnRecovery = new System.Windows.Forms.Button();
      this.cbxShowLoadingIcon = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.lblFormat = new System.Windows.Forms.Label();
      this.picPreview = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
      this.SuspendLayout();
      // 
      // btnSelectImage
      // 
      this.btnSelectImage.Location = new System.Drawing.Point(11, 51);
      this.btnSelectImage.Margin = new System.Windows.Forms.Padding(2);
      this.btnSelectImage.Name = "btnSelectImage";
      this.btnSelectImage.Size = new System.Drawing.Size(149, 55);
      this.btnSelectImage.TabIndex = 0;
      this.btnSelectImage.Text = "Select Image";
      this.btnSelectImage.UseVisualStyleBackColor = true;
      this.btnSelectImage.Click += new System.EventHandler(this.btnSelectImage_Click);
      // 
      // btnApply
      // 
      this.btnApply.Enabled = false;
      this.btnApply.Location = new System.Drawing.Point(11, 200);
      this.btnApply.Margin = new System.Windows.Forms.Padding(2);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(149, 55);
      this.btnApply.TabIndex = 1;
      this.btnApply.Text = "Apply";
      this.btnApply.UseVisualStyleBackColor = true;
      this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
      // 
      // btnRecovery
      // 
      this.btnRecovery.Location = new System.Drawing.Point(11, 276);
      this.btnRecovery.Margin = new System.Windows.Forms.Padding(2);
      this.btnRecovery.Name = "btnRecovery";
      this.btnRecovery.Size = new System.Drawing.Size(149, 55);
      this.btnRecovery.TabIndex = 2;
      this.btnRecovery.Text = "Reset to default";
      this.btnRecovery.UseVisualStyleBackColor = true;
      this.btnRecovery.Click += new System.EventHandler(this.btnRecovery_Click);
      // 
      // cbxShowLoadingIcon
      // 
      this.cbxShowLoadingIcon.AutoSize = true;
      this.cbxShowLoadingIcon.Enabled = false;
      this.cbxShowLoadingIcon.Location = new System.Drawing.Point(11, 140);
      this.cbxShowLoadingIcon.Margin = new System.Windows.Forms.Padding(2);
      this.cbxShowLoadingIcon.Name = "cbxShowLoadingIcon";
      this.cbxShowLoadingIcon.Size = new System.Drawing.Size(171, 24);
      this.cbxShowLoadingIcon.TabIndex = 3;
      this.cbxShowLoadingIcon.Text = "Show Loading Icon";
      this.cbxShowLoadingIcon.UseVisualStyleBackColor = true;
      this.cbxShowLoadingIcon.CheckedChanged += new System.EventHandler(this.cbxShowLoadingIcon_CheckedChanged);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.ForeColor = System.Drawing.Color.DarkRed;
      this.label1.Location = new System.Drawing.Point(535, 18);
      this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(151, 20);
      this.label1.TabIndex = 4;
      this.label1.Text = "Use at your own risk";
      // 
      // lblFormat
      // 
      this.lblFormat.AutoSize = true;
      this.lblFormat.ForeColor = System.Drawing.Color.DarkRed;
      this.lblFormat.Location = new System.Drawing.Point(11, 18);
      this.lblFormat.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblFormat.Name = "lblFormat";
      this.lblFormat.Size = new System.Drawing.Size(113, 20);
      this.lblFormat.TabIndex = 5;
      this.lblFormat.Text = "Format: , Max: ";
      // 
      // picPreview
      // 
      this.picPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.picPreview.BackColor = System.Drawing.Color.Black;
      this.picPreview.Location = new System.Drawing.Point(186, 51);
      this.picPreview.Name = "picPreview";
      this.picPreview.Size = new System.Drawing.Size(500, 280);
      this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.picPreview.TabIndex = 6;
      this.picPreview.TabStop = false;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(698, 342);
      this.Controls.Add(this.picPreview);
      this.Controls.Add(this.lblFormat);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.cbxShowLoadingIcon);
      this.Controls.Add(this.btnRecovery);
      this.Controls.Add(this.btnApply);
      this.Controls.Add(this.btnSelectImage);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Margin = new System.Windows.Forms.Padding(2);
      this.MaximizeBox = false;
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Boot logo changer";
      ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectImage;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnRecovery;
        private System.Windows.Forms.CheckBox cbxShowLoadingIcon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFormat;
    private PictureBox picPreview;
  }
}

