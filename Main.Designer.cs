
namespace MonitorAutoBrightness
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label1 = new System.Windows.Forms.Label();
            this.SensorValueLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BrightnessValueLabel = new System.Windows.Forms.Label();
            this.TrayIconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CloseAppMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayIconMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sensor value:";
            // 
            // SensorValueLabel
            // 
            this.SensorValueLabel.Location = new System.Drawing.Point(126, 9);
            this.SensorValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SensorValueLabel.Name = "SensorValueLabel";
            this.SensorValueLabel.Size = new System.Drawing.Size(140, 20);
            this.SensorValueLabel.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 41);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Brightness:";
            // 
            // BrightnessValueLabel
            // 
            this.BrightnessValueLabel.Location = new System.Drawing.Point(126, 41);
            this.BrightnessValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.BrightnessValueLabel.Name = "BrightnessValueLabel";
            this.BrightnessValueLabel.Size = new System.Drawing.Size(165, 20);
            this.BrightnessValueLabel.TabIndex = 3;
            this.BrightnessValueLabel.Text = "-";
            // 
            // TrayIconMenu
            // 
            this.TrayIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CloseAppMenuItem});
            this.TrayIconMenu.Name = "TrayIconMenu";
            this.TrayIconMenu.Size = new System.Drawing.Size(94, 26);
            // 
            // CloseAppMenuItem
            // 
            this.CloseAppMenuItem.Name = "CloseAppMenuItem";
            this.CloseAppMenuItem.Size = new System.Drawing.Size(93, 22);
            this.CloseAppMenuItem.Text = "Exit";
            this.CloseAppMenuItem.Click += new System.EventHandler(this.CloseAppMenuItem_Click);
            // 
            // TrayIcon
            // 
            this.TrayIcon.ContextMenuStrip = this.TrayIconMenu;
            this.TrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIcon.Icon")));
            this.TrayIcon.Text = "Monitor Auto-brightness";
            this.TrayIcon.Visible = true;
            this.TrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseClick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(318, 77);
            this.Controls.Add(this.BrightnessValueLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SensorValueLabel);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monitor Auto-brightness";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.Resize += new System.EventHandler(this.Main_Resize);
            this.TrayIconMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label SensorValueLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label BrightnessValueLabel;
        private System.Windows.Forms.ContextMenuStrip TrayIconMenu;
        private System.Windows.Forms.ToolStripMenuItem CloseAppMenuItem;
        private System.Windows.Forms.NotifyIcon TrayIcon;
    }
}

