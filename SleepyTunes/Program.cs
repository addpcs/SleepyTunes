using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SleepyTunes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Gui());
        }

        #region Dll Imports

        [DllImport("user32.dll")]
        public static extern void LockWorkStation();

        [DllImport("user32.dll")]
        public static extern int ExitWindowsEx(uint uFlags, int dwReason);

        public enum ExitVals : uint
        {
            LogOff = 0x10,
            PowerOff = 0x18,
            Reboot = 0x12,
            Shutdown = 0x11
        }

        #endregion
    }
}
