using System.Collections.Generic;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Microsoft.Win32;

namespace Windows_Switch
{
    public partial class Main : Form
    {
        private IKeyboardMouseEvents m_GlobalHook;
        public string AppName = "VirtualDesktopSwitch";
        public static List<Keys> keysDown = new List<Keys>();
        private bool openFrom = false;
        private bool exit = false;

        public Main()
        {
            InitializeComponent();

            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.KeyDown += OnKeyDown;
            m_GlobalHook.KeyUp += OnKeyUp;

            m_GlobalHook.MouseWheelExt += HookManager_MouseWheelExt;
        }

        private void HookManager_MouseWheelExt(object sender, MouseEventExtArgs e)
        {
            if (WIN() && CTRL())
            {
                if (e.Delta > 0)
                {
                    SendKeys.Send("{RIGHT}");
                }
                else
                {
                    SendKeys.Send("{LEFT}");
                }
                e.Handled = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            while (keysDown.Contains(e.KeyCode))
            {
                keysDown.Remove(e.KeyCode);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (keysDown.Contains(e.KeyCode) == false)
            {
                keysDown.Add(e.KeyCode);
            }
        }

        public static bool CTRL()
        {
            if (keysDown.Contains(Keys.LControlKey) ||
                keysDown.Contains(Keys.RControlKey) ||
                keysDown.Contains(Keys.Control) ||
                keysDown.Contains(Keys.ControlKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool WIN()
        {
            if (keysDown.Contains(Keys.LWin) ||
                keysDown.Contains(Keys.RWin))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AutoStart()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rkApp.GetValue(AppName) == null || rkApp.GetValue(AppName).ToString() != Application.ExecutablePath)
            {
                openFrom = true;
                rkApp.SetValue(AppName, Application.ExecutablePath);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            notifyIcon1.Visible = false;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisplayOnTray();
            if (!exit)
            {
                e.Cancel = true;
            }
        }

        private void mniOpenFrom_Click(object sender, System.EventArgs e)
        {
            openFrom = true;
            Show();
            notifyIcon1.Visible = false;
        }

        private void mniExit_Click(object sender, System.EventArgs e)
        {
            exit = true;
            Close();
        }

        private void Main_Load(object sender, System.EventArgs e)
        {
            AutoStart();
            if (!openFrom)
            {
                DisplayOnTray();
            }
        }
        private void DisplayOnTray()
        {
            Hide();
            notifyIcon1.Visible = true;
            ShowInTaskbar = false;
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            DisplayOnTray();
        }
    }
}
