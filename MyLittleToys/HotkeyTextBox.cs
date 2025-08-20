using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MyLittleToys
{
    public class HotkeyTextBox : TextBox
    {
        private static IntPtr _hookID = IntPtr.Zero;
        private static HotkeyTextBox _activeInstance = null;
        private LowLevelKeyboardProc _proc;

        public HotkeyTextBox()
        {
            _proc = HookCallback;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            _activeInstance = this;
            _hookID = SetHook(_proc);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
            _activeInstance = null;
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                if (_activeInstance != null)
                {
                    _activeInstance.ProcessKeyPress(key);
                    // Win 키가 포함된 조합키가 OS로 전달되는 것을 차단합니다.
                    if (IsKeyDown(Keys.LWin) || IsKeyDown(Keys.RWin))
                    {
                        return (IntPtr)1; // 메시지 처리 완료, OS로 보내지 않음
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private delegate void SetTextCallback(string text);

        private void ProcessKeyPress(Keys keyPressed)
        {
            var keys = new List<string>();
            if (IsKeyDown(Keys.ControlKey)) keys.Add("Control");
            if (IsKeyDown(Keys.ShiftKey)) keys.Add("Shift");
            if (IsKeyDown(Keys.Menu)) keys.Add("Alt");
            if (IsKeyDown(Keys.LWin) || IsKeyDown(Keys.RWin)) keys.Add("Win");

            if (!IsModifierKey(keyPressed))
            {
                keys.Add(keyPressed.ToString());
                string newText = string.Join(", ", keys);

                if (this.InvokeRequired)
                {
                    this.Invoke(new SetTextCallback(SetText), new object[] { newText });
                }
                else
                {
                    this.Text = newText;
                }
            }
        }

        private void SetText(string text)
        {
            this.Text = text;
        }

        protected override void OnKeyPress(KeyPressEventArgs e) { e.Handled = true; }
        protected override void OnKeyDown(KeyEventArgs e) { e.SuppressKeyPress = true; }

        private bool IsModifierKey(Keys key)
        {
            return key == Keys.ControlKey || key == Keys.LControlKey || key == Keys.RControlKey ||
                   key == Keys.ShiftKey || key == Keys.LShiftKey || key == Keys.RShiftKey ||
                   key == Keys.Menu || key == Keys.Alt ||
                   key == Keys.LWin || key == Keys.RWin;
        }

        #region P/Invoke
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        private static bool IsKeyDown(Keys key)
        {
            return (GetKeyState((int)key) & 0x8000) != 0;
        }
        #endregion
    }
}