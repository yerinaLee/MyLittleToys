using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Tesseract;

namespace MyLittleToys
{
    public partial class Form1 : Form
    {
        #region Win32 API 및 상수 선언
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);
        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
        private const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;
        private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        private const uint EVENT_OBJECT_DESTROY = 0x8001;
        private const int MOD_WIN = 0x0008;
        private const int HOTKEY_ID_AOT = 1;
        private const int HOTKEY_ID_TEXT = 2;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT { public int Left, Top, Right, Bottom; }

        private Dictionary<IntPtr, bool> windowStates = new Dictionary<IntPtr, bool>();
        private Dictionary<IntPtr, HighlightForm> highlightForms = new Dictionary<IntPtr, HighlightForm>();
        private Dictionary<IntPtr, System.Windows.Forms.Timer> moveTimers = new Dictionary<IntPtr, System.Windows.Forms.Timer>();

        private List<IntPtr> _eventHooks = new List<IntPtr>();
        private WinEventDelegate _eventDelegate;
        #endregion

        public Form1()
        {
            InitializeComponent();
            _eventDelegate = new WinEventDelegate(WinEventProc);

            _eventHooks.Add(SetWinEventHook(EVENT_SYSTEM_MOVESIZESTART, EVENT_SYSTEM_MOVESIZEEND, IntPtr.Zero, _eventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT));
            _eventHooks.Add(SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _eventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT));
            _eventHooks.Add(SetWinEventHook(EVENT_OBJECT_DESTROY, EVENT_OBJECT_DESTROY, IntPtr.Zero, _eventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT));
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (eventType == EVENT_OBJECT_DESTROY && highlightForms.ContainsKey(hwnd))
            {
                RemoveHighlight(hwnd);
                return;
            }

            if (highlightForms.ContainsKey(hwnd))
            {
                switch (eventType)
                {
                    case EVENT_SYSTEM_MOVESIZESTART:
                        StartTrackingWindow(hwnd);
                        break;

                    case EVENT_SYSTEM_MOVESIZEEND:
                        StopTrackingWindow(hwnd);
                        UpdateHighlightPosition(hwnd);
                        break;

                    case EVENT_SYSTEM_FOREGROUND:
                        UpdateHighlightPosition(hwnd);
                        break;
                }
            }
        }

        private void StartTrackingWindow(IntPtr hwnd)
        {
            if (moveTimers.ContainsKey(hwnd)) return;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            timer.Tick += (s, e) => UpdateHighlightPosition(hwnd);
            moveTimers[hwnd] = timer;
            timer.Start();
        }

        private void StopTrackingWindow(IntPtr hwnd)
        {
            if (moveTimers.TryGetValue(hwnd, out System.Windows.Forms.Timer timer))
            {
                timer.Stop();
                timer.Dispose();
                moveTimers.Remove(hwnd);
            }
        }

        private void UpdateAllHighlightPositions()
        {
            foreach (var handle in highlightForms.Keys.ToList())
            {
                UpdateHighlightPosition(handle);
            }
        }

        private void UpdateHighlightPosition(IntPtr handle)
        {
            if (highlightForms.TryGetValue(handle, out HighlightForm form))
            {
                if (DwmGetWindowAttribute(handle, DWMWA_EXTENDED_FRAME_BOUNDS, out RECT rect, Marshal.SizeOf(typeof(RECT))) == 0)
                {
                    form.UpdatePosition(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Hide();
            LoadAndRegisterHotkeys();
            settingsMenuItem.Click += settingsMenuItem_Click;
            exitMenuItem.Click += exitMenuItem_Click;
        }

        private void LoadAndRegisterHotkeys()
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID_AOT);
            UnregisterHotKey(this.Handle, HOTKEY_ID_TEXT);
            try
            {
                var aotHotkey = ParseHotkey(Properties.Settings.Default.AOT_Hotkey);
                if (aotHotkey.Key != Keys.None) RegisterHotKey(this.Handle, HOTKEY_ID_AOT, aotHotkey.Modifiers, (int)aotHotkey.Key);

                var textHotkey = ParseHotkey(Properties.Settings.Default.Text_Hotkey);
                if (textHotkey.Key != Keys.None) RegisterHotKey(this.Handle, HOTKEY_ID_TEXT, textHotkey.Modifiers, (int)textHotkey.Key);
            }
            catch { }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x0312)
            {
                switch (m.WParam.ToInt32())
                {
                    case HOTKEY_ID_AOT: ToggleAlwaysOnTop(); break;
                    case HOTKEY_ID_TEXT: ExtractTextFromScreen(); break;
                }
            }
        }

        private void ToggleAlwaysOnTop()
        {
            IntPtr handle = GetForegroundWindow();
            if (handle == IntPtr.Zero || handle == this.Handle) return;

            bool isTopMost = windowStates.ContainsKey(handle) && windowStates[handle];

            if (isTopMost)
            {
                RemoveHighlight(handle);
            }
            else
            {
                SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
                windowStates[handle] = true;

                if (!highlightForms.ContainsKey(handle))
                {
                    var highlight = new HighlightForm();
                    highlight.Show();
                    highlightForms[handle] = highlight;
                    UpdateHighlightPosition(handle);
                }
            }
        }

        private void RemoveHighlight(IntPtr handle)
        {
            SetWindowPos(handle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            if (windowStates.ContainsKey(handle)) windowStates.Remove(handle);
            if (highlightForms.ContainsKey(handle))
            {
                highlightForms[handle].Close();
                highlightForms.Remove(handle);
            }

            StopTrackingWindow(handle); // 타이머 정리
        }

        private void ExtractTextFromScreen()
        {
            using (var overlay = new OverlayForm())
            {
                if (overlay.ShowDialog() == DialogResult.OK && overlay.CapturedImage != null)
                {
                    try
                    {
                        string languages = "kor+eng+jpn+chi_sim+chi_tra";
                        using (var engine = new TesseractEngine(@"./tessdata", languages, EngineMode.Default))
                        using (var ms = new MemoryStream())
                        {
                            overlay.CapturedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            using (var pix = Pix.LoadFromMemory(ms.ToArray()))
                            using (var page = engine.Process(pix))
                            {
                                string text = page.GetText();
                                if (!string.IsNullOrWhiteSpace(text)) Clipboard.SetText(text);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("OCR 처리 중 오류가 발생했습니다: " + ex.Message);
                    }
                }
            }
        }

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm())
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    LoadAndRegisterHotkeys();
                }
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var hook in _eventHooks)
            {
                UnhookWinEvent(hook);
            }

            foreach (var timer in moveTimers.Values)
            {
                timer.Stop();
                timer.Dispose();
            }

            UnregisterHotKey(this.Handle, HOTKEY_ID_AOT);
            UnregisterHotKey(this.Handle, HOTKEY_ID_TEXT);
        }

        private (int Modifiers, Keys Key) ParseHotkey(string hotkeyString)
        {
            if (string.IsNullOrWhiteSpace(hotkeyString)) return (0, Keys.None);

            var parts = hotkeyString.ToUpper().Split(',').Select(s => s.Trim()).ToList();
            var key = (Keys)Enum.Parse(typeof(Keys), parts.Last());

            int modifiers = 0;
            if (parts.Contains("CONTROL")) modifiers |= 0x0002;
            if (parts.Contains("SHIFT")) modifiers |= 0x0004;
            if (parts.Contains("ALT")) modifiers |= 0x0001;
            if (parts.Contains("WIN")) modifiers |= MOD_WIN;

            return (modifiers, key);
        }
    }
}
