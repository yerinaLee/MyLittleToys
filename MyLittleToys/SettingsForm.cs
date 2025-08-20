using System;
using System.Windows.Forms;

namespace MyLittleToys
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            this.Load += SettingsForm_Load;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            txtAotHotkey.Text = Properties.Settings.Default.AOT_Hotkey;
            txtTextHotkey.Text = Properties.Settings.Default.Text_Hotkey;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AOT_Hotkey = txtAotHotkey.Text;
            Properties.Settings.Default.Text_Hotkey = txtTextHotkey.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show("설정이 저장되었습니다.");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}