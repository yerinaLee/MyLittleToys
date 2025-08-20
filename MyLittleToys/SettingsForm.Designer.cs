namespace MyLittleToys
{
    partial class SettingsForm
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
            label1 = new Label();
            btnSave = new Button();
            btnCancel = new Button();
            label2 = new Label();
            txtAotHotkey = new HotkeyTextBox();
            txtTextHotkey = new HotkeyTextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("온글잎 류뚱체 Regular", 11.1272717F, FontStyle.Regular, GraphicsUnit.Point, 129);
            label1.Location = new Point(32, 82);
            label1.Name = "label1";
            label1.Size = new Size(146, 19);
            label1.TabIndex = 0;
            label1.Text = "Always On Top 단축키";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(262, 247);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(86, 26);
            btnSave.TabIndex = 2;
            btnSave.Text = "저장";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(163, 247);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(86, 26);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "취소";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("온글잎 류뚱체 Regular", 11.1272717F, FontStyle.Regular, GraphicsUnit.Point, 129);
            label2.Location = new Point(200, 82);
            label2.Name = "label2";
            label2.Size = new Size(154, 19);
            label2.TabIndex = 4;
            label2.Text = "Text Extractor 단축키";
            // 
            // txtAotHotkey
            // 
            txtAotHotkey.Location = new Point(48, 131);
            txtAotHotkey.Name = "txtAotHotkey";
            txtAotHotkey.Size = new Size(115, 26);
            txtAotHotkey.TabIndex = 5;
            // 
            // txtTextHotkey
            // 
            txtTextHotkey.Location = new Point(220, 131);
            txtTextHotkey.Name = "txtTextHotkey";
            txtTextHotkey.Size = new Size(115, 26);
            txtTextHotkey.TabIndex = 6;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(378, 309);
            Controls.Add(txtTextHotkey);
            Controls.Add(txtAotHotkey);
            Controls.Add(label2);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(label1);
            Name = "SettingsForm";
            Text = "SettingsForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button btnSave;
        private Button btnCancel;
        private Label label2;
        private HotkeyTextBox txtAotHotkey;
        private HotkeyTextBox txtTextHotkey;
    }
}