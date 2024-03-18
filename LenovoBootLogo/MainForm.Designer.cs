using System.Windows.Forms;

namespace LenovoBootLogo
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnSelectImage = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnRecovery = new System.Windows.Forms.Button();
            this.cbxShowLoadingIcon = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblFormat = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SelectImage
            // 
            this.btnSelectImage.Location = new System.Drawing.Point(24, 22);
            this.btnSelectImage.Name = "btnSelectImage";
            this.btnSelectImage.Size = new System.Drawing.Size(199, 69);
            this.btnSelectImage.TabIndex = 0;
            this.btnSelectImage.Text = "Select Image";
            this.btnSelectImage.UseVisualStyleBackColor = true;
            this.btnSelectImage.Click += new System.EventHandler(this.btnSelectImage_Click);
            // 
            // Apply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(244, 22);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(199, 69);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // Recovery
            // 
            this.btnRecovery.Location = new System.Drawing.Point(771, 22);
            this.btnRecovery.Name = "btnRecovery";
            this.btnRecovery.Size = new System.Drawing.Size(199, 69);
            this.btnRecovery.TabIndex = 2;
            this.btnRecovery.Text = "Reset to default";
            this.btnRecovery.UseVisualStyleBackColor = true;
            this.btnRecovery.Click += new System.EventHandler(this.btnRecovery_Click);
            // 
            // checkBox1
            // 
            this.cbxShowLoadingIcon.AutoSize = true;
            this.cbxShowLoadingIcon.Enabled = false;
            this.cbxShowLoadingIcon.Location = new System.Drawing.Point(495, 43);
            this.cbxShowLoadingIcon.Name = "cbxShowLoadingIcon";
            this.cbxShowLoadingIcon.Size = new System.Drawing.Size(226, 29);
            this.cbxShowLoadingIcon.TabIndex = 3;
            this.cbxShowLoadingIcon.Text = "Show Loading Icon";
            this.cbxShowLoadingIcon.UseVisualStyleBackColor = true;
            this.cbxShowLoadingIcon.CheckedChanged += new System.EventHandler(this.cbxShowLoadingIcon_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.DarkRed;
            this.label1.Location = new System.Drawing.Point(763, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Use at your own risk";
            // 
            // label2
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.ForeColor = System.Drawing.Color.DarkRed;
            this.lblFormat.Location = new System.Drawing.Point(19, 117);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(156, 25);
            this.lblFormat.TabIndex = 5;
            this.lblFormat.Text = "Format: , Max: ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(997, 169);
            this.Controls.Add(this.lblFormat);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxShowLoadingIcon);
            this.Controls.Add(this.btnRecovery);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnSelectImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ShowIcon = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Name = "MainForm";
            this.Text = "Lenovo boot logo changer";
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
    }
}

