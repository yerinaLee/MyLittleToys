namespace MyLittleToys
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            settingsMenuItem = new ToolStripMenuItem();
            exitMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "MyLittleToys";
            notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Font = new Font("온글잎 류뚱체 Regular", 9.163635F, FontStyle.Regular, GraphicsUnit.Point, 129);
            contextMenuStrip1.ImageScalingSize = new Size(18, 18);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { settingsMenuItem, exitMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(199, 77);
            // 
            // settingsMenuItem
            // 
            settingsMenuItem.Font = new Font("온글잎 류뚱체 Regular", 11.1272717F, FontStyle.Regular, GraphicsUnit.Point, 129);
            settingsMenuItem.Name = "settingsMenuItem";
            settingsMenuItem.Size = new Size(198, 24);
            settingsMenuItem.Text = "설정(&S)";
            // 
            // exitMenuItem
            // 
            exitMenuItem.Font = new Font("온글잎 류뚱체 Regular", 11.1272717F, FontStyle.Regular, GraphicsUnit.Point, 129);
            exitMenuItem.Name = "exitMenuItem";
            exitMenuItem.Size = new Size(198, 24);
            exitMenuItem.Text = "종료(&X)";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem settingsMenuItem;
        private ToolStripMenuItem exitMenuItem;
    }
}
